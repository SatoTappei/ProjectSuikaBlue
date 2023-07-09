using UnityEngine;

namespace VectorField
{
    /// <summary>
    /// ベクトルフィールドをギズモに表示するクラス
    /// 各種データを読み取るのみで、GridController側はこのクラスが無くても動作する
    /// </summary>
    [RequireComponent(typeof(GridController))]
    public class GridGizmosDrawer : MonoBehaviour
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

        GridController _gridController;
        GUIStyle _style;
        GUIStyleState _styleState;

        void Awake()
        {
            _gridController = GetComponent<GridController>();
            InitGUIStyle();
        }

        void InitGUIStyle()
        {
            _styleState = new GUIStyleState();
            _style = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                normal = _styleState,
            };
        }

        void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                if (_gridController.Grid == null) return;
                if (_gizmosViewMode == GizmosViewMode.None) return;

                // 各セルの描画
                for (int i = 0; i < _gridController.Height; i++)
                {
                    for (int k = 0; k < _gridController.Width; k++)
                    {
                        DrawCellOnGizmos(_gridController.Grid[i, k], _gridController.CellSize);
                        DrawCellCostOnGizmos(_gridController.Grid[i, k]);
                    }
                }
            }
        }

        void DrawCellOnGizmos(Cell cell, float cellSize)
        {
            Vector3 size = new Vector3(cellSize, 0.01f, cellSize);
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
}
