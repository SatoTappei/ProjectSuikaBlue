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
            /// �ω��ʂ����l��ω�������
            /// </summary>
            /// <returns>�l��1�ȏ�: true �l��0�ȉ�: false</returns>
            public bool Step(float delta) => (Value += delta) > 0;
        }

        readonly StatusBase _base;

        // �p�����[�^
        Param _food;
        Param _water;
        Param _hp;
        Param _lifeSpan;
        Param _breedingRate;
        /// <summary>
        /// 8�r�b�g��؂�̈�`�q(�J���[R �J���[G �J���[B �T�C�Y)
        /// </summary>
        uint _gene;

        public Status(StatusBase statusBase, uint gene = 0)
        {
            _base = statusBase;

            // �S�Ẵp�����[�^�̍ő�l�͑S��ށ��S�̓���
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
    }

}