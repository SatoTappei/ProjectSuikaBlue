using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BT
{
    /// <summary>
    /// ビヘイビアツリーで使用するノードの基底クラス
    /// 全てのノードはこのクラスを継承する必要がある
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
        /// ログにノード名を表示するかのフラグ
        /// </summary>
        static readonly bool LogNodeName = false;

        State _currentState;
        string _nodeName;
        bool _isActive;

        public BehaviorTreeNode(string nodeName = "何かしらのノード")
        {
            _nodeName = nodeName;
        }

        public event UnityAction OnNodeEnter;
        public event UnityAction OnNodeExit;

        /// <summary>
        /// 最初の1回は"OnEnter()とOnStay()"が呼ばれる
        /// OnStay()がRunning以外を返した場合は"OnStay()とOnExit()"が呼ばれる
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
            if(LogNodeName) Debug.Log(_nodeName + " を実行中");
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