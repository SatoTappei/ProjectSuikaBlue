using UnityEngine;

namespace Old
{
    /// <summary>
    /// ベクターフィールドのグリッドの各セルのクラス
    /// このクラスの二次元配列をグリッドとして扱う
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
            // コストを0から255の間に制限する
            set => _cost = (byte)Mathf.Min(value, byte.MaxValue);
        }
        public ushort CalculatedCost
        {
            get => _calculatedCost;
            // コストを0から65535に制限する
            set => _calculatedCost = (ushort)Mathf.Min(value, ushort.MaxValue);
        }
    }
}
