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
        [Range(0, 100)]
        [SerializeField] float _deltaFood;
        [Range(0, 100)]
        [SerializeField] float _deltaWater;
        [Range(0, 100)]
        [SerializeField] float _deltaHp;
        [Range(0, 100)]
        [SerializeField] float _deltaLifeSpan;
        [Range(0, 100)]
        [SerializeField] float _deltaBreedingRate;
        [Header("最小/最大サイズ")]
        [Range(0.1f, 2.0f)]
        [SerializeField] float _minSize = 0.5f;
        [Range(0.1f, 2.0f)]
        [SerializeField] float _maxSize = 1.5f;

        public ActorType Type => _type;
        public float DeltaFood => _deltaFood;
        public float DeltaWater => _deltaWater;
        public float DeltaHp => _deltaHp;
        public float DeltaLifeSpan => _deltaLifeSpan;
        public float DeltaBreedingRate => _deltaBreedingRate;
        public float MinSize => _minSize;
        public float MaxSize => _maxSize;
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
    }
}