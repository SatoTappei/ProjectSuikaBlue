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
            // ���ɏ������܂ꂽ�X�e�[�g���AActor���̕]���Ō��肵�����̃X�e�[�g
            BaseState state = _context.NextState;
            TryChangeState(state);
        }
    }
}
