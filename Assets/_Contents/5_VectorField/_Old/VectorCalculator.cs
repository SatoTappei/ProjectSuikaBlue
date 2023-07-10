using System.Collections.Generic;
using UnityEngine;

namespace Old
{
    /// <summary>
    /// 8方向の列挙型
    /// </summary>
    public enum EightDirection
    {
        Neutral, // どの方向でもない
        North,
        South,
        West,
        East,
        NorthEast,
        NorthWest,
        SouthEast,
        SouthWest,
    }

    /// <summary>
    /// ベクターフィールドの各セルのベクトルを計算の処理だけを抜き出したクラス
    /// 予めグリッドを生成してコストが付与している必要がある
    /// </summary>
    public class VectorCalculator
    {
        /// <summary>
        /// グリッド上で上下左右の添え字を指定するための配列
        /// </summary>
        static readonly (int z, int x)[] FourDirections =
        {
            (-1, 0), (1, 0), (0, -1), (0, 1),
        };

        /// <summary>
        /// グリッド上で八近傍の添え字を指定するための配列
        /// </summary>
        static readonly (int z, int x)[] EightDirections =
        {
            (-1, 0), (1, 0), (0, -1), (0, 1), 
            (-1, 1), (-1, -1), (1, 1), (1, -1),
        };

        Queue<Cell> _openQueue;
        Queue<Cell> _neighbourQueue;
        Cell[,] _grid;
        Vector3 _gridOrigin;
        float _cellSize;

        public VectorCalculator(Cell[,] grid, Vector3 gridOrigin, float cellSize)
        {
            _openQueue = new Queue<Cell>();
            // 4方向調べるので初期容量は4で固定
            _neighbourQueue = new Queue<Cell>(4);
            _grid = grid;
            _gridOrigin = gridOrigin;
            _cellSize = cellSize;
        }

        /// <summary>
        /// 外部から呼び出すことで指定した位置に向かうベクトルの流れを作成する
        /// ターゲットとなるセルを返す
        /// </summary>
        public Cell CreateVectorField(Vector3 targetPos)
        {
            // 指定した位置のコストを0にする
            WorldPosToGridIndex(targetPos, out int z, out int x);
            Cell targetCell = _grid[z, x];
            targetCell.Cost = 0;
            targetCell.CalculatedCost = 0;

            // 選択したセルから上下左右4方向のセルを調べていく
            _openQueue.Clear();
            _openQueue.Enqueue(targetCell);
            while (_openQueue.Count > 0)
            {
                Cell current = _openQueue.Dequeue();

                // 上下左右
                _neighbourQueue.Clear();
                InsertNeighbours(current, FourDirections);
                foreach (Cell neighbour in _neighbourQueue)
                {
                    // 隣のセルのコストが最大の場合は処理しない
                    if (neighbour.Cost == byte.MaxValue) continue;
                    // 隣のセルの計算済みコストがより小さくなる場合は更新
                    if (neighbour.Cost + current.CalculatedCost < neighbour.CalculatedCost)
                    {
                        neighbour.CalculatedCost = (ushort)(neighbour.Cost + current.CalculatedCost);
                        _openQueue.Enqueue(neighbour);
                    }
                }
            }

            // 周囲八近傍を調べてコストを元にベクトルフィールドを作成する
            foreach (Cell cell in _grid)
            {
                _neighbourQueue.Clear();
                InsertNeighbours(cell, EightDirections);
                int baseCalculatedCost = cell.CalculatedCost;
                foreach (Cell neighbour in _neighbourQueue)
                {
                    // 基準となる計算済みコストより周囲のセルの計算済みコストの方が低い場合は
                    // その方向へのベクトルを作成して基準を更新
                    if (neighbour.CalculatedCost < baseCalculatedCost)
                    {
                        baseCalculatedCost = neighbour.CalculatedCost;
                        cell.Vector = CalculateVectorToNeighbourCell(cell, neighbour);
                    }
                }
            }

            return targetCell;
        }

