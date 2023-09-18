using UnityEngine;

namespace PSB.InGame
{
    public class SelectedCellVisualizer : MonoBehaviour
    {
        const int RayDistance = 100;      // 適当な値
        const float CellOffsetY = 0.501f; // タイルの大きさが0.5なのでそれと重ならないように

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
            // シングルトンのインスタンスの有無を確認
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
            // Raycastがヒットした場合はその地点を、しなかった場合はVector3の初期値を代入する
            hitPoint = hit.collider != null ? hit.point : default;

            return hit.collider != null;
        }
    }
}
