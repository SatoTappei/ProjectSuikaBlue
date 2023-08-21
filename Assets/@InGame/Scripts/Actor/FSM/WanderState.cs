using System.Linq;
using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// �ׂ̃Z���Ɉړ����s���A�ׂ̃Z���ɓ���������]���X�e�[�g�ɖ߂�
    /// </summary>
    public class WanderState : BaseState
    {
        IBlackBoardForState _blackBoard;
        Transform _actor;
        Vector3 _currentCellPos;
        Vector3 _nextCellPos;
        float _lerpProgress;
        float _speedModify = 1;

        bool OnNextCell => _actor.position == _nextCellPos;

        public WanderState(IBlackBoardForState blackBoard) : base(StateType.Wander)
        {
            _blackBoard = blackBoard;
            _actor = blackBoard.Transform;
        }

        protected override void Enter()
        {
            _lerpProgress = 0;

            // ����Ȃ�8�����͂܂ꂽ�ʒu�ɑ��݂��邱�Ƃ�����
            SetTargetCell();
            Modify();
            Look();
        }

        protected override void Exit()
        {          
        }

        protected override void Stay()
        {
            // ���̃Z���̏�ɗ����ꍇ�͕]���X�e�[�g�ɖ߂�
            if (OnNextCell)
            {
                ToEvaluateState();
            }
            else
            {
                Move();
            }
        }

        bool SetTargetCell()
        {
            _currentCellPos = _actor.position;

            // ����8�}�X�̃����_���ȃZ���Ɉړ�����
            Vector3 pos = _blackBoard.Transform.position;
            Vector2Int index = FieldManager.Instance.WorldPosToGridIndex(pos);
            foreach (Vector2Int dir in Utility.EightDirections.OrderBy(_ => System.Guid.NewGuid()))
            {
                if (FieldManager.Instance.TryGetCell(index + dir, out Cell cell))
                {
                    if (!cell.IsWalkable) continue;

                    // �o�H�̃Z���ƃL�����N�^�[�̍������Ⴄ�̂Ő����Ɉړ������邽�߂ɍ��������킹��
                    _nextCellPos = cell.Pos;
                    _nextCellPos.y = _actor.position.y;
                    return true;
                }
            }
            
            return false;
        }

        void ToEvaluateState() => TryChangeState(_blackBoard.EvaluateState);

        void Move()
        {
            _lerpProgress += Time.deltaTime * _blackBoard.Speed * _speedModify;
            _actor.position = Vector3.Lerp(_currentCellPos, _nextCellPos, _lerpProgress);
        }

        /// <summary>
        /// �΂߈ړ��̑��x��␳����
        /// </summary>
        void Modify()
        {
            bool dx = Mathf.Approximately(_currentCellPos.x, _nextCellPos.x);
            bool dz = Mathf.Approximately(_currentCellPos.z, _nextCellPos.z);

            _speedModify = (dx || dz) ? 1 : 0.7f;
        }

        void Look()
        {
            Vector3 dir = _nextCellPos - _currentCellPos;
            _blackBoard.Model.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
    }
}