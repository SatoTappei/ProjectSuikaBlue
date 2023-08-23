using UnityEngine;
using UnityEngine.Events;
using UniRx;
using System;
using System.Buffers;

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
    [RequireComponent(typeof(InitializeProcess))]
    [RequireComponent(typeof(ActionEvaluator))]
    [RequireComponent(typeof(BlackBoard))]
    public class Actor : MonoBehaviour, IReadOnlyParams, IReadOnlyBreedingParam
    {
        public static event UnityAction<Actor> OnSpawned;

        [SerializeField] InitializeProcess _initProcess;
        [SerializeField] ActionEvaluator _evaluator;
        // StatusBaseの取得やController側での制御に必要なので個体毎にデータを持つ
        [SerializeField] ActorType _type;

        IBlackBoardForActor _blackBoard;
        Status _status;
        BaseState _currentState;
        string _name;
        bool _initialized;

        public ActorType Type => _type;
        // 読み取る用。初期化前に読み取った場合は仮の値として1を返す。
        float IReadOnlyParams.Food         => _initialized ? _status.Food.Percentage : 1;
        float IReadOnlyParams.Water        => _initialized ? _status.Water.Percentage : 1;
        float IReadOnlyParams.HP           => _initialized ? _status.Hp.Percentage : 1;
        float IReadOnlyParams.LifeSpan     => _initialized ? _status.LifeSpan.Percentage : 1;
        float IReadOnlyParams.BreedingRate => _initialized ? _status.BreedingRate.Percentage : 1;
        string IReadOnlyEvaluate.ActionName => _initialized ? _blackBoard.NextAction.ToString() : string.Empty;
        string IReadOnlyObjectInfo.Name => _initialized ? _name ??= gameObject.name : string.Empty;
        // 繁殖ステートが読み取る用。
        uint IReadOnlyBreedingParam.Gene => _status.Gene;

        /// <summary>
        /// スポナーが生成のタイミングで呼ぶ初期化処理
        /// </summary>
        public void Init(uint? gene = null) 
        {
            // 初期化完了後、各種ステータスの値を参照できる。
            _status = _initProcess.Execute(gene, _type);
            _initialized = true;

            // FSMの準備。評価ステートを初期状態のステートとする。
            _blackBoard = GetComponent<BlackBoard>();
            _currentState = _blackBoard.InitState;

            // 食べる/飲むステートがステータスを変化させるように登録する
            _blackBoard.OnEatFoodRegister(v => _status.Food.Value += v);
            _blackBoard.OnDrinkWaterRegister(v => _status.Water.Value += v);

            // 繁殖ステートが自身が雄/雌の時に行う処理をそれぞれ登録する
            _blackBoard.OnFemaleBreedingRegister(SendSpawnChildMessage);
            _blackBoard.OnFemaleBreedingRegister(_ => _status.BreedingRate.Value = 0);
            _blackBoard.OnMaleBreedingRegister(() => _status.BreedingRate.Value = 0);

            OnSpawned?.Invoke(this);
        }

        /// <summary>
        /// 毎フレーム呼び出され、呼び出されるたびにパラメータを1フレーム分だけ変化させる
        /// </summary>
        public void StepParams()
        {
            _status.StepFood();
            _status.StepWater();
            _status.StepLifeSpan();
           
            // 食料と水分が0以下なら体力を減らす
            if (_status.Food.IsBelowZero && _status.Water.IsBelowZero)
            {
                _status.StepHp();
            }
            // 体力が一定以上なら繁殖率が増加する
            if (_status.IsBreedingRateIncrease)
            {
                _status.StepBreedingRate();
            }
        }

        /// <summary>
        /// 毎フレーム呼び出され、呼び出されるたびに現在のステートを1フレーム分だけ更新する
        /// </summary>
        public void StepAction()
        {
            _currentState = _currentState.Update();
        }

        public void Evaluate()
        {
            // ダミーを作って呼び出す
            int length = Utility.GetEnumLength<ActionType>() - 1;
            float[] buffer = ArrayPool<float>.Shared.Rent(length);
            float[] dummy = buffer.AsSpan(0, length).ToArray();

            Evaluate(dummy);

            ArrayPool<float>.Shared.Return(buffer);
        }

        public void Evaluate(float[] leaderEvaluate)
        {
            // リーダーの各行動への評価との合算で選択する
            float[] myEvaluate = _evaluator.Evaluate(_status);
            ActionType action = ActionEvaluator.SelectMax(myEvaluate, leaderEvaluate);

            // 黒板に書き込み
            _blackBoard.NextAction = action;
        }

        void SendSpawnChildMessage(uint gene)
        {
            MessageBroker.Default.Publish(new SpawnChildMessage 
            {
                Gene1 = gene,
                Gene2 = _status.Gene,
                Params = this,
                Pos = transform.position, // 自身の位置に生成する
            });
        }
    }

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

    // 黒髪
        // リーダーがいない。各々が勝手に行動する。
        // 金髪を見つけると攻撃してくる。
        // 攻撃で死ぬ。
        // 繁殖はしない。
        // ランダムで沸く。
        // 死ぬたびに遺伝的な変異をする？強くなったり弱くなったり
}