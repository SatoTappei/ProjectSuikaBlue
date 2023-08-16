using UnityEngine;
using UnityEngine.Events;

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
    [RequireComponent(typeof(ChildSpawner))]
    [RequireComponent(typeof(InitializeProcess))]
    public class Actor : MonoBehaviour, IReadOnlyParams
    {
        public static event UnityAction<Actor> OnSpawned;

        [SerializeField] ChildSpawner _spawner;
        [SerializeField] InitializeProcess _initProcess;
        // StatusBaseの取得やController側での制御に必要なので個体毎にデータを持つ
        [SerializeField] ActorType _type;

        Status _status;
        bool _initialized;

        public ActorType Type => _type;
        // UI側が読み取る用。初期化前に読み取った場合は仮の値として1を返す。
        float IReadOnlyParams.Food         => _initialized ? _status.Food.Percentage : 1;
        float IReadOnlyParams.Water        => _initialized ? _status.Water.Percentage : 1;
        float IReadOnlyParams.HP           => _initialized ? _status.Hp.Percentage : 1;
        float IReadOnlyParams.LifeSpan     => _initialized ? _status.LifeSpan.Percentage : 1;
        float IReadOnlyParams.BreedingRate => _initialized ? _status.BreedingRate.Percentage : 1;

        /// <summary>
        /// スポナーが生成のタイミングで呼ぶ初期化処理
        /// </summary>
        public void Init(uint? gene = null) 
        {
            // 初期化完了後、各種ステータスの値を参照できる。
            _status = _initProcess.Execute(gene, _type);
            _initialized = true;

            OnSpawned?.Invoke(this);
        }

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

        public void Move()
        {
            transform.Translate(Vector3.forward * Time.deltaTime);
        }
    }

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