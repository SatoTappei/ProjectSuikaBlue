using System.Collections.Generic;

namespace PSB.InGame
{
    public class StateHolder
    {
        Dictionary<ActionType, BaseState> _stateDict;
        // ï]âøÇÕëŒâûÇ∑ÇÈçsìÆÇ™ñ≥Ç¢ì¡ï Ç»èÛë‘Ç»ÇÃÇ≈ï ìrï€éùÇ∑ÇÈ
        EvaluateState _evaluateState;

        public StateHolder(DataContext context)
        {
            CreateState(context);
        }

        public EvaluateState EvaluateState
        {
            get => _evaluateState;
        }
        public BaseState this[ActionType key]
        {
            get
            {
                if (_stateDict.ContainsKey(key))
                {
                    return _stateDict[key];
                }
                else
                {
                    throw new KeyNotFoundException("ëJà⁄êÊÇÃÉXÉeÅ[ÉgÇ™ë∂ç›ÇµÇ»Ç¢: " + key);
                }
            }
        }

        void CreateState(DataContext context)
        {
            _evaluateState = new(context);

            _stateDict = new(Utility.GetEnumLength<ActionType>());
            _stateDict.Add(ActionType.Killed, new KilledState(context));
            _stateDict.Add(ActionType.Senility, new SenilityState(context));
            _stateDict.Add(ActionType.Attack, new AttackState(context));
            _stateDict.Add(ActionType.Escape, new EscapeState(context));
            _stateDict.Add(ActionType.Gather, new GatherState(context));
            _stateDict.Add(ActionType.Breed, new MaleBreedState(context));
            _stateDict.Add(ActionType.SearchFood, new SearchFoodState(context));
            _stateDict.Add(ActionType.SearchWater, new SearchWaterState(context));
            _stateDict.Add(ActionType.Wander, new WanderState(context));
            _stateDict.Add(ActionType.None, new IdleState(context));
        }
    }
}
