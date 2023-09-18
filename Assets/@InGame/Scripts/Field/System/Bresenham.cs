using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public class Bresenham
    {
        Cell[,] _field;

        public Bresenham(Cell[,] field)
        {
            _field = field;
        }

        /// <summary>
        /// AからBへの経路探索を行う。
        /// pathにはセルAの次のセルから、セルBもしくは障害物の1つ手前のセルまでが挿入される。
        /// </summary>
        /// <returns>Bにたどり着いた: true 障害物にぶつかった: false</returns>
        public bool TryGetPath(Vector2Int a, Vector2Int b, out List<Vector2Int> path)
        {
            int deltaX = b.x - a.x;
            int deltaY = b.y - a.y;
            int width = Mathf.Abs(deltaX);
            int height = Mathf.Abs(deltaY);
            int stepX1 = 0 < deltaX ? 1 : deltaX < 0 ? -1 : 0;
            int stepY1 = 0 < deltaY ? 1 : deltaY < 0 ? -1 : 0;

            int longSide = Mathf.Max(width, height);
            int shortSide = Mathf.Min(width, height);

            int stepX2 = 0;
            int stepY2 = 0;
            // 横長の場合x軸が主軸、縦長の場合y軸が主軸
            if (height < width)
            {
                stepX2 = 0 < deltaX ? 1 : deltaX < 0 ? -1 : 0; // 真横の場合は0
            }
            else
            {
                stepY2 = 0 < deltaY ? 1 : deltaY < 0 ? -1 : 0; // 真上/真下の場合は0
            }

            path = new(longSide); // TODO:newしている
            int fraction = longSide / 2; // 本来は除算を使用しないことで高速化を図る
            for (int i = 0; i < longSide; i++)
            {
                fraction += shortSide;
                if (fraction >= longSide)
                {
                    fraction -= longSide;
                    a.x += stepX1;
                    a.y += stepY1;
                }
                else
                {
                    a.x += stepX2;
                    a.y += stepY2;
                }

                if (_field[a.y, a.x].IsWalkable)
                {
                    path.Add(a);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        // デバッグ用
        void DebugVisualize(Vector2Int index)
        {
            var v = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            v.transform.position = _field[index.y, index.x].Pos;
            v.transform.position += Vector3.up;
        }
    }
}
