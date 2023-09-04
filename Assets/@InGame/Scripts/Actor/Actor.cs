using UnityEngine;
using UnityEngine.Events;
using UniRx;

namespace PSB.InGame
{
    public enum ActorType
    {
        None,
        Kinpatsu,
        KinpatsuLeader,
        Kurokami,
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
        [SerializeField] SkinnedMeshRenderer _renderer;
        [SerializeField] Material _defaultMaterial;

        ActionEvaluator _evaluator;
        SightSensor _sightSensor;
        BaseState _currentState;
        bool _initialized;
        bool _isDead;

        // キャラクターの各種パラメータ。初期化前に読み取った場合は仮の値を返す。
        public float Food         => _initialized ? _context.Food.Percentage : default;
        public float Water        => _initialized ? _context.Water.Percentage : default;
        public float HP           => _initialized ? _context.HP.Percentage : default;
        public float LifeSpan     => _initialized ? _context.LifeSpan.Percentage : default;
        public float BreedingRate => _initialized ? _context.BreedingRate.Percentage : default;
        public string StateName   => _initialized ? _currentState.Type.ToString() : string.Empty;
        public ActorType Type     => _initialized ? _context.Type : ActorType.None;
        // 死んだ場合(プールに返却した)のフラグ
        public bool IsDead => _initialized ? _isDead : false;

        /// <summary>
        /// スポナーから生成された際にスポナー側が呼び出して初期化する必要がある。
        /// </summary>
        public void Init(uint? gene = null) 
        {
            _isDead = false;

            _context.Init(gene);
            ApplyGene();
            _currentState = _context.EvaluateState;
            _evaluator ??= new(_context);
            _sightSensor ??= new(_context);

            OnSpawned?.Invoke(this);
            _initialized = true;
        }

        // 死亡する際に非表示になる
        void OnDisable()
        {
            _isDead = true;

            // 死亡したメッセージの送信
            MessageBroker.Default.Publish(new ActorDeathMessage());
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
        public void StepAction()
        {
            _currentState = _currentState.Update();
        }

        /// <summary>
        /// 自身の情報とリーダーの評価値を元に次の行動を決める。
        /// </summary>
        public void Evaluate(float[] leaderEvaluate)
        {
            // 評価ステート以外では評価しないことで、毎フレーム評価処理を行うのを防ぐ
            if (_currentState != _context.EvaluateState) return;

            // 周囲の敵と物を検知
            _context.Enemy = _sightSensor.SearchTarget(_context.EnemyTag);
            
            // TODO:追加の評価リソースがある場合はここに書く
            
            // 黒板に書き込む
            _context.NextAction = _evaluator.SelectAction(leaderEvaluate);
        }

        // ↓リーダーのみのメソッド
        public float[] LeaderEvaluate()
        {
            // 本来はpublicな黒板を見て行動を評価する
            float[] eval = new float[Utility.GetEnumLength<ActionType>() - 1];
            eval[(int)ActionType.Gather] = 1;

            return eval;
        }
    }

    // バグ:経路が見つからないエラーが出るバグ
    // 出来れば: 他のステートも繁殖ステートと同じく途中で餓死＆殺害されるよう修正
    // 次タスク: リーダーが死んだ際の処理、群れの長がいないといけない
    // 次タスク: リーダーが死ぬとランダムで次のリーダーが決まる。群れの最後の1匹が死ぬとがめおべら
    // 次タスク: 個体の強さを数値化する。サイズと色で求め、各種評価にはその値を使う。

    // うろうろのStayから寿命のStay？寿命のEnterが呼ばれていないのにStayが呼ばれている。
    // なぜ？しかも1度プールに戻してから取り出した個体のみ。

    // 繁殖時のバグ(修正済み？)
    // 餓死ステートに遷移した状態でも繁殖候補として残ってしまっている
    // なので餓死ステートでも死なない？
    // しかし、餓死ステートに入れば強制的にレンダラが無効化され、表示しなくなるはず
    // 餓死状態になると餓死ステートに遷移はする
    // マッチング待機状態でバグっている
    // 餓死ステートに遷移したまま番まで移動している
    // 餓死ステートのまま繁殖の番を待っている

    // Exitが呼ばれないで餓死ステートに遷移している？
    // 進捗:Stateを表示していると思っていたテキストが評価したアクションを表示していた。

    // 一定間隔で評価 or 毎フレーム評価
    //  ステートの更新タイミングはセルの上にいる時
    //  毎フレーム評価しても大丈夫？なはず
    //  リーダーからの命令はどうする？ステートの更新タイミングは一定間隔なので、
    //   1フレームだけの命令の送信は正常に動かない可能性がある。

    // ステートマシンはどうする？
    // 評価 -> 行動 ->
    // タイミングについて。
    // 評価 -> 書き込むは同一フレームで行う。

    // 水分: 時間経過で減る、飲んで回復
    // 食料: 時間経過で減る、食べて回復
    // 体力: 0になると死ぬ
    // 水分と食料が0の時は減っていく
    // 攻撃されると体力が減る
    // 水分と体力が満たされている場合、回復していく
    // 繁殖率: 体力が高い状態だと加算されていく。繁殖したらリセットされる
    // 一度繁殖した個体は繁殖しないようにすると子供が死んだらゲームが詰む
    // 寿命: 0になったら死ぬ。他のパラメータに影響されず常に一定量減少していく。

    // リーダーが死ぬとランダムで次のリーダーが決まる。
    // 群れの最後の1匹が死ぬとがめおべら

    // 一定間隔で水分と食料のうち少ないほうを満たそうとする
    // 具体的には何回かセルを移動したらチェック
    // 満たされているかチェック。満たされている場合は何もしない。
    // 近くの水源もしくは食料のマスを取得
    // ハムで経路探索。経路があれば向かう。無い場合はなにもしない

    // 行動の単位はアニメーション
    // アニメーションが終わったら次の行動を選択する
    // つまり、行動A -> 判断 -> 行動B と行動毎に中央の状態に戻って次の行動をチェックする必要がある。
    // 通常のステートベースでは無い。
    // 外部からの攻撃などで行動中に死ぬ場合がある？

    // 繁殖する際は親の特徴を受け継ぐ(遺伝)
    // 遺伝的アルゴリズム
    // 遺伝子は4つ、RGB+サイズ、

    // キャラクター:黒髪
    // リーダーがいない。各々が勝手に行動する。
    // 金髪を見つけると攻撃してくる。
    // 攻撃で死ぬ。
    // 繁殖はしない。
    // ランダムで沸く。
    // 死ぬたびに遺伝的な変異をする？強くなったり弱くなったり
}