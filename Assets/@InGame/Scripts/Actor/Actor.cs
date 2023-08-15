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

    [RequireComponent(typeof(ChildSpawner))]
    public class Actor : MonoBehaviour, IReadOnlyParams
    {
        public static event UnityAction<Actor> OnSpawned;

        // インスペクタで割り当てない
        // 基準となる値 + 親から遺伝した値
        // 生成自体はスポナーが行う。その際に遺伝した値を渡したい。
        // 遺伝した値はランダムなのでSOやインスペクタから渡さない。

        [SerializeField] ChildSpawner _spawner;
        // StatusBaseの取得やController側での制御に必要なので個体毎にデータを持つ
        [SerializeField] ActorType _type;
        [Header("3Dモデルのメッシュのオブジェクト")]
        [SerializeField] Transform _model;

        Status _status;
        bool _initialized;
        
        public ActorType Type => _type;
        // UI側が読み取る用
        float IReadOnlyParams.Food => /*_food.Percentage*/1;
        float IReadOnlyParams.Water => /*_water.Percentage*/1;
        float IReadOnlyParams.HP => /*_hp.Percentage*/1;

        /// <summary>
        /// ステータスの設定、Controller側で制御するためのコールバックの呼び出し
        /// 遺伝子がnullの場合は親無しなのでデフォルトの遺伝子を使用する
        /// </summary>
        public void Init(uint? gene = null) 
        {
            // 種類と遺伝子からステータスを初期化
            StatusBase statusBase = StatusBaseHolder.Get(_type);
            gene ??= statusBase.DefaultGene;
            _status = new(statusBase, (uint)gene);
            
            // 遺伝子を個体に反映
            ApplyInheritedSize(_status.Size);
            ApplyInheritedColor(_status.Color);

            OnSpawned?.Invoke(this);

            // もう一度このメソッドを呼んで初期化しないようにフラグを立てる
            _initialized = true;
        }

        void Start()
        {
            // 直接シーンに配置した場合などスポナーを経由しない場合用
            // 外部からデータを取得するのでStartのタイミングで呼ぶ必要がある
            if (!_initialized) Init();
        }

        void ApplyInheritedSize(float size)
        {
            Debug.Log(size);
            _model.transform.localScale *= size;
        }

        void ApplyInheritedColor(Color32 color)
        {
            Debug.Log(color);
        }

        public void Move()
        {
            transform.Translate(Vector3.forward * Time.deltaTime);
        }
    }

    // ★:現在の問題点
    //    キャラクターはスポナーから生成される。生成したタイミングでキャラクターのInitメソッドを呼ぶ。
    //    生成されたキャラクターはStateBaseをGameManagerから取得してくる。
    
    //    以下が現在の問題点
    //    ・自身のStatusBaseを取得するタイミングがStart()なので、Initとは別タイミングになってしまう。
    //    ・キャラクターはstaticでGameManagerの参照を持っているのでいずれかのキャラをDestroyするとnullが代入され、全体でぬるりが出る。
    //    ・遺伝子の引継ぎについて。スポナーから生成しているので遺伝子をスポナーに渡す->その遺伝子をInitでキャラクターに渡すしかない。

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