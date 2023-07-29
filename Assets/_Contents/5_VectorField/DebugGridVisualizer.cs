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
        DetectObstacleRay
    }

    [Header("ギズモへの表示モード")]
    [SerializeField] GizmosViewMode _gizmosViewMode = GizmosViewMode.CalculatedCost;
    [Header("セルを描画する際の高さのオフセット")]
    [SerializeField] float _drawHeightOffset = 0;

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

        if (_gizmosViewMode == GizmosViewMode.DetectObstacleRay)
        {
            // 障害物を検知するRay
            DrawDetectObstacleRay(data);
        }
        else
        {
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
    }

    void DrawDetectObstacleRay(GridData data)
    {
        Vector3 rayOrigin = data.GridOrigin + Vector3.up * data.ObstacleHeight;
        UnityEditor.Handles.Label(rayOrigin + Vector3.up, "障害物Ray", _style);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(rayOrigin, Vector3.down * data.ObstacleHeight);
        Gizmos.DrawWireSphere(rayOrigin, 0.5f);
    }

    void DrawCellIndexOnTGizmos(Cell cell, int i, int k)
    {
        UnityEditor.Handles.Label(cell.Pos, $"({i},{k})", _style);
    }

    void DrawCellOnGizmos(Cell cell, float cellSize)
    {
        Vector3 size = new Vector3(cellSize, 0.01f, cellSize);
        Vector3 pos = cell.Pos;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(pos + Vector3.up * _drawHeightOffset, size);
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