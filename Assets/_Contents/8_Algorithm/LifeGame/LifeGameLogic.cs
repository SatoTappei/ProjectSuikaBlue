using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ライフゲームの論理データ。
/// </summary>
public class LifeGameLogic
{
    public enum CellState
    {
        Alive,
        Dead,
    }

    /// <summary>
    /// 行数。
    /// </summary>
    public int Rows { get; }

    /// <summary>
    /// 列数。
    /// </summary>
    public int Columns { get; }

    // 指定の行番号・列番号の状態
    public CellState this[int row, int column] // インデクサー
    {
        get => _cells[row, column];
        set => _cells[row, column] = value;
    }
    private CellState[,] _cells;

    /// <summary>
    /// コンストラクター。
    /// </summary>
    /// <param name="rows">行数。</param>
    /// <param name="columns">列数。</param>
    public LifeGameLogic(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        _cells = new CellState[Rows, Columns];
    }

    /// <summary>
    /// 適当にランダムで生きているセルを設置する。
    /// </summary>
    /// <param name="aliveProbability">生きているセルの出現率。</param>
    public void InitializeRandom(double aliveProbability)
    {
        var random = new System.Random();
        for (var r = 0; r < Rows; r++)
        {
            for (var c = 0; c < Columns; c++)
            {
                _cells[r, c] = random.NextDouble() > aliveProbability
                    ? CellState.Alive : CellState.Dead;
            }
        }
    }

    public void Step(LifeGameLogic next)
    {
        for (var r = 0; r < Rows; r++)
        {
            for (var c = 0; c < Columns; c++)
            {
                var cell = _cells[r, c];

                // 周囲の生きているセル数
                var ac = GetNeighborCount(r, c, CellState.Alive);

                if (cell == CellState.Dead) // 死んでいるセル
                {
                    // 周囲に生きたセルが3つあれば誕生
                    if (ac == 3) { next[r, c] = CellState.Alive; }
                    else { next[r, c] = CellState.Dead; }
                }
                else // 生きているセル
                {
                    // 周囲に生きたセルが2～3つあれば生存、それ以外は死滅
                    if (ac == 2 || ac == 3) { next[r, c] = CellState.Alive; }
                    else { next[r, c] = CellState.Dead; }
                }
            }
        }
    }

    /// <summary>
    /// 指定の行番号・列番号の周囲の指定の状態のセル数を返す。
    /// </summary>
    /// <param name="row">行番号。</param>
    /// <param name="column">列番号。</param>
    /// <param name="state">セルの状態。</param>
    /// <returns>周囲の <paramref name="state"/> 状態のセル数を返す。</returns>
    public int GetNeighborCount(int row, int column, CellState state)
    {
        int count = 0;

        // 周囲のセルが指定の状態なら count をインクリメントする。
        { if (TryGetCell(row - 1, column - 1, out CellState x) && x == state) { count++; } }
        { if (TryGetCell(row - 1, column, out CellState x) && x == state) { count++; } }
        { if (TryGetCell(row - 1, column + 1, out CellState x) && x == state) { count++; } }
        { if (TryGetCell(row, column - 1, out CellState x) && x == state) { count++; } }
        { if (TryGetCell(row, column + 1, out CellState x) && x == state) { count++; } }
        { if (TryGetCell(row + 1, column - 1, out CellState x) && x == state) { count++; } }
        { if (TryGetCell(row + 1, column, out CellState x) && x == state) { count++; } }
        { if (TryGetCell(row + 1, column + 1, out CellState x) && x == state) { count++; } }

        return count;
    }

    /// <summary>
    /// 指定の行番号・列番号のセルを取得する。
    /// </summary>
    /// <param name="row">行番号。</param>
    /// <param name="column">列番号。</param>
    /// <param name="state">セル。</param>
    /// <returns>セルを取得できれば true。そうでなければ false。</returns>
    public bool TryGetCell(int row, int column, out CellState state)
    {
        if (row < 0 || column < 0 || row >= _cells.GetLength(0) || column >= _cells.GetLength(1))
        {
            state = default;
            return false;
        }

        state = _cells[row, column];
        return true;
    }
}