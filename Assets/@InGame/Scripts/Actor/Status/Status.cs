using UnityEngine;

namespace PSB.InGame
{
    public class Status
    {
        public struct Param
        {
            public Param(float max)
            {
                Value = max;
            }

            public float Value;

            public float Max => StatusBase.Max;
            public float Percentage => Value / StatusBase.Max;
            public bool IsBelowZero => Value < 0;

            /// <summary>
            /// 変化量だけ値を変化させる
            /// </summary>
            /// <returns>値が1以上: true 値が0以下: false</returns>
            public bool Step(float delta) => (Value += delta) > 0;
        }

        readonly StatusBase _base;

        // パラメータ
        Param _food;
        Param _water;
        Param _hp;
        Param _lifeSpan;
        Param _breedingRate;
        /// <summary>
        /// 8ビット区切りの遺伝子(カラーR カラーG カラーB サイズ)
        /// </summary>
        uint _gene;

        public Status(StatusBase statusBase, uint gene = 0)
        {
            _base = statusBase;

            // 全てのパラメータの最大値は全種類＆全個体同じ
            _food = new Param(StatusBase.Max);
            _water = new Param(StatusBase.Max);
            _hp = new Param(StatusBase.Max);
            _lifeSpan = new Param(StatusBase.Max);
            _breedingRate = new Param(StatusBase.Max);

            _gene = gene;
        }

        public float Food { get => _food.Value; set => _food.Value = value; }
        public float Water { get => _water.Value; set => _water.Value = value; }
        public float Hp { get => _hp.Value; set => _hp.Value = value; }
        public float LifeSpan { get => _lifeSpan.Value; set => _lifeSpan.Value = value; }
        public float BreedingRate { get => _breedingRate.Value; set => _breedingRate.Value = value; }
        public byte ColorR => (byte)(_gene >> 24 & 0xFF);
        public byte ColorG => (byte)(_gene >> 16 & 0xFF);
        public byte ColorB => (byte)(_gene >> 8 & 0xFF);
        public Color32 Color => new Color32(ColorR, ColorG, ColorB, 255);
        public float Size => GeneToSize();

        /// <summary>
        /// 遺伝子のうちサイズに適用する8ビットのみを取り出して変換する
        /// </summary>
        /// <returns>LocalScale</returns>
        float GeneToSize()
        {
            // 0~255
            float f = _gene & 0xFF;
            // fを最小/最大サイズの範囲にリマップ
            return (f - 0) * (_base.MaxSize - _base.MinSize) / (byte.MaxValue - byte.MinValue) + _base.MinSize;
        }
    }

}