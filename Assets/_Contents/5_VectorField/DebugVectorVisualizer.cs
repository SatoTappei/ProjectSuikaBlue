using UnityEngine;

namespace VectorField
{
    /// <summary>
    /// �x�N�g���t�B�[���h�𐶐���A�x�N�g���̗����8�����̃x�N�g����
    /// ���ŕ`�悷��f�o�b�O�p�N���X
    /// </summary>
    [RequireComponent(typeof(VectorFieldManager))]
    public class DebugVectorVisualizer : MonoBehaviour
    {
        [SerializeField] Sprite _cursorSprite;
        GameObject _parent;

        void Awake()
        {
            _parent = new GameObject("VectorVisualizeCursors");
            _parent.transform.SetParent(transform);
        }

        public void Valid() => _parent.gameObject.SetActive(true);
        public void Invalid() => _parent.gameObject.SetActive(false);

        /// <summary>
        /// ����S�폜����
        /// �x�N�^�[�t�B�[���h�̍X�V���ɌĂяo�����
        /// </summary>
        public void RemoveAll()
        {
            while (_parent.transform.childCount > 0)
            {
                Destroy(_parent.transform.GetChild(0));
            }
        }

        /// <summary>
        /// �Z���̃x�N�g���ɑΉ��������� 2DSprite �Ƃ��Đ�������
        /// �΂ߕ����̃x�N�g���͐��K���ς݂ł���K�v������
        /// </summary>
        public void VisualizeCellVector(Cell cell)
        {
            if (cell.Vector == Vector3.zero) return;

            if      (cell.Vector == new Vector3(0, 0, 1)) CreateCursor(cell.Pos, 90, -90);
            else if (cell.Vector == new Vector3(0, 0, -1)) CreateCursor(cell.Pos, 90, 90);
            else if (cell.Vector == new Vector3(1, 0, 0)) CreateCursor(cell.Pos, 90, 0);
            else if (cell.Vector == new Vector3(-1, 0, 0)) CreateCursor(cell.Pos, 90, 180);
            else if (cell.Vector == new Vector3(1, 0, 1).normalized) CreateCursor(cell.Pos, 90, -45);
            else if (cell.Vector == new Vector3(1, 0, -1).normalized) CreateCursor(cell.Pos, 90, 45);
            else if (cell.Vector == new Vector3(-1, 0, 1).normalized) CreateCursor(cell.Pos, 90, 315);
            else if (cell.Vector == new Vector3(-1, 0, -1).normalized) CreateCursor(cell.Pos, 90, 135);
            else
            {
                throw new System.ArgumentException("�x�N�g���̒l��8�����̖��ɑΉ����Ă��Ȃ�: " + cell.Vector);
            }
        }

        /// <summary>
        /// �w�肵���ʒu/�����̖��𐶐�����
        /// </summary>
        void CreateCursor(Vector3 pos, float rotX, float rotY)
        {
            GameObject cursor = new GameObject("Cursor");
            cursor.transform.position = pos + Vector3.up * 5.0f;
            cursor.transform.SetParent(_parent.transform);
            cursor.transform.Rotate(rotX, rotY, 0);
            SpriteRenderer sr = cursor.AddComponent<SpriteRenderer>();
            sr.sprite = _cursorSprite;
        }
    }
}