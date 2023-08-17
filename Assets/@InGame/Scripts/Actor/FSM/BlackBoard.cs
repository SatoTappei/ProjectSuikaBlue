using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public class BlackBoard : MonoBehaviour
    {
        Dictionary<ActionType, BaseState> _actionStateDict;
        // �]���͑Ή�����s�����������ʂȏ�ԂȂ̂ŕʓr�ێ�����
        EvaluateState _evaluateState;

        // �X�e�[�g�����ǂݎ��
        public BaseState NextState => TryGetActionState(NextAction);
        public BaseState EvaluateState => _evaluateState;
        // Actor�����珑������
        public ActionType NextAction;

        void Awake()
        {
            CreateState();
        }

        void CreateState()
        {
            _evaluateState = new(this);

            _actionStateDict = new(4);
            _actionStateDict.Add(ActionType.SearchFood, new SearchFoodState(this));
        }

        BaseState TryGetActionState(ActionType type)
        {
            if (_actionStateDict.ContainsKey(type))
            {
                return _actionStateDict[type];
            }
            else
            {
                throw new KeyNotFoundException("�J�ڐ�̃X�e�[�g�����݂��Ȃ�: " + type);
            }
        }
    }
}