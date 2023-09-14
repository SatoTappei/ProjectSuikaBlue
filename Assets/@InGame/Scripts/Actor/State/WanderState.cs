using System.Linq;
using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// �ׂ̃Z���Ɉړ����s���A�ׂ̃Z���ɓ���������]���X�e�[�g�ɖ߂�
    /// </summary>
    public class WanderState : BaseState
    {
        readonly MoveModule _move;
        readonly FieldModule _field;
        bool _hasNextCell;

        public WanderState(DataContext context) : base(context, StateType.Wander)
        {
            _move = new(context);
            _field = new(context);
        }

        protected override void Enter()
        {
            _move.Reset();
            _field.SetActorOnCell();
            _hasNextCell = SetTargetCell();
        }

        protected override void Exit()
        {          
        }

        protected override void Stay()
        {
            if (!_hasNextCell) { ToEvaluateState(); return; }

            if (_move.OnNextCell)
            {
                // ���̃Z���ɓ��������^�C�~���O�ňړ��O�̃Z���̏�������
                _field.DeleteActorOnCell(_move.CurrentCellPos);

                ToEvaluateState();
            }
            else
            {
                _move.Move();
            }
        }

        bool SetTargetCell()
        {
            _move.CurrentCellPos = Context.Transform.position;

            // ����8�}�X�̃����_���ȃZ���Ɉړ�����
            Vector3 pos = Context.Transform.position;
            Vector2Int index = FieldManager.Instance.WorldPosToGridIndex(pos);
            foreach (Vector2Int dir in Utility.EightDirections.OrderBy(_ => System.Guid.NewGuid()))
            {
                if (!FieldManager.Instance.IsWithinGrid(index + dir)) continue;

                if (FieldManager.Instance.TryGetCell(index + dir, out Cell cell))
                {
                    if (!cell.IsWalkable) continue;
                    if (!cell.IsEmpty) continue;

                    // �o�H�̃Z���ƃL�����N�^�[�̍������Ⴄ�̂Ő����Ɉړ������邽�߂ɍ��������킹��
                    _move.NextCellPos = cell.Pos;
                    _move.NextCellPos.y = Context.Transform.position.y;
                    // �ړ���̃Z����\�񂷂�
                    _field.SetActorOnCell(_move.NextCellPos);

                    _move.Modify();
                    _move.Look();

                    return true;
                }
            }
            
            return false;
        }
    }
}