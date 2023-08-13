using UnityEngine;
using UnityEngine.Events;

namespace PSB.InGame
{
    [RequireComponent(typeof(FieldManager))]
    public class FieldClickHandler : MonoBehaviour
    {
        const int RayDistance = 100; // �K���Ȓl

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
            // Raycast���q�b�g�����ꍇ�͂��̒n�_���A���Ȃ������ꍇ��Vector3�̏����l��������
            hitPoint = hit.collider != null ? hit.point : Vector3.zero;

            return hit.collider != null;
        }

        /// <summary>
        /// ���[���h���W�ɑΉ������O���b�h�̓Y������Ԃ�
        /// </summary>
        Vector2Int WorldPosToGridIndex(in Vector3 pos)
        {
            Cell[,] field = _fieldManager.Field;

            // �O���b�h�̑O�㍶�E
            float forwardZ = field[0, 0].Pos.z;
            float backZ = field[field.GetLength(0) - 1, 0].Pos.z;
            float leftX = field[0, 0].Pos.x;
            float rightX = field[0, field.GetLength(1) - 1].Pos.x;
            // �O���b�h��1��
            float width = rightX - leftX;
            float height = backZ - forwardZ;
            // �O���b�h�̒[����̋���
            float fromPosZ = pos.z - forwardZ;
            float fromPosX = pos.x - leftX;
            // �O���b�h�̋����牽���̈ʒu��
            float percentZ = Mathf.Abs(fromPosZ / height);
            float percentX = Mathf.Abs(fromPosX / width);

            // x�͂��̂܂܁Ay��z�ɑΉ����Ă���
            Vector2Int index = new Vector2Int()
            {
                x = Mathf.RoundToInt((field.GetLength(1) - 1) * percentX),
                y = Mathf.RoundToInt((field.GetLength(0) - 1) * percentZ),
            };

            return index;
        }
    }
}
