using UnityEngine;
using UnityEngine.UI;

namespace PSB.InGame
{
    public class SelectedActorView : ActorSelector
    {
        [Header("ëÄçÏÇ∑ÇÈUI")]
        [SerializeField] Transform _root;
        [SerializeField] Image _icon;
        [SerializeField] Text _nameText;
        [SerializeField] Text _actionNameText;
        [SerializeField] Transform _foodBar;
        [SerializeField] Transform _waterBar;
        [SerializeField] Transform _hpBar;
        [SerializeField] Transform _lifeSpanBar;
        [SerializeField] Transform _breedingRateBar;

        IReadOnlyParams _actor;

        void Start()
        {
            OnDeselected();
        }

        protected override void OnUpdate()
        {
            if (_actor != null) SyncParamsToUI();
        }

        protected override void OnSelected(GameObject actor) 
        {
            _actor = actor.GetComponent<IReadOnlyParams>();
            _root.localScale = Vector3.one;       
        }

        protected override void OnDeselected() 
        {
            _root.localScale = Vector3.zero; 
            _actor = null;
        }

        void SyncParamsToUI()
        {
            _nameText.text = _actor.Name;
            _actionNameText.text = _actor.ActionName;
            _foodBar.localScale         = new Vector3(_actor.Food, 1, 1);
            _waterBar.localScale        = new Vector3(_actor.Water, 1, 1);
            _hpBar.localScale           = new Vector3(_actor.HP, 1, 1);
            _lifeSpanBar.localScale     = new Vector3(_actor.LifeSpan, 1, 1);
            _breedingRateBar.localScale = new Vector3(_actor.BreedingRate, 1, 1);
        }
    }
}