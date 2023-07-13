using System;
using System.Collections.Generic;
using UnityEngine;
using VectorField;

/// <summary>
/// ベクターフィールドの各セルのベクトルを計算の処理だけを抜き出したクラス
/// ベクトルの計算を行うには予めグリッドを生成してコストが付与している必要がある
/// </summary>
public class VectorCalculator
{
    /// <summary>
    /// グリッド上で上下左右の添え字を指定するための配列
    /// </summary>
    static readonly Vector2Int[] FourDirections =
    {
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1),
    };

    /// <summary>
    /// グリッド上で時計回りに八近傍の添え字を指定するための配列
    /// </summary>
    static readonly Vector2Int[] EightDirections =
    {
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1),
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
        new Vector2Int(1, 0),
        new Vector2Int(1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, -1),
    };

    Queue<Cell> _openQueue = new();
    Queue<Cell> _neighbourQueue = new(8);
    Cell[,] _grid;
    GridData _data;

    public VectorCalculator(Cell[,] grid, GridData data)
    {
        _grid = grid;
        _data = data;
    }

    /// <summary>
    /// このメソッドを外部から呼び出すことで、指定した位置に向けたベクトルフィールドを作成する
    /// </summary>
    public Cell CreateVectorField(Vector3 targetPos)
    {
        InitCellAll();
        Cell targetCell = SetTargetCell(targetPos);
        CalculateNeighbourCellCost(targetCell);
        CreateVectorFlow();

        return targetCell;
    }

    void InitCellAll()
    {
        foreach (Cell cell in _grid)
        {
            cell.CalculatedCost = ushort.MaxValue;
            cell.Vector = Vector3.zero;
        }
    }

    /// <summary>
    /// 指定した位置に対応するセルのコスト/計算済みコストをを0にして返す
    /// このセルを基準にベクトルの流れを作る
    /// </summary>
    Cell SetTargetCell(Vector3 targetPos)
    {
        Vector2Int index = WorldPosToGridIndex(targetPos);
        Cell targetCell = _grid[index.y, index.x];
        targetCell.Cost = 0;
        targetCell.CalculatedCost = 0;

        return targetCell;
    }

    /// <summary>
    /// ワールド座標に対応したグリッドの添え字を返す
    /// </summary>
    Vector2Int WorldPosToGridIndex(Vector3 targetPos)
    {
        // グリッドの1辺の長さ
        float forwardZ = _grid[0, 0].Pos.z;
        float backZ = _grid[_data.Height - 1, _data.Width - 1].Pos.z;
        float leftX = _grid[0, 0].Pos.x;
        float rightX = _grid[_data.Height - 1, _data.Width - 1].Pos.x;
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
            x = Mathf.RoundToInt((_data.Width - 1) * percentX),
            y = Mathf.RoundToInt((_data.Height - 1) * percentZ),
        };

        return index;
    }

    /// <summary>
    /// 周囲八近傍を調べてコストを元にベクトルの流れを作成する
    /// </summary>
    void CreateVectorFlow()
    {
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
    }

    /// <summary>
    /// 幅優先探索を用いてコスト/計算済みコストが0のセルの上下左右のセルのコスト/計算済みを計算する
    /// デフォルトの計算済みコストが65535なので最初の1回だけは必ず更新可能
    /// </summary>
    void CalculateNeighbourCellCost(Cell targetCell)
    {
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
    }

    /// <summary>
    /// 選択したセルの8もしくは4方向のセルを開いたセルのキューに挿入する
    /// </summary>
    void InsertNeighbours(Cell current, Vector2Int[] directions)
    {
        foreach (Vector2Int dir in directions)
        {
            int neighbourIndexZ = current.Z + dir.y;
            int neighbourIndexX = current.X + dir.x;

            if (IsWithinGridRange(neighbourIndexZ, neighbourIndexX))
            {
                _neighbourQueue.Enqueue(_grid[neighbourIndexZ, neighbourIndexX]);
            }
        }
    }

    bool IsWithinGridRange(int z, int x)
    {
        return 0 <= z && z < _grid.GetLength(0) && 0 <= x && x < _grid.GetLength(1);
    }

    /// <summary>
    /// 隣接したセルとの位置関係から正規化された方向ベクトルを求める
    /// </summary>
    Vector3 CalculateVectorToNeighbourCell(Cell current, Cell neighbour)
    {
        int indexDirZ = neighbour.Z - current.Z;
        int indexDirX = neighbour.X - current.X;

        Vector3 vector = new Vector3(indexDirX, 0, indexDirZ);
        if (indexDirZ * indexDirX != 0)
        {
            vector.Normalize();
        }

        return vector;
    }
}
