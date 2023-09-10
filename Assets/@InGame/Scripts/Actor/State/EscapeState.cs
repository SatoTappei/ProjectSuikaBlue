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
            // �т�����}�[�N�Đ�
            Context.PlayBikkuri();
        }

        protected override void Exit()
        {
            // �g���I������o�H������
            Context.Path.Clear();
        }

        protected override void Stay()
        {

        }
    }
}
