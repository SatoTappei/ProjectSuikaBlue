namespace PSB.InGame
{
    public class EvaluateState : BaseState
    {
        BlackBoard _blackBoard;

        public EvaluateState(BlackBoard blackBoard) : base(StateType.Evaluate)
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
            // ���ɏ������܂ꂽ��ԂɑJ�ڂ���H
            // ���ɂ͒l�݂̂�������Ă���A���\�b�h�͖����B

            
            Log();
        }
    }
}
