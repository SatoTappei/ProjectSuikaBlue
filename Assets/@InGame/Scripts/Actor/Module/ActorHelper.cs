using UnityEngine;

namespace PSB.InGame
{
    public static class ActorHelper
    {
        public static bool IsNeighbourOnGrid(Vector2Int a, Vector2Int b)
        {
            int dx = Mathf.Abs(a.x - b.x);
            int dy = Mathf.Abs(a.y - b.y);
            return (dx <= 1) && (dy <= 1);
        }

        /// <summary>
        /// 対象が隣のセルにあるかを調べ、隣にある場合は経路を作成する
        /// </summary>
        /// <returns>経路を作成:true 隣のセルではない:false</returns>
        public static bool CreatePathIfNeighbourOnGrid(Vector2Int from, Vector2Int to, DataContext context)
        {
            if (IsNeighbourOnGrid(from, to) && FieldManager.Instance.IsActorOnCell(from))
            {
                // 隣のセルにある場合は移動しないので、現在地を経路として追加する
                FieldManager.Instance.TryGetCell(from, out Cell cell);
                context.Path.Add(cell.Pos);
                FieldManager.Instance.SetActorOnCell(from, context.Type);
                return true;
            }

            return false;
        }
    }
}
