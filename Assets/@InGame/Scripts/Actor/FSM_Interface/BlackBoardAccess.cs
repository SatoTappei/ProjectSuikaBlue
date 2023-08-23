using UnityEngine;
using UnityEngine.Events;

namespace PSB.InGame
{
    public interface IBlackBoardForActor : IBreedingRegister, IStatusRegister 
    {
        BaseState InitState { get; }
        ActionType NextAction { get; set; }
    }

    public interface IBlackBoardForState : IBreedingInvoker, IMovable, IStatusInvoker
    {
        BaseState NextState { get; }
        BaseState EvaluateState { get; }
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

    public interface IBreedingRegister
    {
        void OnMaleBreedingRegister(UnityAction action);
        void OnFemaleBreedingRegister(UnityAction<uint> action);
    }

    public interface IBreedingInvoker
    {
        void OnMaleBreedingInvoke();
        void OnFemaleBreedingInvoke(uint value);
    }
}
