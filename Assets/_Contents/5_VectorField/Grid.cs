using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VectorField
{
    /// <summary>
    /// ベクターフィールドのグリッドのクラス
    /// [縦,横]で扱う
    /// </summary>
    public class Grid
    {
        Cell[,] _grid;
        Vector3 _centerPos;
        float _cellSize;
        int _width;
        int _height;

        public Grid(int width, int height, float cellSize, Vector3 centerPos)
        {
            _grid = new Cell[height, width];
            _centerPos = centerPos;
            _cellSize = cellSize;
            _width = width;
            _height = height;
        }

        public void DrawOnGizmos()
        {
            if (_grid == null) return;

            Gizmos.color = Color.green;
            for(int i = 0; i < _height; i++)
            {
                for(int k = 0; k < _width; k++)
                {
                    // 辺の大きさが偶数の場合はセルの中心に合わせるオフセットが必要
                    float offsetZ = _height % 2 == 0 ? 0.5f : 0;
                    float offsetX = _width % 2 == 0 ? 0.5f : 0;

                    float z = (_centerPos.z + i - _height / 2 + offsetZ) * _cellSize;
                    float x = (_centerPos.x + k - _width / 2 + offsetX) * _cellSize;
                    Vector3 pos = new Vector3(x, _centerPos.y, z);
                    Gizmos.DrawWireCube(pos, Vector3.one * _cellSize);
                }
            }
        }
    }
}