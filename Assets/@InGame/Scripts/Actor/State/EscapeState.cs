using UniRx;

namespace PSB.InGame
{
    public class EscapeState : BaseState
    {
        readonly MoveModule _move;
        readonly FieldModule _field;

        public EscapeState(DataContext context) : base(context, StateType.Escape)
        {
            _move = new(context);
            _field = new(context);
        }

        protected override void Enter()
        {
            // びっくりマーク再生
            Context.PlayBikkuri();
        }

        protected override void Exit()
        {
            // 使い終わった経路を消す
            Context.Path.Clear();
        }

        protected override void Stay()
        {

        }
    }
}
