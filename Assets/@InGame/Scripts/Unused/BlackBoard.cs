//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Events;

//namespace PSB.InGame
//{
//    // ������̂т��p�����Ă���K�v�͂Ȃ��̂���
//    // ��X�C���X�y�N�^���犄�蓖�Ă�K�v������悤�ɂȂ邩������Ȃ��̂ł��̂т��p�����Ă���

//    public class BlackBoard
//    {
//        public BlackBoard(idatacon)
//        {
//            CreateState();
//        }

//        //[SerializeField] ActorType _actorType;
//        //[SerializeField] float _speed;
//        [SerializeField] Transform _model;

//        ActionType _nextAction;
//        Dictionary<ActionType, BaseState> _stateDict;
//        Transform _transform;
//        Actor _enemy;
//        Transform _leader;
//        // �]���͑Ή�����s�����������ʂȏ�ԂȂ̂ŕʓr�ێ�����
//        EvaluateState _evaluateState;
//        // �H�ׂ�/���ޓx�ɌĂяo�����B�����ɂ͉񕜗ʂ��n�����
//        event UnityAction<float> OnEatFood;
//        event UnityAction<float> OnDrinkWater;
//        // �����q�����Y�ރ^�C�~���O�ŌĂяo�����B�����ɂ͔Ԃ̈�`�q���n�����
//        event UnityAction<uint> OnFemaleBreeding;
//        // �Y�����ɎY�܂���^�C�~���O�ŌĂяo�����B�����ɂ͔Ԃ̈�`�q���n�����
//        event UnityAction OnMaleBreeding;

//        Transform IBlackBoardForState.Leader => _leader;
//        BaseState IBlackBoardForState.NextState => TryGetState(_nextAction);
//        BaseState IBlackBoardForState.EvaluateState => _evaluateState;

//        Transform IBlackBoardForActor.Leader { set => _leader = value; }
//        BaseState IBlackBoardForActor.InitState => _evaluateState;
//        ActionType IBlackBoardForActor.NextAction { get => _nextAction; set => _nextAction = value; }

//        Transform IMovable.Transform => _transform ??= transform;
//        Transform IMovable.Model => _model;
//        float IMovable.Speed => _speed;

//        Actor IEnemyReader.Enemy => _enemy;
//        Actor IEnemyWriter.Enemy { set => _enemy = value; }

//        void Awake()
//        {
//            _nextAction = ActionType.SearchFood; // <- ������M���ăf�o�b�O�A����l��None
            
//        }

//        void OnDisable()
//        {
//            OnEatFood = null;
//            OnDrinkWater = null;
//            OnFemaleBreeding = null;
//            OnMaleBreeding = null;
//        }

//        void CreateState()
//        {
//            _evaluateState = new(this);

//            _stateDict = new(4);
//            _stateDict.Add(ActionType.SearchFood, new SearchFoodState(this));
//            _stateDict.Add(ActionType.SearchWater, new SearchWaterState(this));
//            _stateDict.Add(ActionType.None, new IdleState(this));
//            _stateDict.Add(ActionType.Wander, new WanderState(this));
//            _stateDict.Add(ActionType.Breed, new BreedState(this));
//            _stateDict.Add(ActionType.Killed, new KilledState(this));
//            _stateDict.Add(ActionType.Senility, new SenilityState(this));
//            _stateDict.Add(ActionType.Attack, new AttackState(this));
//            _stateDict.Add(ActionType.Escape, new EscapeState(this));
//            _stateDict.Add(ActionType.Gather, new GatherState(this));
//        }

//        BaseState TryGetState(ActionType type)
//        {
//            if (_stateDict.ContainsKey(type))
//            {
//                return _stateDict[type];
//            }
//            else
//            {
//                throw new KeyNotFoundException("�J�ڐ�̃X�e�[�g�����݂��Ȃ�: " + type);
//            }
//        }

//        void IStatusRegister.OnEatFoodRegister(UnityAction<float> action) => OnEatFood += action;
//        void IStatusRegister.OnDrinkWaterRegister(UnityAction<float> action) => OnDrinkWater += action;

//        void IBreedingRegister.OnFemaleBreedingRegister(UnityAction<uint> action) => OnFemaleBreeding += action;
//        void IBreedingRegister.OnMaleBreedingRegister(UnityAction action) => OnMaleBreeding += action;

//        void IStatusInvoker.OnEatFoodInvoke(float value) => OnEatFood.Invoke(value);
//        void IStatusInvoker.OnDrinkWaterInvoke(float value) => OnDrinkWater.Invoke(value);

//        void IBreedingInvoker.OnFemaleBreedingInvoke(uint value) => OnFemaleBreeding.Invoke(value);
//        void IBreedingInvoker.OnMaleBreedingInvoke() => OnMaleBreeding.Invoke();
//    }
//}