using UnityEngine;

namespace VectorField
{
    public enum EightDirection
    {
        North,
        South,
        West,
        East,
        NorthEast,
        NorthWest,
        SouthEast,
        SouthWest,
    }

    /// <summary>
    /// 8�����̃x�N�g������ŕ`�悷��N���X
    /// �f�o�b�O�p�Ȃ̂Ŗ����Ă����삷��
    /// </summary>
    public class VectorVisualizer : MonoBehaviour
    {
        [Header("���̉摜")]
        [SerializeField] Sprite _CursorSprite;

        Transform _parent;

        void Awake()
        {
            _parent = new GameObject("VectorVisualParent").transform;
            _parent.SetParent(transform);
        }

        public void Valid() => _parent.gameObject.SetActive(true);
        public void Invalid() => _parent.gameObject.SetActive(false);
        public void RemoveAll()
        {
            // �q�I�u�W�F�N�g��S�폜���čX�V����̂łƂĂ��d��
            while (_parent.childCount > 0)
            {
                Destroy(_parent.GetChild(0));
            }
        }

        public void Add(Vector3 pos, EightDirection dir)
        {
            GameObject cursor = new GameObject("Cursor " + dir);
            cursor.transform.position = pos + Vector3.up * 0.01f;
            cursor.transform.SetParent(_parent);
            SpriteRenderer sr = cursor.AddComponent<SpriteRenderer>();
            sr.sprite = _CursorSprite;

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