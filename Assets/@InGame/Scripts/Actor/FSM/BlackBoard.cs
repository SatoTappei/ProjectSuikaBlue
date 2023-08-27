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
        Actor _enemy;
        Transform _leader;
        // 評価は対応する行動が無い特別な状態なので別途保持する
        EvaluateState _evaluateState;
        // 食べる/飲む度に呼び出される。引数には回復量が渡される
        event UnityAction<float> OnEatFood;
        event UnityAction<float> OnDrinkWater;
        // 雌が子供を産むタイミングで呼び出される。引数には番の遺伝子が渡される
        event UnityAction<uint> OnFemaleBreeding;
        // 雄が雌に産ませるタイミングで呼び出される。引数には番の遺伝子が渡される
        event UnityAction OnMaleBreeding;

        Transform IBlackBoardForState.Leader => _leader;
        BaseState IBlackBoardForState.NextState => TryGetActionState(_nextAction);
        BaseState IBlackBoardForState.EvaluateState => _evaluateState;

        Transform IBlackBoardForActor.Leader { set => _leader = value; }
        BaseState IBlackBoardForActor.InitState => _evaluateState;
        ActionType IBlackBoardForActor.NextAction { get => _nextAction; set => _nextAction = value; }

        Transform IMovable.Transform => _transform ??= transform;
        Transform IMovable.Model => _model;
        float IMovable.Speed => _speed;

        Actor IEnemyReader.Enemy => _enemy;
        Actor IEnemyWriter.Enemy { set => _enemy = value; }

        void Awake()
        {
            _nextAction = ActionType.SearchFood; // <- ここを弄ってデバッグ、既定値はNone
            CreateState();
        }

        void OnDisable()
        {
            OnEatFood = null;
            OnDrinkWater = null;
            OnFemaleBreeding = null;
            OnMaleBreeding = null;
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
            _stateDict.Add(ActionType.Killed, new KilledState(this));
            _stateDict.Add(ActionType.Senility, new SenilityState(this));
            _stateDict.Add(ActionType.Attack, new AttackState(this));
            _stateDict.Add(ActionType.Escape, new EscapeState(this));
            _stateDict.Add(ActionType.Gather, new GatherState(this));
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

        void IBreedingRegister.OnFemaleBreedingRegister(UnityAction<uint> action) => OnFemaleBreeding += action;
        void IBreedingRegister.OnMaleBreedingRegister(UnityAction action) => OnMaleBreeding += action;

        void IStatusInvoker.OnEatFoodInvoke(float value) => OnEatFood.Invoke(value);
        void IStatusInvoker.OnDrinkWaterInvoke(float value) => OnDrinkWater.Invoke(value);

        void IBreedingInvoker.OnFemaleBreedingInvoke(uint value) => OnFemaleBreeding.Invoke(value);
        void IBreedingInvoker.OnMaleBreedingInvoke() => OnMaleBreeding.Invoke();
    }
}