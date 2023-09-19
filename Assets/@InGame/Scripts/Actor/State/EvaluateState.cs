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
            // 黒板に書き込まれたステートが、Actor側の評価で決定した次のステート
            BaseState state = _context.NextState;
            TryChangeState(state);
        }
    }
}