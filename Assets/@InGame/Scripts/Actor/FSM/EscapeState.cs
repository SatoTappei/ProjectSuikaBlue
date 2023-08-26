namespace PSB.InGame
{
    public class EscapeState : BaseState
    {
        IBlackBoardForState _blackBoard;

        public EscapeState(IBlackBoardForState blackBoard) : base(StateType.Escape)
        {
            _blackBoard = blackBoard;
        }

        protected override void Enter()
        {
            Log();
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
        }
    }
}
