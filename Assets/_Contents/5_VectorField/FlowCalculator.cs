using System.Collections.Generic;
using UnityEngine;
using VectorField;

/// <summary>
/// 生成したベクトルフィールド上の任意の2座標間のベクトルの流れを計算する処理を抜き出したクラス
/// 予めグリッド内の各セルのベクトルが計算済みである必要がある
/// </summary>
public class FlowCalculator
{
    Cell[,] _grid;
    GridData _data;

    public FlowCalculator(Cell[,] grid, GridData data)
    {
        _grid = grid;
        _data = data;
    }

    /// <summary>
    /// ベクトルの流れを引数で渡したキューに挿入していく
    /// このクラスがキューを持ち、そこに経路を詰めて返すと複数のインスタンスから呼び出された際に
    /// キューを空にして新たな経路を詰めると、参照する経路が変わってしまうため、こうしている
    /// </summary>
    public void InsertVectorFlowToQueue(Vector3 startPos, Queue<Vector3> queue)
    {
        queue.Clear();

        // 始点のベクトルをキューに挿入
        Vector2Int index = GridUtility.WorldPosToGridIndex(startPos, _grid, _data);
        queue.Enqueue(_grid[index.y, index.x].Vector);
        // ベクトルの向きに応じた隣のセルのベクトルをキューに挿入していく
        // セル数の数以上ループしないようにして無限ループを防ぐ
        for (int i = 0; i < _data.Height * _data.Width; i++)
        {
            Vector2Int dirIndex = CellVectorToDirIndex(_grid[index.y, index.x]);
            Vector2Int neighbourIndex = index + dirIndex;

            // グリッド外もしくはベクトルの流れの終端に到達した場合
            if (IsFlowEnd(neighbourIndex.y, neighbourIndex.x)) break;

            queue.Enqueue(_grid[neighbourIndex.y, neighbourIndex.x].Vector);
            index = neighbourIndex;
        }
    }

    /// <summary>
    /// セルのベクトルをベクトルが向かう先のセルの添え字に変換して返す
    /// Vector3のxはそのままVector2Intのx、zはyに対応している
    /// </summary>
    Vector2Int CellVectorToDirIndex(Cell cell)
    {
        if (cell.Vector == Vector3.zero) return new Vector2Int(0, 0);

        if (cell.Vector == new Vector3(0, 0, 1)) return new Vector2Int(0, 1);
        else if (cell.Vector == new Vector3(0, 0, -1)) return new Vector2Int(0, -1);
        else if (cell.Vector == new Vector3(1, 0, 0)) return new Vector2Int(1, 0);
        else if (cell.Vector == new Vector3(-1, 0, 0)) return new Vector2Int(-1, 0);
        else if (cell.Vector == new Vector3(1, 0, 1).normalized) return new Vector2Int(1, 1);
        else if (cell.Vector == new Vector3(1, 0, -1).normalized) return new Vector2Int(1, -1);
        else if (cell.Vector == new Vector3(-1, 0, 1).normalized) return new Vector2Int(-1, 1);
        else if (cell.Vector == new Vector3(-1, 0, -1).normalized) return new Vector2Int(-1, -1);
        else
        {
            throw new System.ArgumentException("ベクトルの値がセルの添え字に対応していない: " + cell.Vector);
        }
    }

    bool IsFlowEnd(int z, int x)
    {
        // グリッドの範囲内かチェック
        if (!(0 <= z && z < _grid.GetLength(0) && 0 <= x && x < _grid.GetLength(1))) return true;
        
        // 目標のセルの場合かチェック
        return _grid[z, x].Vector == Vector3.zero;
    }
}
