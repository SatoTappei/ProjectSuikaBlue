using UnityEngine;

namespace PSB.InGame
{
    [CreateAssetMenu(fileName = "Status_")]
    public class StatusBase : ScriptableObject
    {
        // �S�̂̑S�Ă̒l�̍ő�l��100�ŌŒ�
        public const float Max = 100;

        [SerializeField] ActorType _type;
        [Header("�S�̂̑S�Ă̒l�̍ő�l��100�ŌŒ�\n�ω���(Delta)�Ō��t������")]
        [Range(0, 20)]
        [SerializeField] float _deltaFood;
        [Range(0, 20)]
        [SerializeField] float _deltaWater;
        [Range(0, 20)]
        [SerializeField] float _deltaHp;
        [Range(0, 20)]
        [SerializeField] float _deltaLifeSpan;
        [Range(0, 20)]
        [SerializeField] float _deltaBreedingRate;
        [Header("�ŏ�/�ő�T�C�Y")]
        [Range(0.1f, 2.0f)]
        [SerializeField] float _minSize = 0.5f;
        [Range(0.1f, 2.0f)]
        [SerializeField] float _maxSize = 1.5f;
        [Header("�ɐB����̂ɕK�v�ȔɐB����臒l")]
        [SerializeField] float _breedingThreshold = 0.7f;
        [Header("�ɐB�����������邽�߂ɕK�v�ȑ̗͂�臒l")]
        [SerializeField] float _breedingHpThreshold = 0.8f;
        [Header("�ړ����x")]
        [SerializeField] float _moveSpeed = 3.0f;
        [Header("���̍s�������߂�ۂ̕]���֐�")]
        [SerializeField] AnimationCurve _breedCurve;
        [SerializeField] AnimationCurve _foodCurve;
        [SerializeField] AnimationCurve _waterCurve;
        [SerializeField] AnimationCurve _wanderCurve;
        [Header("�s����]������ۂ̕␳�l")]
        [Tooltip("���g���T�C�Y���傫��/�������ꍇ�ɂ��̒l���A�]���l����������")]
        [Range(0, 5)]
        [SerializeField] float _sizeEvalFactor = 1.5f;
        [Tooltip("���g���Z��/�����ꍇ�ɂ��̒l���A�]���l���ω�����")]
        [Range(0, 5)]
        [SerializeField] float _colorEvalFactor = 1.5f;
        [Tooltip("�̗͂�����/�Ⴂ�ꍇ�ɂ��̒l���A�]���l���ω�����")]
        [Range(0, 5)]
        [SerializeField] float _hpEvalFactor = 2.0f;
        [Header("�s����]������ۂ̗̑͂�臒l")]
        [Range(0, 1)]
        [SerializeField] float _attackHpThreshold = 0.75f;
        [Range(0, 1)]
        [SerializeField] float _escapeHpThreshold = 0.33f;
        [Header("���E�̐ݒ�")]
        [SerializeField] float _sightRadius = 3.0f;
        [SerializeField] LayerMask _sightTargetLayer;
        [Header("�U���ŗ^����_���[�W")]
        [SerializeField] int _meleeDamage = 10;
        [Header("�H���␅���̉񕜑��x")]
        [SerializeField] float _healingRate = 100;
        [Header("����ɂ����鎞��")]
        [SerializeField] float _matingTime = 1.0f;
        [Header("�G�̃^�O")]
        [SerializeField] string _enemyTag;
        [Range(0, 1)]
        [Header("���S���ɑ�������W���̕]���l")]
        [SerializeField] float _deathGatherScore = 0.1f;
        [Header("���[�_�[��: ���Ԋu�ŏW��������Ԋu")]
        [SerializeField] float _gatherInterval = 10.0f;

        public ActorType Type => _type;
        public float DeltaFood => _deltaFood;
        public float DeltaWater => _deltaWater;
        public float DeltaHp => _deltaHp;
        public float DeltaLifeSpan => _deltaLifeSpan;
        public float DeltaBreedingRate => _deltaBreedingRate;
        public float MinSize => _minSize;
        public float MaxSize => _maxSize;
        public float BreedingThreshold => _breedingThreshold;
        public float BreedingHpThreshold => _breedingHpThreshold;
        public float MoveSpeed => _moveSpeed;
        public float SizeEvalFactor => _sizeEvalFactor;
        public float ColorEvalFactor => _colorEvalFactor;
        public float HpEvalFactor => _hpEvalFactor;
        public float AttackHpThreshold => _attackHpThreshold;
        public float EscapeHpThreshold => _escapeHpThreshold;
        public float SightRadius => _sightRadius;
        public float HealingRate => _healingRate;
        public LayerMask SightTargetLayer => _sightTargetLayer;
        public int MeleeDamage => _meleeDamage;
        public float MatingTime => _matingTime;
        public string EnemyTag => _enemyTag;
        public float DeathGatherScore => _deathGatherScore;
        public float GatherInterval => _gatherInterval;
        /// <summary>
        /// �e�������ꍇ�̃f�t�H���g�̈�`�q�B�J���[�����ŃT�C�Y���ق�1�ɂȂ�l
        /// </summary>
        public uint DefaultGene
        {
            get
            {
                // �ő�/�ŏ��̒l�ɂ���ăT�C�Y��1�ɂȂ�l�͕ς��̂�
                // ���}�b�v�̌�����ό`�������̂�p���ċ��߂�
                uint size = (uint)((1 - _minSize) * byte.MaxValue / (_maxSize - _minSize));
                // ���24�r�b�g���F�A����8�r�b�g���T�C�Y�̕���
                return 0xFFFFFF00 + size;
            }
        }

        public float BreedEvaluate(float percentage) => _breedCurve.Evaluate(percentage);
        public float FoodEvaluate(float percentage) => _foodCurve.Evaluate(percentage);
        public float WaterEvaluate(float percentage) => _waterCurve.Evaluate(percentage);
        public float WanderEvaluate(float percentage) => _wanderCurve.Evaluate(percentage);
    }
}