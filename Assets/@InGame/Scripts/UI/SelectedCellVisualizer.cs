using UnityEngine;

namespace PSB.InGame
{
    public class SelectedCellVisualizer : MonoBehaviour
    {
        const int RayDistance = 100;      // �K���Ȓl
        const float CellOffsetY = 0.501f; // �^�C���̑傫����0.5�Ȃ̂ł���Əd�Ȃ�Ȃ��悤��

        [SerializeField] LayerMask _fieldLayer;
        [SerializeField] GameObject _cursorPrefab;

        Camera _mainCamera;
        Transform _cursor;

        void Awake()
        {
            _mainCamera = Camera.main;
            _cursor = Instantiate(_cursorPrefab, Vector3.zero, Quaternion.identity).transform;
        }

        void Start()
        {
            // �V���O���g���̃C���X�^���X�̗L�����m�F
            FieldManager.CheckInstance();
        }

        void Update()
        {
            if (IsRaycastHitField(out Vector3 hitPoint))
            {
                if (FieldManager.Instance.TryGetCell(hitPoint, out Cell cell))
                {
                    _cursor.position = cell.Pos + Vector3.up * CellOffsetY;
                }
            }
        }

        bool IsRaycastHitField(out Vector3 hitPoint)
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit, RayDistance, _fieldLayer);
            // Raycast���q�b�g�����ꍇ�͂��̒n�_���A���Ȃ������ꍇ��Vector3�̏����l��������
            hitPoint = hit.collider != null ? hit.point : default;

            return hit.collider != null;
        }
    }
}
