using UnityEngine;

namespace PSB.InGame
{
    public class SearchWaterState : SearchResourceState
    {
        public SearchWaterState(DataContext context) 
            : base(context, StateType.SearchWarter,  ResourceType.Water, 
                  ()=> context.Water.Value += Time.deltaTime * context.Base.HealingRate) { }
    }
}