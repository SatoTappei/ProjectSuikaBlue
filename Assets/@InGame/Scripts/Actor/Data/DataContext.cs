using System.Collections.Generic;
using UnityEngine;
using System;

namespace PSB.InGame
{
    public class DataContext : MonoBehaviour
    {
        [SerializeField] ActorType _type;
        [SerializeField] Transform _model;
        [Header("�M�Y���ւ̕`����s��")]
        [SerializeField] bool _isDrawGizmos;

        Transform _transform;
        StatusBase _base;
        Dictionary<ActionType, BaseState> _stateDict;
        // �]���͑Ή�����s�����������ʂȏ�ԂȂ̂ŕʓr�ێ�����
        EvaluateState _evaluateState;
        // Actor�������������State���œǂݎ��l
        DataContext _enemy;
        Transform _leader;
        ActionType _nextAction;
        // �p�����[�^
        Param _food;
        Param _water;
        Param _hp;
        Param _lifeSpan;
        Param _breedingRate;
        // 8�r�b�g��؂�̈�`�q(�J���[R �J���[G �J���[B �T�C�Y)
        uint _gene;
        // �������ς݃t���O
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
                // ��`�q�̂����T�C�Y�ɓK�p����8�r�b�g(0~255)�݂̂����o���ĕϊ�����
                float f = Gene & 0xFF;
                // f���ŏ�/�ő�T�C�Y�͈̔͂Ƀ��}�b�v
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
                    throw new KeyNotFoundException("�J�ڐ�̃X�e�[�g�����݂��Ȃ�: " + NextAction);
                }
            }
        }
        /// <summary>
        /// �ɐB�\���ǂ���
        /// </summary>
        public bool BreedingReady => BreedingRate.Percentage >= Base.BreedingThreshold;
        /// <summary>
        /// �ɐB�����������邩�ǂ���
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
        /// �����������B��`�q��n�������Ȃ̂ŃX�|�i�[��Actor�ǂ��炪�Ă�ł�����ɓ��삷��B
        /// �L�����N�^�[���̂̃v�[�����O�ł̉^�p���s���A�v�[��������o�����x�ɌĂ΂��z��
        /// </summary>
        public void Init(uint? gene)
        {
            _transform = transform;

            _base ??= StatusBaseHolder.Get(_type);
            _gene = gene ?? Base.DefaultGene;
            // �S�Ẵp�����[�^�̍ő�l�͑S��ށ��S�̓���
            Food = new Param(StatusBase.Max);
            Water = new Param(StatusBase.Max);
            HP = new Param(StatusBase.Max);
            LifeSpan = new Param(StatusBase.Max);
            // �ɐB�������͑������Ă����̂�0�ŏ�����
            BreedingRate = new Param(0);

            if (!_initialized) CreateState();

            // �ŏ���1��A�Ăяo�����s����Ə���������
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
                string msg = "Init���\�b�h���Ă΂�Ă��炸�A�������������������Ă��Ȃ��B";
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
        public void StepBreedingRate() => _breedingRate.Value += Base.DeltaBreedingRate * Time.deltaTime; // �����Z

        // �ȉ��f�o�b�O�p
        public void Log()
        {
            Debug.Log($"�H:{Food.Value} ��:{Water.Value} ��:{HP.Value} " +
                $"��:{LifeSpan.Value} ��{BreedingRate.Value}");
        }

        public void Log2()
        {
            Debug.Log($"�H:{Food.Percentage} ��:{Water.Percentage} ��:{HP.Percentage} " +
                $"��:{LifeSpan.Percentage} ��{BreedingRate.Percentage}");
        }

        public void GeneLog()
        {
            Debug.Log($"�F:{ColorR},{ColorG},{ColorB} �T�C�Y:{Size}");
        }

        void DrawSightRadius()
        {
            Gizmos.DrawWireSphere(transform.position, _base.SightRadius);
        }
    }
}
