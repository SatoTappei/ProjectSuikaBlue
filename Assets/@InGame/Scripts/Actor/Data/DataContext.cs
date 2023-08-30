using System.Collections.Generic;
using UnityEngine;
using System;

namespace PSB.InGame
{
    public class DataContext : MonoBehaviour
    {
        [SerializeField] ActorType _type;
        [SerializeField] Transform _model;
        [Header("ギズモへの描画を行う")]
        [SerializeField] bool _isDrawGizmos;

        Transform _transform;
        StatusBase _base;
        Dictionary<ActionType, BaseState> _stateDict;
        // 評価は対応する行動が無い特別な状態なので別途保持する
        EvaluateState _evaluateState;
        // Actor側が書き込んでState側で読み取る値
        DataContext _enemy;
        Transform _leader;
        ActionType _nextAction;
        // パラメータ
        Param _food;
        Param _water;
        Param _hp;
        Param _lifeSpan;
        Param _breedingRate;
        // 8ビット区切りの遺伝子(カラーR カラーG カラーB サイズ)
        uint _gene;
        // 初期化済みフラグ
        bool _initialized;

        public ActorType Type => _type;
        public Transform Model => _model;
        public EvaluateState EvaluateState => _evaluateState;
        public Transform Transform => _transform;
        public StatusBase Base => _base;
        public string EnemyTag => _type == ActorType.Kurokami ? "Kinpatsu" : "Kurokami";
        public uint Gene => _gene;
        public byte ColorR => (byte)(Gene >> 24 & 0xFF);
        public byte ColorG => (byte)(Gene >> 16 & 0xFF);
        public byte ColorB => (byte)(Gene >> 8 & 0xFF);
        public Color32 Color => new Color32(ColorR, ColorG, ColorB, 255);
        public bool IsEnemyDetected => _enemy != null;

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

        public DataContext Enemy     { get => _enemy;        set => _enemy = value; }
        public Transform Leader      { get => _leader;       set => _leader = value; }
        public ActionType NextAction { get => _nextAction;   set => _nextAction = value; }
        public Param Food            { get => _food;         set => _food = value; }
        public Param Water           { get => _water;        set => _water = value; }
        public Param HP              { get => _hp;           set => _hp = value; }
        public Param LifeSpan        { get => _lifeSpan;     set => _lifeSpan = value; }
        public Param BreedingRate    { get => _breedingRate; set => _breedingRate = value; }

        /// <summary>
        /// 初期化処理。遺伝子を渡すだけなのでスポナーとActorどちらが呼んでも正常に動作する。
        /// キャラクター自体のプーリングでの運用を行い、プールから取り出される度に呼ばれる想定
        /// </summary>
        public void Init(uint? gene)
        {
            _transform = transform;

            _base ??= StatusBaseHolder.Get(_type);
            _gene = gene ?? Base.DefaultGene;
            // 全てのパラメータの最大値は全種類＆全個体同じ
            Food = new Param(StatusBase.Max);
            Water = new Param(StatusBase.Max);
            HP = new Param(StatusBase.Max);
            LifeSpan = new Param(StatusBase.Max);
            // 繁殖率だけは増加していくので0で初期化
            BreedingRate = new Param(0);

            if (!_initialized) CreateState();

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
            _stateDict.Add(ActionType.Breed, new BreedState(this));
            _stateDict.Add(ActionType.SearchFood, new SearchFoodState(this));
            _stateDict.Add(ActionType.SearchWater, new SearchWaterState(this));
            _stateDict.Add(ActionType.Wander, new WanderState(this));
            _stateDict.Add(ActionType.None, new IdleState(this));
        }

        public void StepFood()         => _food.Value         -= Base.DeltaFood         * Time.deltaTime;
        public void StepWater()        => _water.Value        -= Base.DeltaWater        * Time.deltaTime;
        public void StepHp()           => _hp.Value           -= Base.DeltaHp           * Time.deltaTime;
        public void StepLifeSpan()     => _lifeSpan.Value     -= Base.DeltaLifeSpan     * Time.deltaTime;    
        public void StepBreedingRate() => _breedingRate.Value += Base.DeltaBreedingRate * Time.deltaTime; // 足し算

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
