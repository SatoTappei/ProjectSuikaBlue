using System.Linq;
using System.Collections.Generic;
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
        Stack<Vector3> _path;
        Vector3 _currentCellPos;
        Vector3 _nextCellPos;
        float _lerpProgress;
        float _speedModify = 1;

        bool OnNextCell => _actor.position == _nextCellPos;

        public WanderState(IBlackBoardForState blackBoard) : base(StateType.Wander)
        {
            _blackBoard = blackBoard;
            _actor = blackBoard.Transform;
            _path = new();
        }

        protected override void Enter()
        {
            _lerpProgress = 0;

            // ����Ȃ�8�����͂܂ꂽ�ʒu�ɑ��݂��邱�Ƃ�����
            TryPathfinding();
            TryStepNextCell();
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

        bool TryPathfinding()
        {
            _path.Clear();

            // ����8�}�X�̃����_���ȃZ���Ɉړ�����
            Vector3 pos = _blackBoard.Transform.position;
            Vector2Int index = FieldManager.Instance.WorldPosToGridIndex(pos);
            foreach (Vector2Int dir in Utility.EightDirections.OrderBy(_ => System.Guid.NewGuid()))
            {
                Vector2Int neighbourIndex = index + dir;
                if (FieldManager.Instance.TryGetPath(index, neighbourIndex, out _path))
                {
                    return true;
                }
            }

            return false;
        }

        void ToEvaluateState() => TryChangeState(_blackBoard.EvaluateState);

        /// <summary>
        /// ���݂̃Z���̈ʒu�����g�̈ʒu�ōX�V����B
        /// ���̃Z���̈ʒu������Ύ��̃Z���̈ʒu�A�Ȃ���Ύ��g�̈ʒu�ōX�V����B
        /// </summary>
        /// <returns>���̃Z��������:true ���̃Z��������(�ړI�n�ɓ���):false</returns>
        bool TryStepNextCell()
        {
            _currentCellPos = _actor.position;

            if (_path.TryPop(out _nextCellPos))
            {
                // �o�H�̃Z���ƃL�����N�^�[�̍������Ⴄ�̂Ő����Ɉړ������邽�߂ɍ��������킹��
                _nextCellPos.y = _actor.position.y;
                Modify();
                _lerpProgress = 0;

                return true;
            }

            _nextCellPos = _actor.position;

            return false;
        }

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
    }
}