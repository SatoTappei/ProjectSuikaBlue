using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public class EscapeState : BaseState
    {
        readonly MoveModule _move;
        readonly FieldModule _field;
        readonly RuleModule _rule;
        
        bool _firstStep; // 経路のスタート地点から次のセルに移動中

        public EscapeState(DataContext context) : base(context, StateType.Escape)
        {
            _move = new(context);
            _field = new(context);
            _rule = new(context);
        }

        List<Vector3> Path => Context.Path;
        Vector3 Position => Context.Transform.position;
        DataContext Enemy { set => Context.Enemy = value; }

        protected override void Enter()
        {
            _move.Reset();
            _move.TryStepNextCell();
            _field.SetOnCell(Position);
            _firstStep = true;

            // びっくりマーク再生
            Context.PlayDiscoverEffect();
        }

        protected override void Exit()
        {
            Enemy = null;
            _field.DeletePathGoalOnCell();
            Path.Clear();
        }

        protected override void Stay()
        {
            if (_move.OnNextCell)
            {
                // 経路のスタート地点は予約されているので、次のセルに移動した際に消す
                // 全てのセルに対して行うと、別のキャラクターで予約したセルまで消してしまう。
                if (_firstStep)
                {
                    _firstStep = false;
                    _field.DeleteOnCell(_move.CurrentCellPos);
                }

                if (_rule.IsDead()) { ToEvaluateState(); return; }             
                if (!_move.TryStepNextCell()) { ToEvaluateState(); return; }
            }
            else
            {
                _move.Move();
            }
        }
    }
}
