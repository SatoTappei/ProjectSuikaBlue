using UnityEngine;
using VectorField;

/// <summary>
/// �O���b�h�Ɗe�Z���̃R�X�g���M�Y���ɕ\������f�o�b�O�p�̃N���X
/// </summary>
[RequireComponent(typeof(VectorFieldManager))]
public class DebugGridVisualizer : MonoBehaviour
{
    /// <summary>
    /// �M�Y���ւ̕\�����[�h�p�̗񋓌^
    /// </summary>
    enum GizmosViewMode
    {
        None,
        Cost,
        CalculatedCost,
        DetectObstacleRay
    }

    [Header("�M�Y���ւ̕\�����[�h")]
    [SerializeField] GizmosViewMode _gizmosViewMode = GizmosViewMode.CalculatedCost;
    [Header("�Z����`�悷��ۂ̍����̃I�t�Z�b�g")]
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
    /// �O������Ăяo�����ƂŃO���b�h�Ɗe�Z���̃R�X�g��`�悷��
    /// </summary>
    public void DrawGridOnGizmos(Cell[,] grid, GridData data)
    {
        if (_gizmosViewMode == GizmosViewMode.None) return;

        if (_gizmosViewMode == GizmosViewMode.DetectObstacleRay)
        {
            // ��Q�������m����Ray
            DrawDetectObstacleRay(data);
        }
        else
        {
            // �e�Z���̕`��
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
        UnityEditor.Handles.Label(rayOrigin + Vector3.up, "��Q��Ray", _style);
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

        // �ʍs�ł��Ȃ��Z���͐Ԃ�X�A����ȊO�͍��ŃR�X�g��`��
        string text = cost == ushort.MaxValue ? "X" : cost.ToString();
        _styleState.textColor = cost == ushort.MaxValue ? Color.red : Color.black;
        UnityEditor.Handles.Label(cell.Pos, text, _style);
#endif
    }
}