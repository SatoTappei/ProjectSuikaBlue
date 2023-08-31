using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public class GatherState : BaseState
    {
        enum Stage
        {
            Move,
            Attack,
        }

        Stack<Vector3> _path = new();
        Transform _actor;
        Transform _leader;
        Vector3 _currentCellPos;
        Vector3 _nextCellPos;
        float _lerpProgress;
        float _speedModify = 1;

        bool OnNextCell => _actor.position == _nextCellPos;

        public GatherState(DataContext context) : base(context, StateType.Gather)
        {
            _actor = Context.Transform;
        }

        protected override void Enter()
        {
            _leader = Context.Leader.transform;
            TryPathfinding();
            TryStepNextCell();
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            if (OnNextCell)
            {
                if (TryStepNextCell())
                {
                    // �o�H�̓r���̃Z���̏ꍇ�̏���
                }
                else
                {
                    ToEvaluateState();
                }
            }
            else
            {
                Move();
            }
        }

        bool TryPathfinding()
        {
            _path.Clear();
            return FieldManager.Instance.TryGetPath(_actor.position, _leader.transform.position, out _path);
        }

        void ToEvaluateState() => TryChangeState(Context.EvaluateState);

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
                Look();
                _lerpProgress = 0;

                return true;
            }

            _nextCellPos = _actor.position;

            return false;
        }

        void Move()
        {
            //_lerpProgress += Time.deltaTime * Context.Speed * _speedModify;
            _actor.position = Vector3.Lerp(_currentCellPos, _nextCellPos, _lerpProgress);
        }

        void Look()
        {
            Vector3 dir = _nextCellPos - _currentCellPos;
            Context.Model.rotation = Quaternion.LookRotation(dir, Vector3.up);
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
