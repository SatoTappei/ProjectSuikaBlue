using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    // 現状ものびを継承している必要はないのだが
    // 後々インスペクタから割り当てる必要があるようになるかもしれないのでものびを継承している

    [DefaultExecutionOrder(-1)]
    public class BlackBoard : MonoBehaviour, IBlackBoardForActor, IBlackBoardForState
    {
        [SerializeField] float _speed;

        Dictionary<ActionType, BaseState> _stateDict;
        // 評価は対応する行動が無い特別な状態なので別途保持する
        EvaluateState _evaluateState;
        ActionType _nextAction;

        BaseState IBlackBoardForState.NextState => TryGetActionState(_nextAction);
        BaseState IBlackBoardForState.EvaluateState => _evaluateState;
        Transform IMovable.Transform => transform;
        float IMovable.Speed => _speed;

        BaseState IBlackBoardForActor.InitState => _evaluateState;
        ActionType IBlackBoardForActor.NextAction { set => _nextAction = value; }

        void Awake()
        {
            _nextAction = ActionType.SearchFood; // <- ここを弄ってデバッグ、既定値はNone
            CreateState();
        }

        void CreateState()
        {
            _evaluateState = new(this);

            _stateDict = new(4);
            _stateDict.Add(ActionType.SearchFood, new SearchFoodState(this));
            _stateDict.Add(ActionType.None, new IdleState(this));
        }

        BaseState TryGetActionState(ActionType type)
        {
            if (_stateDict.ContainsKey(type))
            {
                return _stateDict[type];
            }
            else
            {
                throw new KeyNotFoundException("遷移先のステートが存在しない: " + type);
            }
        }
    }
}