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
        bool _hasNeighbourCell;

        public WanderState(DataContext context) : base(context, StateType.Wander)
        {
            _move = new(context);
            _field = new(context);
        }

        Vector3 Position => Context.Transform.position;

        protected override void Enter()
        {
            _move.Reset();
            _field.SetOnCell();
            _hasNeighbourCell = SetNeighbourCell();
        }

        protected override void Exit()
        {          
        }

        protected override void Stay()
        {
            if (!_hasNeighbourCell) { ToEvaluateState(); return; }

            if (_move.OnNextCell)
            {
                // ���̃Z���ɓ��������^�C�~���O�ňړ��O�̃Z���̏�������
                _field.DeleteOnCell(_move.CurrentCellPos);

                ToEvaluateState();
            }
            else
            {
                _move.Move();
            }
        }

        bool SetNeighbourCell()
        {
            _move.CurrentCellPos = Position;

            // ����8�}�X�̃����_���ȃZ���Ɉړ�����
            Vector2Int index = FieldManager.Instance.WorldPosToGridIndex(Position);
            foreach (Vector2Int dir in Utility.EightDirections.OrderBy(_ => System.Guid.NewGuid()))
            {
                // �L�����N�^�[�����Ȃ��Z���̎擾
                if (!FieldManager.Instance.TryGetCell(index + dir, out Cell cell)) continue;
                if (!cell.IsEmpty) continue;

                // �o�H�̃Z���ƃL�����N�^�[�̍������Ⴄ�̂Ő����Ɉړ������邽�߂ɍ��������킹��
                _move.NextCellPos = cell.Pos;
                _move.NextCellPos.y = Context.Transform.position.y;
                // �ړ���̃Z����\�񂷂�
                _field.SetOnCell(_move.NextCellPos);

                _move.Modify();
                _move.Look();

                return true;
            }
            
            return false;
        }
    }
}