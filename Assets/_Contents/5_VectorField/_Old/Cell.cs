using UnityEngine;

namespace Old
{
    /// <summary>
    /// �x�N�^�[�t�B�[���h�̃O���b�h�̊e�Z���̃N���X
    /// ���̃N���X�̓񎟌��z����O���b�h�Ƃ��Ĉ���
    /// </summary>
    public class Cell
    {
        Vector3 _pos;
        Vector3 _vector;
        int _z;
        int _x;
        byte _cost;
        ushort _calculatedCost;

        public Cell(Vector3 pos, int z, int x)
        {
            _pos = pos;
            _z = z;
            _x = x;
        }

        public Vector3 Pos => _pos;
        public int Z => _z;
        public int X => _x;

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
