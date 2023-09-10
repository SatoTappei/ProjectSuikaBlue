using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UniRx;

namespace PSB.InGame
{
    public class AttackState : BaseState
    {
        enum Stage
        {
            Move,
            Attack,
        }

        readonly MoveModule _move;
        readonly FieldModule _field;
        Stage _stage;
        // ���͔��ߖT�̃Z���̕��������肷��̂� 8 �ŌŒ�
        Collider[] _detected = new Collider[8];
        bool _firstStep; // �o�H�̃X�^�[�g�n�_���玟�̃Z���Ɉړ���

        public AttackState(DataContext context) : base(context, StateType.Attack)
        {
            _move = new(context);
            _field = new(context);
        }

        protected override void Enter()
        {
            TryStepNextCell();
            _field.SetActorOnCell();
            _stage = Stage.Move;
            _firstStep = true;

            // �т�����}�[�N�Đ�
            Context.PlayBikkuri();
        }

        protected override void Exit()
        {
            // �g���I������o�H������
            Context.Path.Clear();
        }

        protected override void Stay()
        {
            // �ړ�
            if (_stage == Stage.Move)
            {
                if (_move.OnNextCell)
                {
                    // �o�H�̃X�^�[�g�n�_�͗\�񂳂�Ă���̂ŁA���̃Z���Ɉړ������ۂɏ���
                    // �S�ẴZ���ɑ΂��čs���ƁA�ʂ̃L�����N�^�[�ŗ\�񂵂��Z���܂ŏ����Ă��܂��B
                    if (_firstStep)
                    {
                        _firstStep = false;
                        _field.DeleteActorOnCell(_move.CurrentCellPos);
                    }

                    // �ʂ̃X�e�[�g���I������Ă����ꍇ�͑J�ڂ���
                    if (Context.ShouldChangeState(this)) { ToEvaluateState(); return; }

                    // ���݂̃Z���ɑ��̃L�����N�^�[�����Ȃ����A���͔��ߖT�ɓG�������ꍇ�͍U��
                    if (IsCellEmpty() && TryAttackNeighbour()) _stage = Stage.Attack;
                    else
                    {
                        if (!TryStepNextCell()) { ToEvaluateState(); return; }
                    }
                }
                else
                {
                    _move.Move();
                }
            }
            // �U��
            else if (_stage == Stage.Attack)
            {
                { ToEvaluateState(); return; }
            }
            // 1�Z���ړ� -> ���͔��ߖT�ɓG�����邩 -> ������U�����Ȃ������玟�̃Z����

            // �ߐڍU���ǂ�����H
            //  �G�Ɍ������ĕ����Ă���
            //  ���̏�œG��������U��

            // �U���͍����A�����A�������[�_�[�S������
            // �o�H���擾
            // �����Ɍ������ƈႤ�͎̂����ƈႢ����͓���
            //  �܂�A�o�H�T�����J��Ԃ��K�v������B

            // 1�Α��̏󋵂͂ǂ�����H
            // �G��|�����ۂɂ��]���X�e�[�g�ɑJ�ڂ���K�v������B
            //  ��1:1����������]���X�e�[�g�ɑJ�� <- �̗͂��������玩���œ�����͂��B
            //  �E�����ꍇ�͓G�����m�������̃X�e�[�g�ɑJ�ڂ��Ă��Ȃ��͂��B
            //  �K�v�Ȓl:�����X�R�A(�F�A�T�C�Y) <- ���̒l�ɂ���čU���͂��ς��.

            // �����Ɍ������čU�����Ȃ��� <- ���񂾏ꍇ�̓R���C�_�[�ƃ����_����������̂ő��v
            // �L�����N�^�[�̃v�[�����O������
        }

        /// <summary>
        /// �e�l������l�ɖ߂����ƂŁA���݂̃Z���̈ʒu�����g�̈ʒu�ōX�V����B
        /// ���̃Z���̈ʒu������Ύ��̃Z���̈ʒu�A�Ȃ���Ύ��g�̈ʒu�ōX�V����B
        /// </summary>
        /// <returns>���̃Z��������:true ���̃Z��������(�ړI�n�ɓ���):false</returns>
        bool TryStepNextCell()
        {
            _move.Reset();

            if (Context.Path.Count > 0)
            {
                // �o�H�̐擪(���̃Z��)����1���o��
                _move.NextCellPos = Context.Path[0];
                Context.Path.RemoveAt(0);
                // �o�H�̃Z���ƃL�����N�^�[�̍������Ⴄ�̂Ő����Ɉړ������邽�߂ɍ��������킹��
                _move.NextCellPos.y = Context.Transform.position.y;

                _move.Modify();
                _move.Look();
                return true;
            }
            else
            {
                _move.NextCellPos = Context.Transform.position;
                return false;
            }
        }

        /// <summary>
        /// ���݂̃Z���ɑ��̃L�����N�^�[�����邩�ǂ��������C�L���X�g�Ŕ��肷��
        /// �o�H�̓r���̃Z���͗\�񂳂�Ă��Ȃ��̂Ń��C�L���X�g��p����
        /// </summary>
        /// <returns>�����������Ȃ�:true �N������:false</returns>
        bool IsCellEmpty()
        {
            Vector3 pos = Context.Transform.position;
            float radius = 0.5f; // Scale��1�̏ꍇ��1�Z���̔��a
            LayerMask layer = Context.Base.SightTargetLayer;
            Physics.OverlapSphereNonAlloc(pos, radius, _detected, layer);

            // �z��̒��g�ŃR���|�[�l���g���擾�o��������1�̏ꍇ�͎����������Ȃ�
            return _detected.Where(c => c != null && c.TryGetComponent(out DataContext _)).Count() == 1;
        }

        /// <summary>
        /// ���͔��ߖT�̓G�ɑ΂��čU�������݂�
        /// </summary>
        /// <returns>�U������:true �G�����Ȃ�:false</returns>
        bool TryAttackNeighbour()
        {
            Array.Clear(_detected, 0, _detected.Length);

            Vector3 pos = Context.Transform.position;
            LayerMask layer = Context.Base.SightTargetLayer;
            int count = Physics.OverlapSphereNonAlloc(pos, Utility.NeighbourCellRadius, _detected, layer);
            if (count == 0) return false;

            foreach (Collider collider in _detected)
            {
                if (collider == null) break;
                // ���͔��ߖT�ɓG������ꍇ�͍U��
                if (collider.CompareTag(Context.EnemyTag)) Attack();

                return true;
            }

            return false;
        }

        void Attack()
        {
            Debug.Log("�U���I");
        }
    }
}
