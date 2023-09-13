using UnityEngine;

namespace PSB.InGame
{
    [CreateAssetMenu(fileName = "Status_")]
    public class StatusBase : ScriptableObject
    {
        // 全個体の全ての値の最大値は100で固定
        public const float Max = 100;

        [SerializeField] ActorType _type;
        [Header("全個体の全ての値の最大値は100で固定\n変化量(Delta)で個性付けする")]
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
        [Header("最小/最大サイズ")]
        [Range(0.1f, 2.0f)]
        [SerializeField] float _minSize = 0.5f;
        [Range(0.1f, 2.0f)]
        [SerializeField] float _maxSize = 1.5f;
        [Header("繁殖するのに必要な繁殖率の閾値")]
        [SerializeField] float _breedingThreshold = 0.7f;
        [Header("繁殖率が増加するために必要な体力の閾値")]
        [SerializeField] float _breedingHpThreshold = 0.8f;
        [Header("移動速度")]
        [SerializeField] float _moveSpeed = 3.0f;
        [Header("次の行動を決める際の評価関数")]
        [SerializeField] AnimationCurve _breedCurve;
        [SerializeField] AnimationCurve _foodCurve;
        [SerializeField] AnimationCurve _waterCurve;
        [SerializeField] AnimationCurve _wanderCurve;
        [Header("行動を評価する際の補正値")]
        [Tooltip("自身よりサイズが大きい/小さい場合にこの値分、評価値が増減する")]
        [Range(0, 5)]
        [SerializeField] float _sizeEvalFactor = 1.5f;
        [Tooltip("自身より濃い/薄い場合にこの値分、評価値が変化する")]
        [Range(0, 5)]
        [SerializeField] float _colorEvalFactor = 1.5f;
        [Tooltip("体力が高い/低い場合にこの値分、評価値が変化する")]
        [Range(0, 5)]
        [SerializeField] float _hpEvalFactor = 2.0f;
        [Header("行動を評価する際の体力の閾値")]
        [Range(0, 1)]
        [SerializeField] float _attackHpThreshold = 0.75f;
        [Range(0, 1)]
        [SerializeField] float _escapeHpThreshold = 0.33f;
        [Header("視界の設定")]
        [SerializeField] float _sightRadius = 3.0f;
        [SerializeField] LayerMask _sightTargetLayer;
        [Header("攻撃で与えるダメージ")]
        [SerializeField] int _meleeDamage = 10;
        [Header("食事や水分の回復速度")]
        [SerializeField] float _healingRate = 100;
        [Header("交尾にかかる時間")]
        [SerializeField] float _matingTime = 1.0f;
        [Header("敵のタグ")]
        [SerializeField] string _enemyTag;
        [Range(0, 1)]
        [Header("死亡時に増加する集合の評価値")]
        [SerializeField] float _deathGatherScore = 0.1f;
        [Header("リーダー時: 一定間隔で集合させる間隔")]
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
        /// 親が無い場合のデフォルトの遺伝子。カラーが白でサイズがほぼ1になる値
        /// </summary>
        public uint DefaultGene
        {
            get
            {
                // 最大/最小の値によってサイズが1になる値は変わるので
                // リマップの公式を変形したものを用いて求める
                uint size = (uint)((1 - _minSize) * byte.MaxValue / (_maxSize - _minSize));
                // 上位24ビットが色、下位8ビットがサイズの部分
                return 0xFFFFFF00 + size;
            }
        }

        public float BreedEvaluate(float percentage) => _breedCurve.Evaluate(percentage);
        public float FoodEvaluate(float percentage) => _foodCurve.Evaluate(percentage);
        public float WaterEvaluate(float percentage) => _waterCurve.Evaluate(percentage);
        public float WanderEvaluate(float percentage) => _wanderCurve.Evaluate(percentage);
    }
}