namespace PSB.InGame
{
    public class IdleState : BaseState
    {
        public IdleState(DataContext context) : base(context, StateType.Idle)
        {
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