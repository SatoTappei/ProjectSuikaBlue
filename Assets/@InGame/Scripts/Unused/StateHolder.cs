using System.Collections.Generic;

namespace PSB.InGame
{
    public class StateHolder
    {
        Dictionary<ActionType, BaseState> _stateDict;
        // 評価は対応する行動が無い特別な状態なので別途保持する
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
                    throw new KeyNotFoundException("遷移先のステートが存在しない: " + key);
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
