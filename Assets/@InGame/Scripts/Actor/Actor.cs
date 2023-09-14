using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

# region メモ
// 出来れば: 他のステートも繁殖ステートと同じく途中で餓死＆殺害されるよう修正
// 出来れば: 生成数による生成の判定がバグっている
// 変更案: 個体の強さを数値化する。サイズと色で求め、各種評価にはその値を使う。
// 変更案: 1つではなく、優先度でソートして次の行動を保持。
// バグ: 集合を選択した際に、経路の末端に2体のキャラクターが被ってしまう。初期状態9人で発生

// ◎攻撃
// 1対多の状況はどうする？
// 敵を倒した際にも評価ステートに遷移する必要がある。
//  案1:1発殴ったら評価ステートに遷移 <- 体力が減ったら自動で逃げるはず。
//  殺した場合は敵を検知せずこのステートに遷移してこないはず。
//  必要な値:総合スコア(色、サイズ) <- この値によって攻撃力が変わる
// ◎リーダー
// リーダーが死ぬとランダムで次のリーダーが決まる。
// 群れの最後の1匹が死ぬとがめおべら
// ◎食事/水分
// 一定間隔で水分と食料のうち少ないほうを満たそうとする
// 具体的には何回かセルを移動したらチェック
// 満たされているかチェック。満たされている場合は何もしない。
// 近くの水源もしくは食料のマスを取得
// ハムで経路探索。経路があれば向かう。無い場合はなにもしない
// ◎行動
// 行動の単位はアニメーション
// アニメーションが終わったら次の行動を選択する
// つまり、行動A -> 判断 -> 行動B と行動毎に中央の状態に戻って次の行動をチェックする必要がある。
// 通常のステートベースでは無い。
// 外部からの攻撃などで行動中に死ぬ場合がある？
// ◎繁殖
// 繁殖する際は親の特徴を受け継ぐ(遺伝)
// 遺伝的アルゴリズム
// 遺伝子は4つ、RGB+サイズ、
// ◎敵
// キャラクター:黒髪
// リーダーがいない。各々が勝手に行動する。
// 金髪を見つけると攻撃してくる。
// 攻撃で死ぬ。
// 繁殖はしない。
// ランダムで沸く。
// 死ぬたびに遺伝的な変異をする？強くなったり弱くなったり
#endregion

namespace PSB.InGame
{
    public enum ActorType
    {
        None,
        Kinpatsu,
        KinpatsuLeader,
        Kurokami,
    }

    public enum Sex
    {
        Male,
        Female,
    }

    /// <summary>
    /// スポナーから生成され、Controllerによって操作される。
    /// 単体で動作することを考慮していないのでシーン上に直に配置しても機能しない。
    /// </summary>
    [RequireComponent(typeof(DataContext))]
    public class Actor : MonoBehaviour, IReadOnlyActorStatus
    {
        public static event UnityAction<Actor> OnSpawned;

        [SerializeField] DataContext _context;
        [SerializeField] ChildSpawnBehavior _childSpawn;
        [SerializeField] SkinnedMeshRenderer _renderer;
        [SerializeField] Material _defaultMaterial;
        [SerializeField] GameObject _leaderMarker;

        ActionEvaluator _evaluator;
        LeaderEvaluator _leaderEvaluator;
        Pathfinder _pathfinder;
        BaseState _currentState;
        bool _initialized; // Initメソッドが呼び出されたフラグ
        bool _isDead;      // 死亡(非表示になった)フラグ

        // 初期化前に読み取った場合は仮の値を返す。
        public float Food         => _initialized ? _context.Food.Percentage : default;
        public float Water        => _initialized ? _context.Water.Percentage : default;
        public float HP           => _initialized ? _context.HP.Percentage : default;
        public float LifeSpan     => _initialized ? _context.LifeSpan.Percentage : default;
        public float BreedingRate => _initialized ? _context.BreedingRate.Percentage : default;
        public StateType State    => _initialized ? _currentState.Type : StateType.Base;
        public ActorType Type     => _initialized ? _context.Type : ActorType.None;
        public Sex Sex            => _initialized ? _context.Sex : Sex.Male;
        public int Score          => _initialized ? _context.Score : default;
        public bool IsDead        => _initialized ? _isDead : false;

        public bool IsLeader
        {
            get => _context.IsLeader;
            set
            {
                _context.IsLeader = value;
                _leaderMarker.SetActive(_context.IsLeader);
            }
        }

        /// <summary>
        /// スポナーから生成された際にスポナー側が呼び出して初期化する必要がある。
        /// </summary>
        public void Init(uint? gene = null) 
        {
            _isDead = false;
            IsLeader = false;

            _context.Init(gene);
            ApplyGene();
            _currentState = _context.EvaluateState;
            _evaluator ??= new(_context);
            _leaderEvaluator ??= new(_context);
            _pathfinder ??= new(_context);
            // 初期位置のセル上に居るという情報を書き込む
            FieldManager.Instance.SetActorOnCell(transform.position, _context.Type);
            // わかりやすいように名前に性別を反映しておく
            name += _context.Sex == Sex.Male ? "♂" : "♀";

            OnSpawned?.Invoke(this);
            MessageBroker.Default.Publish(new ActorSpawnMessage() { Type = _context.Type });
            
            // 初期化完了フラグ
            _initialized = true;
        }

