using UnityEngine;

namespace VectorField
{
    /// <summary>
    /// �x�N�^�[�t�B�[���h�̃O���b�h�̊e�Z���̃N���X
    /// </summary>
    public class Cell
    {
        Vector3 _vector;
        byte _cost;
        ushort _calculatedCost;

        public Cell(Vector3 pos, int z, int x)
        {
            Pos = pos;
            Z = z;
            X = x;
        }

        public Vector3 Pos { get; }
        public int Z { get;  }
        public int X { get;  }

        public Vector3 Vector
        { 
            get => _vector; 
            set => _vector = value;
        }
        public byte Cost
        {
            get => _cost;
            // �R�X�g��0����255�̊Ԃɐ�������
            set => _cost = (byte)Mathf.Min(value, byte.MaxValue);
        }
        public ushort CalculatedCost
        {
            get => _calculatedCost;
            // �R�X�g��0����65535�ɐ�������
            set => _calculatedCost = (ushort)Mathf.Min(value, ushort.MaxValue);
        }
    }
}
