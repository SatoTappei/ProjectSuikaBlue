using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace PSB.InGame
{
    public class GatherState : BaseState
    {
        readonly MoveModule _move;
        readonly FieldModule _field;
        readonly RuleModule _rule;

        bool _firstStep; // �o�H�̃X�^�[�g�n�_���玟�̃Z���Ɉړ���

        public GatherState(DataContext context) : base(context, StateType.Gather)
        {
            _move = new(context);
            _field = new(context);
            _rule = new(context);
        }

        List<Vector3> Path => Context.Path;
        Vector3 Position => Context.Transform.position;

        protected override void Enter()
        {
            _move.Reset();
            _move.TryStepNextCell();
            _field.SetOnCell(Position);
            _firstStep = true;

            PlayParticle();
        }

        protected override void Exit()
        {
            _field.DeletePathGoalOnCell();
            Path.Clear();
        }

        protected override void Stay()
        {
            if (_move.OnNextCell)
            {
                // �o�H�̃X�^�[�g�n�_�͗\�񂳂�Ă���̂ŁA���̃Z���Ɉړ������ۂɏ���
                // �S�ẴZ���ɑ΂��čs���ƁA�ʂ̃L�����N�^�[�ŗ\�񂵂��Z���܂ŏ����Ă��܂��B
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

        void PlayParticle()
        {
            MessageBroker.Default.Publish(new PlayParticleMessage()
            {
                Type = ParticleType.Gather,
                Pos = Position,
            });
        }
    }
}
