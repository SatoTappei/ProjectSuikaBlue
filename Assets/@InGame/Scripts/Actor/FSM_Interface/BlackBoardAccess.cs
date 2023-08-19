using UnityEngine;
using UnityEngine.Events;

namespace PSB.InGame
{
    public interface IBlackBoardForActor : IStatusRegister
    {
        BaseState InitState { get; }
        ActionType NextAction { set; }
    }

    public interface IBlackBoardForState : IMovable , IStatusInvoker
    {
        BaseState NextState { get; }
        BaseState EvaluateState { get; }
    }

    public interface IMovable
    {
        Transform Transform { get; }
        float Speed { get; }
    }

    public interface IStatusRegister
    {
        void OnEatFoodRegister(UnityAction<float> action);
        void OnDrinkWaterRegister(UnityAction<float> action);
    }

    public interface IStatusInvoker
    {
        void OnEatFoodInvoke(float value);
        void OnDrinkWaterInvoke(float value);
    }
}
