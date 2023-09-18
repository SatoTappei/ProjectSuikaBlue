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

        public WanderState(DataContext context) : base(context, StateType.Wander)
        {
            _move = new(context);
            _field = new(context);
        }

        Vector3 Position => Context.Transform.position;

        protected override void Enter()
        {
            _move.Reset();
            _field.SetOnCell(_move.CurrentCellPos);
            SetNeighbourCell();
        }

        protected override void Exit()
        {
            _field.DeleteOnCell(_move.CurrentCellPos);
            _field.DeleteOnCell(_move.NextCellPos);
        }

        protected override void Stay()
        {
            if (_move.OnNextCell)
            {
                ToEvaluateState();
            }
            else
            {
                _move.Move();
            }
        }

        void SetNeighbourCell()
        {
            // ����8�}�X�̃����_���ȃZ���Ɉړ�����
            Vector2Int index = FieldManager.Instance.WorldPosToGridIndex(Position);
            foreach (Vector2Int dir in Utility.EightDirections.OrderBy(_ => System.Guid.NewGuid()))
            {
                // �L�����N�^�[�����Ȃ��Z���̎擾
                if (!FieldManager.Instance.TryGetCell(index + dir, out Cell cell)) continue;
                if (!cell.IsEmpty) continue;

                // �o�H�̃Z���ƃL�����N�^�[�̍������Ⴄ�̂Ő����Ɉړ������邽�߂ɍ��������킹��
                _move.NextCellPos = new Vector3(cell.Pos.x, Position.y, cell.Pos.z);
                // �ړ���̃Z����\�񂷂�
                _field.SetOnCell(_move.NextCellPos);

                _move.Modify();
                _move.Look();
                break;
            }
        }
    }
}