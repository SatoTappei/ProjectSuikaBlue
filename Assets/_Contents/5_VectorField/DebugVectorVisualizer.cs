using System.Collections.Generic;
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

        public void VisualizeVectorFlow(Cell[,] grid)
        {
            RemoveAll();
            foreach (Cell cell in grid)
            {
                VisualizeCellVector(cell);
            }
        }

        /// <summary>
        /// �x�N�^�[�t�B�[���h�̍X�V���ɌĂяo����A����S�폜����
        /// ���̐����Ɠ����ɌĂяo�����z��Ȃ̂ŁA��x���݂̖����܂Ƃ߂Ă���j������
        /// ���̎菇�𓥂܂Ȃ��ꍇ�������[�v�Ɋׂ�
        /// </summary>
        void RemoveAll()
        {
            List<GameObject> cursorList = new();
            foreach (Transform cursor in _parent.transform)
            {
                cursorList.Add(cursor.gameObject);
            }

            foreach(GameObject cursor in cursorList)
            {
                Destroy(cursor);
            }
        }

        /// <summary>
        /// �Z���̃x�N�g���ɑΉ��������� 2DSprite �Ƃ��Đ�������
        /// �΂ߕ����̃x�N�g���͐��K���ς݂ł���K�v������
        /// </summary>
        void VisualizeCellVector(Cell cell)
        {
            if (cell.Vector == Vector3.zero) return;

            if      (cell.Vector == new Vector3(0, 0, 1)) CreateCursor(cell.Pos, 90, -90);
            else if (cell.Vector == new Vector3(0, 0, -1)) CreateCursor(cell.Pos, 90, 90);
            else if (cell.Vector == new Vector3(1, 0, 0)) CreateCursor(cell.Pos, 90, 0);
            else if (cell.Vector == new Vector3(-1, 0, 0)) CreateCursor(cell.Pos, 90, 180);
            else if (cell.Vector == new Vector3(1, 0, 1).normalized) CreateCursor(cell.Pos, 90, -45);
            else if (cell.Vector == new Vector3(1, 0, -1).normalized) CreateCursor(cell.Pos, 90, 45);
            else if (cell.Vector == new Vector3(-1, 0, 1).normalized) CreateCursor(cell.Pos, 90, -135);
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