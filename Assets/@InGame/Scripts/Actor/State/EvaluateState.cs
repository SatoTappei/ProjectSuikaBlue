using UnityEngine;

namespace PSB.InGame
{
    public class EvaluateState : BaseState
    {
        readonly DataContext _context;
        readonly FieldModule _field;

        public EvaluateState(DataContext context) : base(context, StateType.Evaluate)
        {
            _context = context;
            _field = new(context);
        }

        Vector3 Position => Context.Transform.position;

        protected override void Enter()
        {
            _field.SetOnCell(Position);
        }

        protected override void Exit()
        {
            _field.DeleteOnCell(Position);
        }

        protected override void Stay()
        {
            // ���ɏ������܂ꂽ�X�e�[�g���AActor���̕]���Ō��肵�����̃X�e�[�g
            BaseState state = _context.NextState;
            TryChangeState(state);
        }
    }
}