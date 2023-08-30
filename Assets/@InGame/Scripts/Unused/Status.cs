using UnityEngine;

namespace PSB.InGame
{
    public class Status
    {
        public struct Param
        {
            float _value;

            public Param(float value)
            {
                _value = value;
            }

            public float Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value;
                    _value = Mathf.Clamp(_value, 0, StatusBase.Max);
                }
            }

            public float Max => StatusBase.Max;
            public float Percentage => Value / StatusBase.Max;
            public bool IsBelowZero => Value <= 0;
        }

        // 繁殖するのに必要な繁殖率の閾値
        const float BreedingThreshold = 0.7f;
        // 繁殖率が増加するために必要な閾値
        const float BreedingHpThreshold = 0.8f;

        readonly StatusBase _base;

        /// <summary>
        /// 8ビット区切りの遺伝子(カラーR カラーG カラーB サイズ)
        /// </summary>
        readonly uint _gene;

        public Status(uint? gene = 0)
        {
            //_base = statusBase;

            // 全てのパラメータの最大値は全種類＆全個体同じ
            Food = new Param(StatusBase.Max);
            Water = new Param(StatusBase.Max);
            Hp = new Param(StatusBase.Max);
            LifeSpan = new Param(StatusBase.Max);
            // 繁殖率だけは増加していくので0で初期化
            BreedingRate = new Param(0);

           // _gene = gene;
        }

        // パラメータ
        public Param Food;
        public Param Water;
        public Param Hp;
        public Param LifeSpan;
        public Param BreedingRate;
        // 遺伝子
        public uint Gene => _gene;
        public byte ColorR => (byte)(_gene >> 24 & 0xFF);
        public byte ColorG => (byte)(_gene >> 16 & 0xFF);
        public byte ColorB => (byte)(_gene >> 8 & 0xFF);
        public Color32 Color => new Color32(ColorR, ColorG, ColorB, 255);
        public float Size => GeneToSize();
        /// <summary>
        /// 繁殖可能かどうか
        /// </summary>
        public bool BreedingReady => BreedingRate.Percentage >= BreedingThreshold;
        /// <summary>
        /// 繁殖率が増加するかどうか
        /// </summary>
        public bool IsBreedingRateIncrease => Hp.Percentage >= BreedingHpThreshold;

        public void StepFood() => Food.Value -= _base.DeltaFood * Time.deltaTime;
        public void StepWater()=> Water.Value -= _base.DeltaWater * Time.deltaTime;
        public void StepHp() => Hp.Value -= _base.DeltaHp * Time.deltaTime;
        public void StepLifeSpan() => LifeSpan.Value -= _base.DeltaLifeSpan * Time.deltaTime;
        // ↓足し算
        public void StepBreedingRate() => BreedingRate.Value += _base.DeltaBreedingRate * Time.deltaTime;

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

        // デバッグ用
        public void Log()
        {
            Debug.Log($"食:{Food.Value} 水:{Water.Value} 体:{Hp.Value} " +
                $"寿:{LifeSpan.Value} 繁{BreedingRate.Value}");
        }
        
        public void Log2()
        {
            Debug.Log($"食:{Food.Percentage} 水:{Water.Percentage} 体:{Hp.Percentage} " +
                $"寿:{LifeSpan.Percentage} 繁{BreedingRate.Percentage}");
        }

        public void GeneLog()
        {
            Debug.Log($"色:{ColorR},{ColorG},{ColorB} サイズ:{Size}");
        }
    }
}