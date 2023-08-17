namespace PSB.InGame
{
    public class IdleState : BaseState
    {
        IBlackBoardForState _blackBoard;

        public IdleState(IBlackBoardForState blackBoard) : base(StateType.Idle)
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
            Log();
        }
    }
}