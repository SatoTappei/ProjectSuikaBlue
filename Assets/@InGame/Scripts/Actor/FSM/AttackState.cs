using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PSB.InGame
{
    public class AttackState : BaseState
    {
        enum Stage
        {
            Move,
            Attack,
        }

        IBlackBoardForState _blackBoard;
        Stack<Vector3> _path = new();
        Transform _actor;
        Actor _enemy;
        Vector3 _currentCellPos;
        Vector3 _nextCellPos;
        Stage _stage;
        float _lerpProgress;
        float _speedModify = 1;
        // �U���Ԋu�̃^�C�}�[
        float _attackTimer;

        bool HasPath => _path.Count > 0;
        bool OnNextCell => _actor.position == _nextCellPos;

        public AttackState(IBlackBoardForState blackBoard) : base(StateType.Attack)
        {
            _blackBoard = blackBoard;
            _actor = _blackBoard.Transform;
        }

        protected override void Enter()
        {
            _enemy = _blackBoard.Enemy;
            _stage = Stage.Move;
            _attackTimer = 0;
            TryPathfinding();
            TryStepNextCell();
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            // �ߐڍU���ǂ�����H
            //  �G�Ɍ������ĕ����Ă���
            //  ���̏�œG��������U��

            // �U���͍����A�����A�������[�_�[�S������
            // �o�H���擾
            // �����Ɍ������ƈႤ�͎̂����ƈႢ����͓���
            //  �܂�A�o�H�T�����J��Ԃ��K�v������B

            // 1�Α��̏󋵂͂ǂ�����H

            // �o�H�������̂ŕ]���X�e�[�g�ɑJ��
            //if (!_hasPath) { ToEvaluateState(); return; }

            if (OnNextCell)
            {
                // �Ⴄ�X�e�[�g�Ȃ�J�ڂ���
                if(_blackBoard.NextState.Type != StateType.Attack)
                {
                    ToEvaluateState();
                }
            }

            switch (_stage)
            {
                case Stage.Move: MoveStage(); break;
                case Stage.Attack: AttackStage(); break;
            }
        }

        void MoveStage()
        {
            if (OnNextCell)
            {
                if (TryStepNextCell())
                {
                    // �o�H�̓r���̃Z���̏ꍇ�̏���
                }
                else
                {
                    _stage = Stage.Attack;
                    _attackTimer = 3.1f; // �U���Ԋu�𒴂���K���Ȓl�A�ŏ��Ɉꔭ����
                }
            }
            else
            {
                Move();
            }
        }

        void AttackStage()
        {
            _attackTimer += Time.deltaTime;
            if (_attackTimer > 3) // �K���Ȓl
            {
                _attackTimer = 0;
                float d =  Vector3.Distance(_actor.position, _enemy.transform.position);

                // 1�}�X�ƂȂ�Ȃ̂ł��̂��炢
                if (d <= 1.5f)
                {
                    _enemy.Damaged();
                }
                else
                {
                    _stage = Stage.Move;

                    TryPathfinding();
                    TryStepNextCell();
                }
            }
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
                Look();
                _lerpProgress = 0;

                return true;
            }

            _nextCellPos = _actor.position;

            return false;
        }

        bool TryPathfinding()
        {
            _path.Clear();
            return FieldManager.Instance.TryGetPath(_actor.position, _enemy.transform.position, out _path);
        }

        void Move()
        {
            _lerpProgress += Time.deltaTime * _blackBoard.Speed * _speedModify;
            _actor.position = Vector3.Lerp(_currentCellPos, _nextCellPos, _lerpProgress);
        }

        void Look()
        {
            Vector3 dir = _nextCellPos - _currentCellPos;
            _blackBoard.Model.rotation = Quaternion.LookRotation(dir, Vector3.up);
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
