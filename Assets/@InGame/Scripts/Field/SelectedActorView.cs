using UnityEngine;
using UnityEngine.UI;

namespace PSB.InGame
{
    public class SelectedActorView : MonoBehaviour
    {
        const int RayDistance = 100; // 適当な値

        [SerializeField] LayerMask _actorLayer;
        [Header("操作するUI")]
        [SerializeField] Transform _root;
        [SerializeField] Image _icon;
        [SerializeField] Transform _foodBar;
        [SerializeField] Transform _waterBar;
        [SerializeField] Transform _hpBar;

        Camera _mainCamera;
        IReadOnlyParams _actor;

        void Awake()
        {
            _mainCamera = Camera.main;
        }

        void Start()
        {
            CloseUI();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (IsRaycastHitActor(out _actor))
                {
                    OpenUI();
                }
                else
                {
                    CloseUI();
                }
            }

            if (_actor != null) SyncParamsToUI();
        }

        void CloseUI() { _root.localScale = Vector3.zero; _actor = null; }
        void OpenUI()  { _root.localScale = Vector3.one; }

        bool IsRaycastHitActor(out IReadOnlyParams actor)
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit, RayDistance, _actorLayer))
            {
                // Actorコンポーネント(IReadOnlyParams)を取得できたかどうかで判定する
                return hit.collider.TryGetComponent(out actor);
            }
            else
            {
                actor = null;
                return false;
            }
        }

        void SyncParamsToUI()
        {
            _foodBar.localScale = new Vector3(_actor.Food, 1, 1);
            _waterBar.localScale = new Vector3(_actor.Water, 1, 1);
            _hpBar.localScale = new Vector3(_actor.HP, 1, 1);
        }
    }
}