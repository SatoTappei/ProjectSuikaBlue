using UnityEngine;
using UnityEngine.UI;

namespace PSB.InGame
{
    public class SelectedActorView : ActorSelector
    {
        [Header("ëÄçÏÇ∑ÇÈUI")]
        [SerializeField] Transform _root;
        [SerializeField] Text _nameText;
        [SerializeField] Text _stateNameText;
        [SerializeField] Transform _foodBar;
        [SerializeField] Transform _waterBar;
        [SerializeField] Transform _hpBar;
        [SerializeField] Transform _lifeSpanBar;
        [SerializeField] Transform _breedingRateBar;

        IReadOnlyActorStatus _actor;
        string _name;

        void Start()
        {
            OnDeselected();
        }

        protected override void OnUpdate()
        {
            if (_actor != null)
            {
                if (_actor.State == StateType.Senility || _actor.State == StateType.Killed)
                {
                    Invalid();
                }
                else
                {
                    SyncParamsToUI();
                }
            }
        }

        protected override void OnSelected(GameObject actor) 
        {
            _actor = actor.GetComponent<IReadOnlyActorStatus>();
            _name = actor.name;
            _root.localScale = Vector3.one;       
        }

        protected override void OnDeselected() 
        {
            Invalid();
        }

        void Invalid()
        {
            _root.localScale = Vector3.zero;
            _actor = null;
            _name = null;
        }

        void SyncParamsToUI()
        {
            _nameText.text = _name;
            _stateNameText.text = Filtering(_stateNameText.text, _actor.State.ToString());
            _foodBar.localScale         = new Vector3(_actor.Food, 1, 1);
            _waterBar.localScale        = new Vector3(_actor.Water, 1, 1);
            _hpBar.localScale           = new Vector3(_actor.HP, 1, 1);
            _lifeSpanBar.localScale     = new Vector3(_actor.LifeSpan, 1, 1);
            _breedingRateBar.localScale = new Vector3(_actor.BreedingRate, 1, 1);
        }

        string Filtering(string current, string next)
        {
            // ï]âøÉXÉeÅ[ÉgÇï\é¶Ç∑ÇÈÇ∆èuéûÇ…êÿÇËë÷ÇÌÇ¡Çƒå©ÇƒÇ≠ÇÍÇ™à´Ç¢ÇÃÇ≈íeÇ≠
            return next == StateType.Evaluate.ToString() ? current : next;
        }
    }
}