using UnityEngine;

namespace VectorField
{
    /// <summary>
    /// �x�N�g���t�B�[���h���M�Y���ɕ\������N���X
    /// �e��f�[�^��ǂݎ��݂̂ŁAGridController���͂��̃N���X�������Ă����삷��
    /// </summary>
    [RequireComponent(typeof(GridController))]
    public class GridGizmosDrawer : MonoBehaviour
    {
        /// <summary>
        /// �M�Y���ւ̕\�����[�h�p�̗񋓌^
        /// </summary>
        enum GizmosViewMode
        {
            None,
            Cost,
            CalculatedCost,
        }

        [Header("�M�Y���ւ̕\�����[�h")]
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

                // �e�Z���̕`��
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

            // �ʍs�ł��Ȃ��Z���͐Ԃ�X�A����ȊO�͍��ŃR�X�g��`��
            string text = cost == ushort.MaxValue ? "X" : cost.ToString();
            _styleState.textColor = cost == ushort.MaxValue ? Color.red : Color.black;
            UnityEditor.Handles.Label(cell.Pos, text, _style);
#endif
        }
    }
}
