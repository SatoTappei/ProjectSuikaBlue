using System.Linq;
using UniRx;
using System.Collections.Generic;
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

        readonly MoveModule _move;
        readonly FieldModule _field;
        readonly DetectModule _detect;
        
        Stage _stage;
        bool _firstStep; // �o�H�̃X�^�[�g�n�_���玟�̃Z���Ɉړ���

        public AttackState(DataContext context) : base(context, StateType.Attack)
        {
            _move = new(context);
            _field = new(context);
            _detect = new(context);
        }

        Collider[] Detecetd => Context.Detected;
        List<Vector3> Path => Context.Path;
        string EnemyTag => Context.EnemyTag;
        int MeleeDamage => Context.Base.MeleeDamage;
        DataContext Enemy { set => Context.Enemy = value; }

        protected override void Enter()
        {
            _move.TryStepNextCell();
            _field.SetOnCell();
            _stage = Stage.Move;
            _firstStep = true;

            // �т�����}�[�N�Đ�
            Context.PlayDiscoverEffect();
        }

        protected override void Exit()
        {
            Enemy = null;
            // �g���I������o�H������
            Path.Clear();
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
                        _field.DeleteOnCell(_move.CurrentCellPos);
                    }

                    // �ʂ̃X�e�[�g���I������Ă����ꍇ�͑J�ڂ���
                    if (Context.ShouldChangeState(this)) { ToEvaluateState(); return; }

                    // ���݂̃Z���ɑ��̃L�����N�^�[�����Ȃ����A���͔��ߖT�ɓG�������ꍇ�͍U��
                    if (IsCellEmpty() && TryAttackNeighbour()) _stage = Stage.Attack;
                    // �o�H�̖��[�܂ŒH�蒅�����ꍇ�͑J��
                    else if (!_move.TryStepNextCell()) { ToEvaluateState(); return; }
                }
                else
                {
                    _move.Move();
                }
            }
            // �U��
            else if (_stage == Stage.Attack)
            {
                // Damage���\�b�h���Ăяo���^�C�~���O�Ńp�[�e�B�N�����Đ������̂�
                // �A�j���[�V�����̍Đ��������Ȃ��ꍇ�A���̂܂ܕ]���X�e�[�g�ɑJ��
                ToEvaluateState();
            }
        }

        /// <summary>
        /// ���݂̃Z���ɑ��̃L�����N�^�[�����邩�ǂ��������C�L���X�g�Ŕ��肷��
        /// �o�H�̓r���̃Z���͗\�񂳂�Ă��Ȃ��̂Ń��C�L���X�g��p����
        /// </summary>
        /// <returns>�����������Ȃ�:true �N������:false</returns>
        bool IsCellEmpty()
        {
            // ���a��Scale��1�̏ꍇ��1�Z���̔��a
            _detect.OverlapSphere(0.5f); 
            // �z��̒��g�ŃR���|�[�l���g���擾�o��������1�̏ꍇ�͎����������Ȃ�
            return Detecetd.Where(c => c != null && c.TryGetComponent(out DataContext _)).Count() == 1;
        }

        /// <summary>
        /// ���͔��ߖT�̓G�ɑ΂��čU�������݂�
        /// </summary>
        /// <returns>�U������:true �G�����Ȃ�:false</returns>
        bool TryAttackNeighbour()
        {
            _detect.OverlapSphere(Utility.NeighbourCellRadius);
            
            foreach (Collider collider in Detecetd)
            {
                if (collider == null) break;
                // ���͔��ߖT�ɓG������ꍇ�͍U��
                if (collider.CompareTag(EnemyTag) && TryAttack()) return true;
            }

            return false;
        }

        bool TryAttack()
        {
            if (Context.Enemy == null) return false;

            Context.Enemy.Damage(MeleeDamage);
            return true;
        }
    }
}
