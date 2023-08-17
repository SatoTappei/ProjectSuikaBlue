using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public class BlackBoard : MonoBehaviour
    {
        Dictionary<ActionType, BaseState> _actionStateDict;
        // 評価は対応する行動が無い特別な状態なので別途保持する
        EvaluateState _evaluateState;

        // ステート側が読み取る
        public BaseState NextState => TryGetActionState(NextAction);
        public BaseState EvaluateState => _evaluateState;
        // Actor側から書き込む
        public ActionType NextAction;

        void Awake()
        {
            CreateState();
        }

        void CreateState()
        {
            _evaluateState = new(this);

            _actionStateDict = new(4);
            _actionStateDict.Add(ActionType.SearchFood, new SearchFoodState(this));
        }

        BaseState TryGetActionState(ActionType type)
        {
            if (_actionStateDict.ContainsKey(type))
            {
                return _actionStateDict[type];
            }
            else
            {
                throw new KeyNotFoundException("遷移先のステートが存在しない: " + type);
            }
        }
    }
}