using UnityEngine;

namespace Old
{
    /// <summary>
    /// 8�����̃x�N�g������ŕ`�悷��N���X
    /// �f�o�b�O�p�Ȃ̂Ŗ����Ă����삷��
    /// </summary>
    public class VectorVisualizer : MonoBehaviour
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
        /// �q�I�u�W�F�N�g(���)��S�폜����
        /// �x�N�^�[�t�B�[���h�̍X�V���ɌĂяo�����
        /// </summary>
        public void RemoveAll()
        {
            while (_parent.transform.childCount > 0)
            {
                Destroy(_parent.transform.GetChild(0));
            }
        }

        public void VisualizeCellVector(Cell cell)
        {
            EightDirection dir = Vector3To8Direction(cell.Vector);
            if(dir != EightDirection.Neutral)
            {
                Add(cell.Pos, dir);
            }
        }

        /// <summary>
        /// Vector3�ɑΉ�����8�����̗񋓌^��Ԃ�
        /// </summary>
        EightDirection Vector3To8Direction(Vector3 vector)
        {
            if (vector == new Vector3(0, 0, 1)) return EightDirection.North;
            if (vector == new Vector3(0, 0, -1)) return EightDirection.South;
            if (vector == new Vector3(1, 0, 0)) return EightDirection.East;
            if (vector == new Vector3(-1, 0, 0)) return EightDirection.West;
            if (vector == new Vector3(1, 0, 1)) return EightDirection.NorthEast;
            if (vector == new Vector3(1, 0, -1)) return EightDirection.SouthEast;
            if (vector == new Vector3(-1, 0, 1)) return EightDirection.NorthWest;
            if (vector == new Vector3(-1, 0, -1)) return EightDirection.SouthWest;

            Debug.LogError("�x�N�g���ɑΉ���������������: " + vector);
            return EightDirection.Neutral;
        }

        /// <summary>
        /// �w�肵���ʒu/�����̖��𐶐�����
        /// </summary>
        void Add(Vector3 pos, EightDirection dir)
        {
            GameObject cursor = new GameObject("Cursor " + dir);
            cursor.transform.position = pos + Vector3.up * 0.01f;
            cursor.transform.SetParent(_parent.transform);
            SpriteRenderer sr = cursor.AddComponent<SpriteRenderer>();
            sr.sprite = _cursorSprite;
            
            // 8�����ɑΉ������p�x�ɉ�]������
            switch (dir)
            {
                case EightDirection.North: cursor.transform.Rotate(90, -90, 0);
                    break;
                case EightDirection.South: cursor.transform.Rotate(90, 90, 0);
                    break;
                case EightDirection.West: cursor.transform.Rotate(90, 180, 0);
                    break;
                case EightDirection.East: cursor.transform.Rotate(90, 0, 0);
                    break;
                case EightDirection.NorthEast: cursor.transform.Rotate(90, -45, 0);
                    break;
                case EightDirection.NorthWest: cursor.transform.Rotate(90, 315, 0);
                    break;
                case EightDirection.SouthEast: cursor.transform.Rotate(90, 45, 0);
                    break;
                case EightDirection.SouthWest: cursor.transform.Rotate(90, 135, 0);
                    break;
            }
        }
    }
}

// �w�肵���ʒu�̃x�N�^�[�t�B�[���h�쐬 <- ����
// �w�肵���ʒu����^�[�Q�b�g�Z���܂� �� �w�肵���ʒu����O���b�h�̒[�܂�
// Vector3�^�̃R���N�V�����ŕԂ