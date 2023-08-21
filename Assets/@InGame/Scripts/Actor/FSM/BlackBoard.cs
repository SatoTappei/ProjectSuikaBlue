using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PSB.InGame
{
    // ������̂т��p�����Ă���K�v�͂Ȃ��̂���
    // ��X�C���X�y�N�^���犄�蓖�Ă�K�v������悤�ɂȂ邩������Ȃ��̂ł��̂т��p�����Ă���

    [DefaultExecutionOrder(-1)]
    public class BlackBoard : MonoBehaviour, IBlackBoardForActor, IBlackBoardForState 
    {
        [SerializeField] ActorType _actorType;
        [SerializeField] float _speed;
        [SerializeField] Transform _model;

        ActionType _nextAction;
        Dictionary<ActionType, BaseState> _stateDict;
        Transform _transform;
        // �]���͑Ή�����s�����������ʂȏ�ԂȂ̂ŕʓr�ێ�����
        EvaluateState _evaluateState;
        // �H�ׂ�/���ޓx�ɌĂяo�����B�����ɂ͉񕜗ʂ��n�����
        event UnityAction<float> OnEatFood;
        event UnityAction<float> OnDrinkWater;
        // �����q�����Y�ރ^�C�~���O�ŌĂяo�����B�����ɂ͔Ԃ̈�`�q���n�����
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
            _nextAction = ActionType.SearchFood; // <- ������M���ăf�o�b�O�A����l��None
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
                throw new KeyNotFoundException("�J�ڐ�̃X�e�[�g�����݂��Ȃ�: " + type);
            }
        }

        void IStatusRegister.OnEatFoodRegister(UnityAction<float> action) => OnEatFood += action;
        void IStatusRegister.OnDrinkWaterRegister(UnityAction<float> action) => OnDrinkWater += action;

        void IBreedingRegister.OnBreedingRegister(UnityAction<uint> action) => OnBreeding += action; // <- �����Actor�Ɏ�������

        void IStatusInvoker.OnEatFoodInvoke(float value) => OnEatFood.Invoke(value);
        void IStatusInvoker.OnDrinkWaterInvoke(float value) => OnDrinkWater.Invoke(value);

        void IBreedingInvoker.OnBreedingInvoke(uint value) => OnBreeding.Invoke(value);
    }
}