        /// <summary>
        /// 指定した位置からターゲットもしくは行き止まりまでのベクトルの流れを返す
        /// </summary>
        public List<Vector3> GetFlow(Vector3 pos)
        {
            WorldPosToGridIndex(pos, out int z, out int x);
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = _grid[z, x].Pos;

            return null;
        }

        /// <summary>
        /// ワールド座標に対応したグリッドの添え字を返す
        /// </summary>
        void WorldPosToGridIndex(Vector3 targetPos, out int z, out int x)
        {
            float forwardZ = _grid[0, 0].Pos.z;
            float backZ = _grid[_grid.GetLength(0) - 1, _grid.GetLength(1) - 1].Pos.z;
            float leftX = _grid[0, 0].Pos.x;
            float rightX = _grid[_grid.GetLength(0) - 1, _grid.GetLength(1) - 1].Pos.x;

            float lengthZ = backZ - forwardZ;
            float lengthX = rightX - leftX;

            float fromPosZ = targetPos.z - forwardZ;
            float fromPosX = targetPos.x - leftX;
            // グリッドの端から何％の位置か
            float percentZ = Mathf.Abs(fromPosZ / lengthZ);
            float percentX = Mathf.Abs(fromPosX / lengthX);
            // 添え字に対応させる
            z = Mathf.RoundToInt((_grid.GetLength(0) - 1) * percentZ);
            x = Mathf.RoundToInt((_grid.GetLength(1) - 1) * percentX);

            //Vector3 relativePos = targetPos - _gridOrigin;
            //Vector3 cellPos = relativePos / _cellSize;

            //z = Mathf.FloorToInt(cellPos.x);
            //x = Mathf.FloorToInt(cellPos.z);
        }

        /// <summary>
        /// 選択したセルの8もしくは4方向のセルを開いたセルのキューに挿入する
        /// </summary>
        void InsertNeighbours(Cell current, (int z, int x)[] directions)
        {
            foreach ((int z, int x) dir in directions)
            {
                int neighbourZ = current.Z + dir.z;
                int neighbourX = current.X + dir.x;

                if (IsWithinGridRange(neighbourZ, neighbourX))
                {
                    _neighbourQueue.Enqueue(_grid[neighbourZ, neighbourX]);
                }
            }
        }

        bool IsWithinGridRange(int z, int x)
        {
            return (0 <= z && z < _grid.GetLength(0) && 0 <= x && x < _grid.GetLength(1));
        }

        List<Cell> GetEightNeighbours(Cell cell)
        {
            List<Cell> neighbourList = new();
            //foreach ((int z, int x) dir in EightDirections)
            //{
            //    if (!IsWithinGridRange(cell.Z + dir.z, cell.X + dir.x)) continue;
            //    Cell neighbour = GetCellAtRelativePos(cell, dir);
            //    neighbourList.Add(neighbour);
            //}

            return neighbourList;
        }

        Cell GetCellAtRelativePos(Cell cell, (int z, int x) dir)
        {
            return _grid[cell.Z + dir.z, cell.X + dir.x];
        }



        Vector3 CalculateVectorToNeighbourCell(Cell current, Cell neighbour)
        {
            int indexDirZ = neighbour.Z - current.Z;
            int indexDirX = neighbour.X - current.X;
            return new Vector3(-indexDirX, 0, -indexDirZ);

            //foreach ((int z, int x) dir in EightDirections)
            //{
            //    return new Vector3(-indexDirX, 0, -indexDirZ);
            //}

            //if (nz - cz == 0 && nx - cx == 0)
            //{
            //    return Vector3.zero;
            //}

            //foreach(var dir in EightDirections)
            //{
            //    if(nz-cz == dir.z && nx - cx == dir.x)
            //    {
            //        return new Vector3(dir.x, 0, dir.z);
            //    }
            //}

            //Debug.LogError($"違う: z {nz - cz} x {nx - cx}");
            //return Vector3.zero;
        }
    }
}
