namespace PSB.InGame
{
    public class EvaluateState : BaseState
    {
        readonly DataContext _context;

        public EvaluateState(DataContext context) : base(context, StateType.Evaluate)
        {
            _context = context;
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
            BaseState state = _context.NextState;
            TryChangeState(state);
        }
    }
}
