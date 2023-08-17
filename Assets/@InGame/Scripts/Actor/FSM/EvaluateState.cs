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
            // 黒板に書き込まれた状態に遷移する？
            // 黒板には値のみが書かれている、メソッドは無い。

            
            Log();
        }
    }
}
