using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PSB.InGame
{
    public class DataContext : MonoBehaviour, IDamageReceiver
    {
        // 次の性別を交互に設定するための変数
        static Sex NextSex;

        [SerializeField] ActorType _type;
        [SerializeField] Transform _model;
        [Header("ギズモへの描画を行う")]
        [SerializeField] bool _isDrawGizmos;

        // プールに返却する処理。生成時にプールから登録される
        [HideInInspector] public UnityAction ReturnToPool;
        // Actor側が書き込んでState側で読み取る値
        [HideInInspector] public List<Vector3> Path = new();
        [HideInInspector] public DataContext Enemy;
        [HideInInspector] public Transform Leader;
        [HideInInspector] public ActionType NextAction;
        // パラメータ
        [HideInInspector] public Param Food;
        [HideInInspector] public Param Water;
        [HideInInspector] public Param HP;
        [HideInInspector] public Param LifeSpan;
        [HideInInspector] public Param BreedingRate;

        Transform _transform;
        StatusBase _base;
        Dictionary<ActionType, BaseState> _stateDict;
        Sex _sex;
        // 繁殖ステート
        MaleBreedState _maleBreedState;
        FemaleBreedState _femaleBreedState;
        // 評価は対応する行動が無い特別な状態なので別途保持する
        EvaluateState _evaluateState;
        // 8ビット区切りの遺伝子(カラーR カラーG カラーB サイズ)
        uint _gene;
        // 初期化済みフラグ
        bool _initialized;

        public ActorType Type => _type;
        public Transform Model => _model;
        public EvaluateState EvaluateState => _evaluateState;
        public Transform Transform => _transform;
        public StatusBase Base => _base;
        public Sex Sex => _sex;
        public string EnemyTag => _type == ActorType.Kurokami ? "Kinpatsu" : "Kurokami";
        public uint Gene => _gene;
        public byte ColorR => (byte)(Gene >> 24 & 0xFF);
        public byte ColorG => (byte)(Gene >> 16 & 0xFF);
        public byte ColorB => (byte)(Gene >> 8 & 0xFF);
        public Color32 Color => new Color32(ColorR, ColorG, ColorB, 255);
        public bool IsEnemyDetected => Enemy != null;

        public float Size
        {
            get
            {
                // 遺伝子のうちサイズに適用する8ビット(0~255)のみを取り出して変換する
                float f = Gene & 0xFF;
                // fを最小/最大サイズの範囲にリマップ
                return (f - 0) * (Base.MaxSize - Base.MinSize) / (byte.MaxValue - byte.MinValue) + Base.MinSize;
            }
        }
        public BaseState NextState
        {
            get
            {
                if (_stateDict.ContainsKey(NextAction))
                {
                    return _stateDict[NextAction];
                }
                else
                {
                    throw new KeyNotFoundException("遷移先のステートが存在しない: " + NextAction);
                }
            }
        }
        /// <summary>
        /// 繁殖可能かどうか
        /// </summary>
        public bool BreedingReady => BreedingRate.Percentage >= Base.BreedingThreshold;
        /// <summary>
        /// 繁殖率が増加するかどうか
        /// </summary>
        public bool IsBreedingRateIncrease => HP.Percentage >= Base.BreedingHpThreshold;

        /// <summary>
        /// 初期化処理。遺伝子を渡すだけなのでスポナーとActorどちらが呼んでも正常に動作する。
        /// キャラクター自体のプーリングでの運用を行い、プールから取り出される度に呼ばれる想定
        /// </summary>
        public void Init(uint? gene)
        {
            _transform ??= transform;
            // 行動の評価の処理がされない場合はうろうろステートに遷移する
            NextAction = ActionType.Wander;

            _base ??= StatusBaseHolder.Get(_type);
            _gene = gene ?? Base.DefaultGene;
            // 性別を交互に設定する
            _sex = NextSex;
            NextSex = 1 - NextSex;
            // 全てのパラメータの最大値は全種類＆全個体同じ
            Food = new Param(StatusBase.Max);
            Water = new Param(StatusBase.Max);
            HP = new Param(StatusBase.Max);
            LifeSpan = new Param(StatusBase.Max);
            // 繁殖率だけは増加していくので0で初期化
            BreedingRate = new Param(0);

            if (!_initialized) CreateState();
            AddBreedState();

            // 最初の1回、呼び出しが行われると初期化完了
            _initialized = true;
        }

        void Start()
        {
            CheckInitialized();
        }

        void OnDrawGizmos()
        {
            if (_isDrawGizmos)
            {
                DrawSightRadius();
            }
        }

        void CheckInitialized()
        {
            if (!_initialized)
            {
                string msg = "Initメソッドが呼ばれておらず、初期化処理が完了していない。";
                new System.InvalidOperationException(msg);
            }
        }

        void CreateState()
        {
            _evaluateState = new(this);

            _stateDict = new(Utility.GetEnumLength<ActionType>());
            _stateDict.Add(ActionType.Killed, new KilledState(this));
            _stateDict.Add(ActionType.Senility, new SenilityState(this));
            _stateDict.Add(ActionType.Attack, new AttackState(this));
            _stateDict.Add(ActionType.Escape, new EscapeState(this));
            _stateDict.Add(ActionType.Gather, new GatherState(this));
            //_stateDict.Add(ActionType.Breed, new MaleBreedState(this));
            _stateDict.Add(ActionType.SearchFood, new SearchFoodState(this));
            _stateDict.Add(ActionType.SearchWater, new SearchWaterState(this));
            _stateDict.Add(ActionType.Wander, new WanderState(this));
            _stateDict.Add(ActionType.None, new IdleState(this));
            // 繁殖ステートは性別によって変わるのでプールから取り出す度にどちらかを追加する
            _maleBreedState = new(this);
            _femaleBreedState = new(this);
        }

        /// <summary>
        /// 雄と雌で繁殖時の行動が違うので、プールから取り出して初期化する際に
        /// 繁殖ステートのみ辞書から削除、再度性別ごとに追加する
        /// </summary>
        void AddBreedState()
        {
            _stateDict.Remove(ActionType.Breed);
            _stateDict.Add(ActionType.Breed, _sex == Sex.Male ? _maleBreedState : _femaleBreedState);
        }

        public void StepFood()         => Food.Value         -= Base.DeltaFood         * Time.deltaTime;
        public void StepWater()        => Water.Value        -= Base.DeltaWater        * Time.deltaTime;
        public void StepHp()           => HP.Value           -= Base.DeltaHp           * Time.deltaTime;
        public void StepLifeSpan()     => LifeSpan.Value     -= Base.DeltaLifeSpan     * Time.deltaTime;    
        public void StepBreedingRate() => BreedingRate.Value += Base.DeltaBreedingRate * Time.deltaTime; // 足し算

        public void Damage(int value) => HP.Value -= value;
        public bool ShouldChangeState(BaseState state) => NextState != state;

        // 以下デバッグ用
        public void Log()
        {
            Debug.Log($"食:{Food.Value} 水:{Water.Value} 体:{HP.Value} " +
                $"寿:{LifeSpan.Value} 繁{BreedingRate.Value}");
        }

        public void Log2()
        {
            Debug.Log($"食:{Food.Percentage} 水:{Water.Percentage} 体:{HP.Percentage} " +
                $"寿:{LifeSpan.Percentage} 繁{BreedingRate.Percentage}");
        }

        public void GeneLog()
        {
            Debug.Log($"色:{ColorR},{ColorG},{ColorB} サイズ:{Size}");
        }

        void DrawSightRadius()
        {
            Gizmos.DrawWireSphere(transform.position, _base.SightRadius);
        }
    }
}
