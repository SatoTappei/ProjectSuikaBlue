namespace PSB.InGame
{
    public class SearchFoodState : BaseState
    {
        BlackBoard _blackBoard;

        public SearchFoodState(BlackBoard blackBoard) : base(StateType.Evaluate)
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