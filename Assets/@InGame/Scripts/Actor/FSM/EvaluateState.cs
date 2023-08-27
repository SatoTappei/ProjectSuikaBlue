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
            // ���ɏ������܂ꂽ�X�e�[�g���AActor���̕]���Ō��肵�����̃X�e�[�g
            BaseState state = _blackBoard.NextState;
            TryChangeState(state);
        }
    }
}