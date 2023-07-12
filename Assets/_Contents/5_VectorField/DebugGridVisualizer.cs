using UnityEngine;
using VectorField;

/// <summary>
/// グリッドと各セルのコストをギズモに表示するデバッグ用のクラス
/// </summary>
[RequireComponent(typeof(VectorFieldManager))]
public class DebugGridVisualizer : MonoBehaviour
{
    /// <summary>
    /// ギズモへの表示モード用の列挙型
    /// </summary>
    enum GizmosViewMode
    {
        None,
        Cost,
        CalculatedCost,
    }

    [Header("ギズモへの表示モード")]
    [SerializeField] GizmosViewMode _gizmosViewMode = GizmosViewMode.CalculatedCost;

    GUIStyle _style;
    GUIStyleState _styleState;

    void Awake()
    {
        _styleState = new GUIStyleState();
        _style = new GUIStyle()
        {
            alignment = TextAnchor.MiddleCenter,
            normal = _styleState,
        };
    }

    /// <summary>
    /// 外部から呼び出すことでグリッドと各セルのコストを描画する
    /// </summary>
    public void DrawGridOnGizmos(Cell[,] grid, GridData data)
    {
        if (_gizmosViewMode == GizmosViewMode.None) return;

        // 各セルの描画
        for (int i = 0; i < data.Height; i++)
        {
            for (int k = 0; k < data.Width; k++)
            {
                DrawCellOnGizmos(grid[i, k], data.CellSize);
                DrawCellCostOnGizmos(grid[i, k]);
            }
        }
    }

    void DrawCellIndexOnTGizmos(Cell cell, int i, int k)
    {
        UnityEditor.Handles.Label(cell.Pos, $"({i},{k})", _style);
    }

    void DrawCellOnGizmos(Cell cell, float cellSize)
    {
        Vector3 size = new Vector3(cellSize, 0.01f, cellSize);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(cell.Pos, size);
    }

    void DrawCellCostOnGizmos(Cell cell)
    {
#if UNITY_EDITOR
        int cost = _gizmosViewMode == GizmosViewMode.Cost ? cell.Cost : cell.CalculatedCost;

        // 通行できないセルは赤のX、それ以外は黒でコストを描画
        string text = cost == ushort.MaxValue ? "X" : cost.ToString();
        _styleState.textColor = cost == ushort.MaxValue ? Color.red : Color.black;
        UnityEditor.Handles.Label(cell.Pos, text, _style);
#endif
    }
}
