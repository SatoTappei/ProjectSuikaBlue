namespace PSB.InGame
{
    public class EscapeState : BaseState
    {
        public EscapeState(DataContext context) : base(context, StateType.Escape)
        {
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
