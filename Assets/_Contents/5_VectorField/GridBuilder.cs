using UnityEngine;

namespace VectorField
{
    /// <summary>
    /// VectorFieldManagerクラスが持つGridDataを元に、グリッドと各セルの生成
    /// 及びセルの初期コストを設定して返す処理を行うクラス
    /// </summary>
    public class GridBuilder
    {
        public Cell[,] CreateGrid(GridData data)
        {
            // [高さ,幅]
            Cell[,] grid = new Cell[data.Height, data.Width];
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int k = 0; k < grid.GetLength(1); k++)
                {
                    CreateCell(grid, data, i, k);
                }
            }

            return grid;
        }

        void CreateCell(Cell[,] grid, GridData data, int z, int x)
        {
            // 各セルの位置に向けて上からRayを飛ばして障害物を検知
            Vector3 cellPos = GridIndexToWorldPos(data, z, x);
            Vector3 rayOrigin = cellPos + Vector3.up * data.ObstacleHeight;
            bool isHit = Physics.SphereCast(rayOrigin, data.CellSize / 2, Vector3.down,
                out RaycastHit _, data.ObstacleHeight, data.ObstacleLayer);

            // セルの作成＆コストを設定
            grid[z, x] = new Cell(cellPos, z, x)
            {
                Cost = (byte)(isHit ? byte.MaxValue : 1),
                CalculatedCost = ushort.MaxValue,
            };
        }

        Vector3 GridIndexToWorldPos(GridData data, int z, int x)
        {
            // 辺の大きさが偶数の場合はセルの中心に合わせるオフセットが必要
            float offsetZ = data.Height % 2 == 0 ? 0.5f : 0;
            float offsetX = data.Width % 2 == 0 ? 0.5f : 0;

            float posZ = (data.GridOrigin.z + z - data.Height / 2 + offsetZ) * data.CellSize;
            float posX = (data.GridOrigin.x + x - data.Width / 2 + offsetX) * data.CellSize;
            // y座標はグリッドの高さを基準にして返すので注意
            return new Vector3(posX, data.GridOrigin.y, posZ);
        }
    }
}
