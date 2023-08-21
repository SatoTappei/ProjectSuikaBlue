using UnityEngine;
using UnityEngine.Events;

namespace PSB.InGame
{
    public interface IBlackBoardForActor : IBreedMatching, IStatusRegister 
    {
        BaseState InitState { get; }
        ActionType NextAction { get; set; }
    }

    public interface IBlackBoardForState : IBreedable, IMovable, IStatusInvoker
    {
        BaseState NextState { get; }
        BaseState EvaluateState { get; }
    }

    public interface IBreedMatching
    {
        Actor Partner { set; }
        Sex Sex { set; }
    }

    public interface IBreedable
    {
        ActorType ActorType { get; }
        Actor Partner { get; }
        Sex Sex { get; }
    }

    public interface IMovable
    {
        Transform Transform { get; }
        Transform Model { get; }
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
