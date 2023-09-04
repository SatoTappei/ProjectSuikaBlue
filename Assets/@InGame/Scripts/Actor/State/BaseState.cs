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
        Wander,
        Breed,
        Killed,
        Senility,
        Attack,
        Escape,
        Gather,
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

        public BaseState(DataContext context, StateType type)
        {
            Context = context;
            Type = type;
        }

        protected DataContext Context { get; }
        public StateType Type { get; }

        /// <summary>
        /// 1度の呼び出しでステートの段階に応じてEnter() Stay() Exit()のうちどれか1つが実行される
        /// </summary>
        /// <returns>次の呼び出しで実行されるステート</returns>
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

        protected void ToEvaluateState() => TryChangeState(Context.EvaluateState);

        /// <summary>
        /// 次にプールから取り出した時にEnter以外から始まるのを防ぐ。
        /// プールに戻す際に呼ぶ必要がある。
        /// </summary>
        protected void ResetStage() => _stage = Stage.Enter;

        /// <summary>
        /// Enter()が呼ばれてかつ、ステートの遷移処理を呼んでいない場合のみ遷移可能
        /// </summary>
        /// <returns>ステートの遷移の可否</returns>
        public bool TryChangeState(BaseState nextState)
        {
            if (_stage == Stage.Enter)
            {
                Debug.LogWarning($"Enter()が呼ばれる前にステートを遷移することは不可能: {Type} 遷移先: {nextState}");
                return false;
            }
            else if (_stage == Stage.Exit)
            {
                Debug.LogWarning($"既に別のステートに遷移する処理が呼ばれています: {Type} 遷移先: {nextState}");
                return false;
            }

            _stage = Stage.Exit;
            _nextState = nextState;

            return true;
        }

        protected void Log()
        {
            string s = _nextState != null ? _nextState.ToString() : string.Empty;
            Debug.Log($"状態:{Type} ステージ:{_stage} 次:{s}");
        }
    }
}
