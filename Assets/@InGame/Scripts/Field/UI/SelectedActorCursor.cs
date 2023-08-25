using UnityEngine;

namespace PSB.InGame
{
    public class SelectedActorCursor : ActorSelector
    {
        // �^�C���Ƃ̏d�Ȃ��h������
        const float Height = 0.01f;

        [SerializeField] GameObject _prefab;

        Transform _cursor;

        void Start()
        {
            _cursor = Instantiate(_prefab).transform;
            OnDeselected();
        }

        protected override void OnSelected(GameObject actor)
        {
            _cursor.localScale = Vector3.one;
            _cursor.SetParent(actor.transform);
            _cursor.localPosition = new Vector3(0, Height, 0);

            // �N���b�N���̍Đ�
            AudioManager.PlayAudio(AudioKey.ClickSE);
        }

        protected override void OnDeselected()
        {
            _cursor.localScale = Vector3.zero;
        }
    }
}