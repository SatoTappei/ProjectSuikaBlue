namespace PSB.InGame
{
    public class EvaluateState : BaseState
    {
        DataContext _context;

        public EvaluateState(DataContext context) : base(context, StateType.Evaluate)
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
            // ���ɏ������܂ꂽ�X�e�[�g���AActor���̕]���Ō��肵�����̃X�e�[�g
            BaseState state = _context.NextState;
            TryChangeState(state);
        }
    }
}