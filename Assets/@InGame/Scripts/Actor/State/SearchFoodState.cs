using UnityEngine;

namespace PSB.InGame
{
    public class SearchFoodState : SearchResourceState
    {
        public SearchFoodState(DataContext context)
            : base(context, StateType.SearchFood, ResourceType.Tree,
                  () => context.Food.Value += Time.deltaTime * context.Base.HealingRate) { }
    }
}
