using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PSB.InGame
{
    public class DataContext : MonoBehaviour, IDamageReceiver
    {
        // ���̐��ʂ����݂ɐݒ肷�邽�߂̕ϐ�
        static Sex NextSex;

        [SerializeField] ActorType _type;
        [SerializeField] Transform _model;
        [Header("�M�Y���ւ̕`����s��")]
        [SerializeField] bool _isDrawGizmos;

        // �v�[���ɕԋp���鏈���B�������Ƀv�[������o�^�����
        [HideInInspector] public UnityAction ReturnToPool;
        // Actor�������������State���œǂݎ��l
        [HideInInspector] public List<Vector3> Path = new();
        [HideInInspector] public DataContext Enemy;
        [HideInInspector] public Transform Leader;
        [HideInInspector] public ActionType NextAction;
        // �p�����[�^
        [HideInInspector] public Param Food;
        [HideInInspector] public Param Water;
        [HideInInspector] public Param HP;
        [HideInInspector] public Param LifeSpan;
        [HideInInspector] public Param BreedingRate;

        Transform _transform;
        StatusBase _base;
        Dictionary<ActionType, BaseState> _stateDict;
        Sex _sex;
        // �ɐB�X�e�[�g
        MaleBreedState _maleBreedState;
        FemaleBreedState _femaleBreedState;
        // �]���͑Ή�����s�����������ʂȏ�ԂȂ̂ŕʓr�ێ�����
        EvaluateState _evaluateState;
        // 8�r�b�g��؂�̈�`�q(�J���[R �J���[G �J���[B �T�C�Y)
        uint _gene;
        // �������ς݃t���O
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

        /// <summary>
        /// �����������B��`�q��n�������Ȃ̂ŃX�|�i�[��Actor�ǂ��炪�Ă�ł�����ɓ��삷��B
        /// �L�����N�^�[���̂̃v�[�����O�ł̉^�p���s���A�v�[��������o�����x�ɌĂ΂��z��
        /// </summary>
        public void Init(uint? gene)
        {
            _transform ??= transform;
            // �s���̕]���̏���������Ȃ��ꍇ�͂��낤��X�e�[�g�ɑJ�ڂ���
            NextAction = ActionType.Wander;

            _base ??= StatusBaseHolder.Get(_type);
            _gene = gene ?? Base.DefaultGene;
            // ���ʂ����݂ɐݒ肷��
            _sex = NextSex;
            NextSex = 1 - NextSex;
            // �S�Ẵp�����[�^�̍ő�l�͑S��ށ��S�̓���
            Food = new Param(StatusBase.Max);
            Water = new Param(StatusBase.Max);
            HP = new Param(StatusBase.Max);
            LifeSpan = new Param(StatusBase.Max);
            // �ɐB�������͑������Ă����̂�0�ŏ�����
            BreedingRate = new Param(0);

            if (!_initialized) CreateState();
            AddBreedState();

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
            //_stateDict.Add(ActionType.Breed, new MaleBreedState(this));
            _stateDict.Add(ActionType.SearchFood, new SearchFoodState(this));
            _stateDict.Add(ActionType.SearchWater, new SearchWaterState(this));
            _stateDict.Add(ActionType.Wander, new WanderState(this));
            _stateDict.Add(ActionType.None, new IdleState(this));
            // �ɐB�X�e�[�g�͐��ʂɂ���ĕς��̂Ńv�[��������o���x�ɂǂ��炩��ǉ�����
            _maleBreedState = new(this);
            _femaleBreedState = new(this);
        }

        /// <summary>
        /// �Y�Ǝ��ŔɐB���̍s�����Ⴄ�̂ŁA�v�[��������o���ď���������ۂ�
        /// �ɐB�X�e�[�g�̂ݎ�������폜�A�ēx���ʂ��Ƃɒǉ�����
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
        public void StepBreedingRate() => BreedingRate.Value += Base.DeltaBreedingRate * Time.deltaTime; // �����Z

        public void Damage(int value) => HP.Value -= value;
        public bool ShouldChangeState(BaseState state) => NextState != state;

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
