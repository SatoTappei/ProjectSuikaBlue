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
        /// A����B�ւ̌o�H�T�����s���B
        /// path�ɂ̓Z��A�̎��̃Z������A�Z��B�������͏�Q����1��O�̃Z���܂ł��}�������B
        /// </summary>
        /// <returns>B�ɂ��ǂ蒅����: true ��Q���ɂԂ�����: false</returns>
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
            // �����̏ꍇx�����厲�A�c���̏ꍇy�����厲
            if (height < width)
            {
                stepX2 = 0 < deltaX ? 1 : deltaX < 0 ? -1 : 0; // �^���̏ꍇ��0
            }
            else
            {
                stepY2 = 0 < deltaY ? 1 : deltaY < 0 ? -1 : 0; // �^��/�^���̏ꍇ��0
            }

            path = new(longSide); // TODO:new���Ă���
            int fraction = longSide / 2; // �{���͏��Z���g�p���Ȃ����Ƃō�������}��
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

        // �f�o�b�O�p
        void DebugVisualize(Vector2Int index)
        {
            var v = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            v.transform.position = _field[index.y, index.x].Pos;
            v.transform.position += Vector3.up;
        }
    }
}
