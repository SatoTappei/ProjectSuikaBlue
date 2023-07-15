using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BT
{
    /// <summary>
    /// �r�w�C�r�A�c���[�Ŏg�p����m�[�h�̊��N���X
    /// �S�Ẵm�[�h�͂��̃N���X���p������K�v������
    /// </summary>
    public abstract class BehaviorTreeNode
    {
        public enum State
        {
            Running,
            Failure,
            Success,
        }

        /// <summary>
        /// ���O�Ƀm�[�h����\�����邩�̃t���O
        /// </summary>
        static readonly bool LogNodeName = false;

        State _currentState;
        string _nodeName;
        bool _isActive;

        public BehaviorTreeNode(string nodeName = "��������̃m�[�h")
        {
            _nodeName = nodeName;
        }

        public event UnityAction OnNodeEnter;
        public event UnityAction OnNodeExit;

        /// <summary>
        /// �ŏ���1���"OnEnter()��OnStay()"���Ă΂��
        /// OnStay()��Running�ȊO��Ԃ����ꍇ��"OnStay()��OnExit()"���Ă΂��
        /// </summary>
        public State Update()
        {
            if (!_isActive)
            {
                OnNodeEnter?.Invoke();
                _isActive = true;
                OnEnter();
            }

#if UNITY_EDITOR
            if(LogNodeName) Debug.Log(_nodeName + " �����s��");
#endif

            _currentState = OnStay();

            if (_currentState == State.Failure || _currentState == State.Success)
            {
                OnExit();
                _isActive = false;
                OnNodeExit?.Invoke();
            }

            return _currentState;
        }

        protected abstract void OnEnter();
        protected abstract State OnStay();
        protected abstract void OnExit();
    }
}