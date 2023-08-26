namespace PSB.InGame
{
    public class EvaluateState : BaseState
    {
        IBlackBoardForState _blackBoard;

        public EvaluateState(IBlackBoardForState blackBoard) : base(StateType.Evaluate)
        {
            _blackBoard = blackBoard;
        }

        protected override void Enter()
        {          
        }

        protected override void Exit()
        {           
        }

        protected override void Stay()
        {
            // 黒板に書き込まれたステートが、Actor側の評価で決定した次のステート
            BaseState state = _blackBoard.NextState;
            TryChangeState(state);
        }
    }
}
