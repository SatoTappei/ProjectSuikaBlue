using UnityEngine;

namespace PSB.InGame
{
    public interface IBlackBoardForActor
    {
        BaseState InitState { get; }
        ActionType NextAction { set; }
    }

    public interface IBlackBoardForState : IMovable
    {
        BaseState NextState { get; }
        BaseState EvaluateState { get; }
    }

    public interface IMovable
    {
        Transform Transform { get; }
        float Speed { get; }
    }
}
