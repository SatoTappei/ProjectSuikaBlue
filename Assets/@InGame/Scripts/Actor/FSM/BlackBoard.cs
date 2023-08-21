using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PSB.InGame
{
    // 現状ものびを継承している必要はないのだが
    // 後々インスペクタから割り当てる必要があるようになるかもしれないのでものびを継承している

    [DefaultExecutionOrder(-1)]
    public class BlackBoard : MonoBehaviour, IBlackBoardForActor, IBlackBoardForState 
    {
        [SerializeField] ActorType _actorType;
        [SerializeField] float _speed;
        [SerializeField] Transform _model;

        ActionType _nextAction;
        Dictionary<ActionType, BaseState> _stateDict;
        Transform _transform;
        // 評価は対応する行動が無い特別な状態なので別途保持する
        EvaluateState _evaluateState;
        // 食べる/飲む度に呼び出される。引数には回復量が渡される
        event UnityAction<float> OnEatFood;
        event UnityAction<float> OnDrinkWater;
        // 雌が子供を産むタイミングで呼び出される。引数には番の遺伝子が渡される
        event UnityAction<uint> OnBreeding;

        BaseState IBlackBoardForState.NextState => TryGetActionState(_nextAction);
        BaseState IBlackBoardForState.EvaluateState => _evaluateState;
        
        BaseState IBlackBoardForActor.InitState => _evaluateState;
        ActionType IBlackBoardForActor.NextAction { get => _nextAction; set => _nextAction = value; }

        Transform IMovable.Transform => _transform ??= transform;
        Transform IMovable.Model => _model;
        float IMovable.Speed => _speed;

        void Awake()
        {
            _nextAction = ActionType.SearchFood; // <- ここを弄ってデバッグ、既定値はNone
            CreateState();
        }

        void OnDisable()
        {
            OnEatFood = null;
            OnDrinkWater = null;
            OnBreeding = null;
        }

        void CreateState()
        {
            _evaluateState = new(this);

            _stateDict = new(4);
            _stateDict.Add(ActionType.SearchFood, new SearchFoodState(this));
            _stateDict.Add(ActionType.SearchWater, new SearchWaterState(this));
            _stateDict.Add(ActionType.None, new IdleState(this));
            _stateDict.Add(ActionType.Wander, new WanderState(this));
            _stateDict.Add(ActionType.Breed, new BreedState(this));
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

        void IStatusRegister.OnEatFoodRegister(UnityAction<float> action) => OnEatFood += action;
        void IStatusRegister.OnDrinkWaterRegister(UnityAction<float> action) => OnDrinkWater += action;

        void IBreedingRegister.OnBreedingRegister(UnityAction<uint> action) => OnBreeding += action; // <- これをActorに実装する

        void IStatusInvoker.OnEatFoodInvoke(float value) => OnEatFood.Invoke(value);
        void IStatusInvoker.OnDrinkWaterInvoke(float value) => OnDrinkWater.Invoke(value);

        void IBreedingInvoker.OnBreedingInvoke(uint value) => OnBreeding.Invoke(value);
    }
}