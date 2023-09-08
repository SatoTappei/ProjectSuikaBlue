using UnityEngine;
using UnityEngine.Events;

namespace PSB.InGame
{
    /// <summary>
    /// �����̃Z���܂ňړ����A�ݒ肳�ꂽ���ʒl�����X�e�[�^�X�̑Ή������l�����X�ɉ񕜂���B
    /// �X�e�[�^�X�̃p�����[�^�����ʒl�������Ă��Ă��A���ʒl���̉񕜏��������s�����B
    /// </summary>
    public class SearchResourceState : BaseState
    {
        enum Stage
        {
            Move,
            Eat,
        }

        readonly MoveModule _move;
        readonly FieldModule _field;
        readonly EatParticleModule _particle;
        readonly ResourceType _resourceType;
        readonly UnityAction _stepEatAction;
        Stage _stage;
        float _healingProgress;
        // �o�H�̃X�^�[�g�n�_���玟�̃Z���Ɉړ������Ԃ̃t���O
        bool _firstStep;

        public SearchResourceState(DataContext context, StateType stateType, ResourceType resourceType, 
            UnityAction stepEatAction) : base(context, stateType)
        {
            _move = new(context);
            _field = new(context);
            _particle = new(context);
            _resourceType = resourceType;
            _stepEatAction = stepEatAction;
        }

        protected override void Enter()
        {
            _move.Reset();
            TryStepNextCell();
            _field.SetActorOnCell();
            _particle.Reset();
            _stage = Stage.Move;
            _healingProgress = 0;
            _firstStep = true;
        }

        protected override void Exit()
        {
            Context.Path.Clear();
        }

        protected override void Stay()
        {
            // �ړ�
            if (_stage == Stage.Move)
            {
                if (_move.OnNextCell)
                {
                    // �o�H�̃X�^�[�g�n�_�͗\�񂳂�Ă���̂Ŏ��̃Z���Ɉړ������ۂɏ���
                    // �S�ẴZ���ɑ΂��čs���ƕʂ̃L�����N�^�[�ŗ\�񂵂��Z���܂ŏ����Ă��܂��B
                    if (_firstStep)
                    {
                        _firstStep = false;
                        _field.DeleteActorOnCell(_move.CurrentCellPos);
                    }

                    // �ʂ̃X�e�[�g���I������Ă����ꍇ�͑J�ڂ���
                    if (Context.ShouldChangeState(this)) { ToEvaluateState(); return; }

                    if (TryStepNextCell())
                    {
                        // �o�H�̓r���̃Z���̏ꍇ�̏���
                    }
                    else
                    {
                        _stage = Stage.Eat; // �H�ׂ��Ԃ�
                    }
                }
                else
                {
                    _move.Move();
                }
            }
            // �H�ׂ�
            else if (_stage == Stage.Eat)
            {
                if (!StepEatProgress()) { ToEvaluateState(); return; }

                // ���Ԋu�Ńp�[�e�B�N��
                _particle.Update();
            }
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
                // �o�H�̖�������1���o��
                _move.NextCellPos = Context.Path[0];
                Context.Path.RemoveAt(0);
                // �o�H�̃Z���ƃL�����N�^�[�̍������Ⴄ�̂Ő����Ɉړ������邽�߂ɍ��������킹��
                _move.NextCellPos.y = Context.Transform.position.y;
                
                _move.Modify();
                _move.Look();

                return true;
            }

            _move.NextCellPos = Context.Transform.position;

            return false;
        }

        /// <summary>
        /// �񕜂̐i���x��i�߂�
        /// </summary>
        /// <returns>�񕜂̐i����:true �񕜂̐i�����񕜒l�ɒB����:false</returns>
        bool StepEatProgress()
        {
            _stepEatAction?.Invoke();
            _healingProgress += Time.deltaTime * Context.Base.HealingRate;
            return _healingProgress <= FieldManager.Instance.GetResourceHealingLimit(_resourceType);
        }
    }
}