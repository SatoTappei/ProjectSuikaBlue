using UnityEngine;
using UnityEngine.Events;

namespace PSB.InGame
{
    [RequireComponent(typeof(FieldManager))]
    public class FieldClickHandler : MonoBehaviour
    {
        const int RayDistance = 100; // 適当な値

        public static event UnityAction<Cell> OnFieldClicked;

        [SerializeField] FieldManager _fieldManager;
        [SerializeField] LayerMask _fieldLayer;

        void Update()
        {
            if (Input.GetMouseButtonDown(0)) OnClicked();
        }

        void OnClicked()
        {
            Cell[,] field = _fieldManager.Field;
            if (IsRaycastHitField(out Vector3 hitPoint))
            {
                Vector2Int index = WorldPosToGridIndex(hitPoint);
                if (FieldUtility.IsWithinGrid(field, index))
                {
                    Cell cell = field[index.y, index.x];
                    OnFieldClicked?.Invoke(cell);
                }
            }
        } 

        bool IsRaycastHitField(out Vector3 hitPoint)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit, RayDistance, _fieldLayer);
            // Raycastがヒットした場合はその地点を、しなかった場合はVector3の初期値を代入する
            hitPoint = hit.collider != null ? hit.point : Vector3.zero;

            return hit.collider != null;
        }

        /// <summary>
        /// ワールド座標に対応したグリッドの添え字を返す
        /// </summary>
        Vector2Int WorldPosToGridIndex(in Vector3 pos)
        {
            Cell[,] field = _fieldManager.Field;

            // グリッドの前後左右
            float forwardZ = field[0, 0].Pos.z;
            float backZ = field[field.GetLength(0) - 1, 0].Pos.z;
            float leftX = field[0, 0].Pos.x;
            float rightX = field[0, field.GetLength(1) - 1].Pos.x;
            // グリッドの1辺
            float width = rightX - leftX;
            float height = backZ - forwardZ;
            // グリッドの端からの距離
            float fromPosZ = pos.z - forwardZ;
            float fromPosX = pos.x - leftX;
            // グリッドの橋から何％の位置か
            float percentZ = Mathf.Abs(fromPosZ / height);
            float percentX = Mathf.Abs(fromPosX / width);

            // xはそのまま、yはzに対応している
            Vector2Int index = new Vector2Int()
            {
                x = Mathf.RoundToInt((field.GetLength(1) - 1) * percentX),
                y = Mathf.RoundToInt((field.GetLength(0) - 1) * percentZ),
            };

            return index;
        }
    }
}
