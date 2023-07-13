using UnityEngine;

namespace VectorField
{
    /// <summary>
    /// ベクトルフィールドのグリッドで使用する共通処理を抜き出したクラス
    /// </summary>
    public static class GridUtility
    {
        /// <summary>
        /// ワールド座標に対応したグリッドの添え字を返す
        /// </summary>
        public static Vector2Int WorldPosToGridIndex(Vector3 targetPos, Cell[,] grid, GridData data)
        {
            // グリッドの1辺の長さ
            float forwardZ = grid[0, 0].Pos.z;
            float backZ = grid[data.Height - 1, data.Width - 1].Pos.z;
            float leftX = grid[0, 0].Pos.x;
            float rightX = grid[data.Height - 1, data.Width - 1].Pos.x;
            // グリッドの端から座標までの長さ
            float lengthZ = backZ - forwardZ;
            float lengthX = rightX - leftX;
            // グリッドの端から何％の位置か
            float fromPosZ = targetPos.z - forwardZ;
            float fromPosX = targetPos.x - leftX;
            // グリッドの端から何％の位置か
            float percentZ = Mathf.Abs(fromPosZ / lengthZ);
            float percentX = Mathf.Abs(fromPosX / lengthX);

            // xはそのまま、yはzに対応している
            Vector2Int index = new Vector2Int()
            {
                x = Mathf.RoundToInt((data.Width - 1) * percentX),
                y = Mathf.RoundToInt((data.Height - 1) * percentZ),
            };

            return index;
        }
    }
}