        void OnDisable()
        {
            // 死亡する際に非表示になる
            _isDead = true;
        }

        /// <summary>
        /// 遺伝子を反映してサイズと色を変える
        /// </summary>
        void ApplyGene()
        {
            _context.Model.localScale *= _context.Size;

            // 現在のマテリアルを削除
            Destroy(_renderer.material);
            // デフォルトのコピーからマテリアルを作成
            Material next = new(_defaultMaterial);
            next.SetColor("_BaseColor", _context.Color);
            _renderer.material = next;
        }

        /// <summary>
        /// パラメータを1フレーム分だけ変化させる
        /// </summary>
        public void StepParams()
        {
            _context.StepFood();
            _context.StepWater();
            _context.StepLifeSpan();
           
            // 食料と水分が0以下なら体力を減らす
            if (_context.Food.IsBelowZero && _context.Water.IsBelowZero)
            {
                _context.StepHp();
            }
            // 体力が一定以上なら繁殖率が増加する
            if (_context.IsBreedingRateIncrease)
            {
                _context.StepBreedingRate();
            }
        }

        /// <summary>
        /// 現在のステートを1フレーム分だけ更新する
        /// </summary>
        public void StepAction() => _currentState = _currentState.Update();

        /// <summary>
        /// 評価ステートだった場合にリセットする処理
        /// </summary>
        public void ResetOnEvaluateState()
        {
            // 交尾中だった場合は中止
            _childSpawn.Cancel();
        }

        /// <summary>
        /// 評価ステート以外では評価しないことで、毎フレーム評価処理を行うのを防ぐ
        /// 自身の情報とリーダーの評価値を元に次の行動を決める。
        /// </summary>
        public void Evaluate(float[] leaderEvaluate)
        {

            // 敵に狙われている場合は、攻撃もしくは逃げることが最優先なので、評価値より先に判定する
            _pathfinder.SearchEnemy();

            // 評価値を用いて次の行動を選択
            ActionType action = _evaluator.SelectAction(leaderEvaluate);
            switch (action)
            {
                // 殺害
                case ActionType.Killed:
                    _context.NextAction = ActionType.Killed; return;
                // 寿命
                case ActionType.Senility:
                    _context.NextAction = ActionType.Senility; return;
                // 攻撃
                case ActionType.Attack when _pathfinder.TryPathfindingToEnemy():
                    _context.NextAction = ActionType.Attack; return;
                // 逃げる
                case ActionType.Escape when _pathfinder.TryPathfindingToEscapePoint():
                    _context.NextAction = ActionType.Escape; return;
                // 雄繁殖
                case ActionType.Breed when _context.Sex == Sex.Male && _pathfinder.TryDetectPartner():
                    _context.NextAction = ActionType.Breed; return;
                // 雌繁殖
                case ActionType.Breed when _context.Sex == Sex.Female:
                    _context.NextAction = ActionType.Breed; return;
                // 水分
                case ActionType.SearchWater when _pathfinder.TryPathfindingToResource(ResourceType.Water):
                    _context.NextAction = ActionType.SearchWater; return;
                // 食料
                case ActionType.SearchFood when _pathfinder.TryPathfindingToResource(ResourceType.Tree):
                    _context.NextAction = ActionType.SearchFood; return;
                // 集合
                case ActionType.Gather when _pathfinder.TryPathfindingToGatherPoint():
                    _context.NextAction = ActionType.Gather; return;
            }

            // うろうろステートに必要のない参照を消す
            _context.Enemy = null;
            //_context.Path.Clear();

            // ランダムに隣のセルに移動する
            _context.NextAction = ActionType.Wander;
        }

        /// <summary>
        /// 繁殖を行う
        /// 雄が雌のこのメソッドを呼び出す
        /// </summary>
        public void SpawnChild(uint maleGene, UnityAction callback = null)
        {
            if (_currentState.Type != StateType.FemaleBreed)
            {
                Debug.LogWarning($"{name} 雌の繁殖ステート以外で子供を産む処理が呼ばれている。");
            }
            else
            {
                _childSpawn.SpawnChild(maleGene, callback);
            }
        }

        /// <summary>
        /// リーダーとしての評価を行う
        /// 群れの共通の黒板を用いて評価を行う
        /// </summary>
        /// <returns>各行動に対しての評価値</returns>
        public float[] LeaderEvaluate() => _leaderEvaluator.Evaluate();

        void OnDrawGizmos()
        {
            // 周囲八近傍のセルの距離を描画
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, Utility.NeighbourCellRadius);
        }
    }
}