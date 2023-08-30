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

        // �ɐB����̂ɕK�v�ȔɐB����臒l
        const float BreedingThreshold = 0.7f;
        // �ɐB�����������邽�߂ɕK�v��臒l
        const float BreedingHpThreshold = 0.8f;

        readonly StatusBase _base;

        /// <summary>
        /// 8�r�b�g��؂�̈�`�q(�J���[R �J���[G �J���[B �T�C�Y)
        /// </summary>
        readonly uint _gene;

        public Status(uint? gene = 0)
        {
            //_base = statusBase;

            // �S�Ẵp�����[�^�̍ő�l�͑S��ށ��S�̓���
            Food = new Param(StatusBase.Max);
            Water = new Param(StatusBase.Max);
            Hp = new Param(StatusBase.Max);
            LifeSpan = new Param(StatusBase.Max);
            // �ɐB�������͑������Ă����̂�0�ŏ�����
            BreedingRate = new Param(0);

           // _gene = gene;
        }

        // �p�����[�^
        public Param Food;
        public Param Water;
        public Param Hp;
        public Param LifeSpan;
        public Param BreedingRate;
        // ��`�q
        public uint Gene => _gene;
        public byte ColorR => (byte)(_gene >> 24 & 0xFF);
        public byte ColorG => (byte)(_gene >> 16 & 0xFF);
        public byte ColorB => (byte)(_gene >> 8 & 0xFF);
        public Color32 Color => new Color32(ColorR, ColorG, ColorB, 255);
        public float Size => GeneToSize();
        /// <summary>
        /// �ɐB�\���ǂ���
        /// </summary>
        public bool BreedingReady => BreedingRate.Percentage >= BreedingThreshold;
        /// <summary>
        /// �ɐB�����������邩�ǂ���
        /// </summary>
        public bool IsBreedingRateIncrease => Hp.Percentage >= BreedingHpThreshold;

        public void StepFood() => Food.Value -= _base.DeltaFood * Time.deltaTime;
        public void StepWater()=> Water.Value -= _base.DeltaWater * Time.deltaTime;
        public void StepHp() => Hp.Value -= _base.DeltaHp * Time.deltaTime;
        public void StepLifeSpan() => LifeSpan.Value -= _base.DeltaLifeSpan * Time.deltaTime;
        // �������Z
        public void StepBreedingRate() => BreedingRate.Value += _base.DeltaBreedingRate * Time.deltaTime;

        /// <summary>
        /// ��`�q�̂����T�C�Y�ɓK�p����8�r�b�g�݂̂����o���ĕϊ�����
        /// </summary>
        /// <returns>LocalScale</returns>
        float GeneToSize()
        {
            // 0~255
            float f = _gene & 0xFF;
            // f���ŏ�/�ő�T�C�Y�͈̔͂Ƀ��}�b�v
            return (f - 0) * (_base.MaxSize - _base.MinSize) / (byte.MaxValue - byte.MinValue) + _base.MinSize;
        }

        // �f�o�b�O�p
        public void Log()
        {
            Debug.Log($"�H:{Food.Value} ��:{Water.Value} ��:{Hp.Value} " +
                $"��:{LifeSpan.Value} ��{BreedingRate.Value}");
        }
        
        public void Log2()
        {
            Debug.Log($"�H:{Food.Percentage} ��:{Water.Percentage} ��:{Hp.Percentage} " +
                $"��:{LifeSpan.Percentage} ��{BreedingRate.Percentage}");
        }

        public void GeneLog()
        {
            Debug.Log($"�F:{ColorR},{ColorG},{ColorB} �T�C�Y:{Size}");
        }
    }
}