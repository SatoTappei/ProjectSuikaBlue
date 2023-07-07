using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VectorField
{
    /// <summary>
    /// ベクターフィールドのグリッドの各セルのクラス
    /// </summary>
    public class Cell
    {
        int _cellSize;

        public void DrawOnGizmos(Vector3 pos)
        {
            Vector3 size = new Vector3(_cellSize, 1, _cellSize);
            Gizmos.DrawWireCube(pos, size);
        }
    }
}
