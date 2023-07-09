using UnityEngine;

namespace VectorField
{
    /// <summary>
    /// ベクターフィールドのグリッドの各セルのクラス
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
