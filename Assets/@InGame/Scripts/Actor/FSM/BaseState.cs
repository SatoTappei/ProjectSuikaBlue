using UnityEngine;

namespace PSB.InGame
{
    public enum StateType
    {
        Base,
        Idle,
        Evaluate,
        SearchFood,
        SearchWarter,
    }

    public abstract class BaseState
    {
        enum Stage
        {
            Enter,
            Stay,
            Exit,
        }

        Stage _stage;
        BaseState _nextState;

        public BaseState(StateType type)
        {
            Type = type;
        }

        public StateType Type { get; }

        /// <summary>
        /// 1�x�̌Ăяo���ŃX�e�[�g�̒i�K�ɉ�����Enter() Stay() Exit()�̂����ǂꂩ1�����s�����
        /// </summary>
        /// <returns>���̌Ăяo���Ŏ��s�����X�e�[�g</returns>
        public BaseState Update()
        {
            if (_stage == Stage.Enter)
            {
                Enter();
                _stage = Stage.Stay;
            }
            else if (_stage == Stage.Stay)
            {
                Stay();
            }
            else if (_stage == Stage.Exit)
            {
                Exit();
                _stage = Stage.Enter;

                return _nextState;
            }

            return this;
        }

        protected abstract void Enter();
        protected abstract void Stay();
        protected abstract void Exit();
        protected virtual void OnInvalid() { }

        /// <summary>
        /// Enter()���Ă΂�Ă��A�X�e�[�g�̑J�ڏ������Ă�ł��Ȃ��ꍇ�̂ݑJ�ډ\
        /// </summary>
        /// <returns>�X�e�[�g�̑J�ڂ̉�</returns>
        public bool TryChangeState(BaseState nextState)
        {
            if (_stage == Stage.Enter)
            {
                Debug.LogWarning("Enter()���Ă΂��O�ɃX�e�[�g��J�ڂ��邱�Ƃ͕s�\: �J�ڐ�: " + nextState);
                return false;
            }
            else if (_stage == Stage.Exit)
            {
                Debug.LogWarning("���ɕʂ̃X�e�[�g�ɑJ�ڂ��鏈�����Ă΂�Ă��܂�: �J�ڐ�: " + nextState);
                return false;
            }

            _stage = Stage.Exit;
            _nextState = nextState;

            return true;
        }

        protected void Log()
        {
            string s = _nextState != null ? _nextState.ToString() : string.Empty;
            Debug.Log($"���:{Type} �X�e�[�W:{_stage} ��:{s}");
        }
    }
}
