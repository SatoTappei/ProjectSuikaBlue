using UnityEngine;

namespace PSB.InGame
{
    public class SelectedActorCursor : ActorSelector
    {
        // タイルとの重なりを防ぐため
        const float Height = 0.01f;

        [SerializeField] GameObject _prefab;
        [Header("クリック音再生機能搭載")]
        [SerializeField] AudioSource _audioSource;

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

            // クリック音の再生
            _audioSource.Play();
        }

        protected override void OnDeselected()
        {
            _cursor.localScale = Vector3.zero;
        }
    }
}