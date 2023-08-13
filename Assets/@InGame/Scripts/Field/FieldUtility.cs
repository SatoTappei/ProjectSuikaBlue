using UnityEngine;

namespace PSB.InGame
{
    public static class FieldUtility
    {
        // FieldManagerの他に、Field生成時にも配列の内外判定をするので便利クラスに抜き出しておく
        
        public static bool IsWithinGrid(Cell[,] field, int y, int x)
        {
            return 0 <= y && y < field.GetLength(0) && 0 <= x && x < field.GetLength(1);
        }

        public static bool IsWithinGrid(Cell[,] field, Vector2Int index)
        {
            return IsWithinGrid(field, index.y, index.x);
        }
    }
}
