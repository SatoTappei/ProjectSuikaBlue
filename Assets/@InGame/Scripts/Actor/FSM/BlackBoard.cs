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
        [SerializeField] float _speed;

        Dictionary<ActionType, BaseState> _stateDict;
        // �]���͑Ή�����s�����������ʂȏ�ԂȂ̂ŕʓr�ێ�����
        EvaluateState _evaluateState;
        ActionType _nextAction;

        BaseState IBlackBoardForState.NextState => TryGetActionState(_nextAction);
        BaseState IBlackBoardForState.EvaluateState => _evaluateState;
        Transform IMovable.Transform => transform;
        float IMovable.Speed => _speed;

        BaseState IBlackBoardForActor.InitState => _evaluateState;
        ActionType IBlackBoardForActor.NextAction { set => _nextAction = value; }

        // �H�ׂ�/���ޓx�ɌĂяo�����B�����ɂ͉񕜗ʂ��n�����
        event UnityAction<float> OnEatFood;
        event UnityAction<float> OnDrinkWater;

        void Awake()
        {
            _nextAction = ActionType.SearchFood; // <- ������M���ăf�o�b�O�A����l��None
            CreateState();
        }

        void OnDisable()
        {
            OnEatFood = null;
            OnDrinkWater = null;
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
                throw new KeyNotFoundException("�J�ڐ�̃X�e�[�g�����݂��Ȃ�: " + type);
            }
        }

        void IStatusRegister.OnEatFoodRegister(UnityAction<float> action) => OnEatFood += action;
        void IStatusRegister.OnDrinkWaterRegister(UnityAction<float> action) => OnDrinkWater += action;

        void IStatusInvoker.OnEatFoodInvoke(float value) => OnEatFood.Invoke(value);
        void IStatusInvoker.OnDrinkWaterInvoke(float value) => OnDrinkWater.Invoke(value);
    }
}