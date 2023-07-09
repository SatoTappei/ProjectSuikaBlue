using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VectorField
{
    /// <summary>
    /// 
    /// </summary>
    public class VectorCalculator
    {
        static readonly (int z, int x)[] FourDirections =
        {
            (-1, 0), (1, 0), (0, -1), (0, 1),
        };

        static readonly (int z, int x)[] EightDirections =
        {
            (-1, 0), (1, 0), (0, -1), (0, 1), (-1, 1), (-1, -1), (1, 1), (1, -1),
        };

        Cell[,] _grid;
        VectorVisualizer _visualizer;

        public VectorCalculator(Cell[,] grid, VectorVisualizer visualizer)
        {
            _grid = grid;
            _visualizer = visualizer;
        }

        public void SetTargetCell(Vector3 targetPos)
        {
#if UNITY_EDITOR
            if (_visualizer != null) _visualizer.RemoveAll();
#endif

            Vector3ToIndex(targetPos, out int z, out int x);
            Cell targetCell = _grid[z, x];
            targetCell.Cost = 0;
            targetCell.CalculatedCost = 0;

            Queue<Cell> openQueue = new Queue<Cell>();
            openQueue.Enqueue(targetCell);

            List<Cell> neighbourList = new List<Cell>(4);
            while (openQueue.Count > 0)
            {
                Cell current = openQueue.Dequeue();


                GetNeighbours(current, neighbourList);
                foreach (Cell neighbour in neighbourList)
                {
                    // 隣のセルのコストが最大の場合は処理しない
                    if (neighbour.Cost == byte.MaxValue) continue;
                    // 隣のセルの計算済みコストがより小さくなる場合は更新
                    if (neighbour.Cost + current.CalculatedCost < neighbour.CalculatedCost)
                    {
                        neighbour.CalculatedCost = (ushort)(neighbour.Cost + current.CalculatedCost);
                        openQueue.Enqueue(neighbour);
                    }
                }
            }

            CreateVectorField();
        }

        EightDirection GetDir(Vector3 vec)
        {
            if(vec == new Vector3(0, 0, 1))
            {
                return EightDirection.North;
            }
            if (vec == new Vector3(0, 0, -1))
            {
                return EightDirection.South;
            }
            if (vec == new Vector3(1, 0, 0))
            {
                return EightDirection.East;
            }
            if (vec == new Vector3(-1, 0, 0))
            {
                return EightDirection.West;
            }
            if (vec == new Vector3(1, 0, 1))
            {
                return EightDirection.NorthEast;
            }
            if (vec == new Vector3(1, 0, -1))
            {
                return EightDirection.SouthEast;
            }
            if (vec == new Vector3(-1, 0, 1))
            {
                return EightDirection.NorthWest;
            }
            if (vec == new Vector3(-1, 0, -1))
            {
                return EightDirection.SouthWest;
            }

            Debug.LogError("ベクトルから方向が取れない: " + vec);
            return EightDirection.North;
        }

        void Vector3ToIndex(Vector3 pos, out int z, out int x)
        {
            z = 0;
            x = 0;
        }

        void GetNeighbours(Cell current, List<Cell> list)
        {


            foreach ((int z, int x) dir in FourDirections)
            {
                int neighbourZ = current.Z + dir.z;
                int neighbourX = current.X + dir.x;

                if (!IsWithinGridRange(neighbourZ, neighbourX)) continue;

                list.Add(_grid[neighbourZ, neighbourX]);
            }
        }

        bool IsWithinGridRange(int z, int x)
        {
            return (0 <= z && z < _grid.GetLength(0) && 0 <= x && x < _grid.GetLength(1));
        }

        void CreateVectorField()
        {
            foreach (Cell cell in _grid)
            {
                List<Cell> neighbours = GetEightNeighbours(cell);
                int calculatedCost = cell.CalculatedCost;
                foreach (Cell neighbour in neighbours)
                {
                    if (neighbour.CalculatedCost < calculatedCost)
                    {
                        calculatedCost = neighbour.CalculatedCost;
                        cell.Vector = GetVector(neighbour, cell);
                    }
                }

#if UNITY_EDITOR
                if (_visualizer != null) _visualizer.Add(cell.Pos, GetDir(cell.Vector));
#endif
            }
        }

        List<Cell> GetEightNeighbours(Cell cell)
        {
            List<Cell> neighbourList = new();
            foreach ((int z, int x) dir in EightDirections)
            {
                if (!IsWithinGridRange(cell.Z + dir.z, cell.X + dir.x)) continue;
                Cell neighbour = GetCellAtRelativePos(cell, dir);
                neighbourList.Add(neighbour);
            }

            return neighbourList;
        }

        Cell GetCellAtRelativePos(Cell cell, (int z, int x) dir)
        {
            return _grid[cell.Z + dir.z, cell.X + dir.x];
        }

        Vector3 GetVector(Cell neighbour, Cell current)
        {
            var nz = neighbour.Z;
            var nx = neighbour.X;
            var cz = current.Z;
            var cx = current.X;

            if (nz - cz == 0 && nx - cx == 0)
            {
                return Vector3.zero;
            }

            foreach(var dir in EightDirections)
            {
                if(nz-cz == dir.z && nx - cx == dir.x)
                {
                    return new Vector3(dir.x, 0, dir.z);
                }
            }

            Debug.LogError($"違う: z {nz - cz} x {nx - cx}");
            return Vector3.zero;
        }
    }
}
