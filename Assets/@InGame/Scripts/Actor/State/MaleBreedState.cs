using UnityEngine;

namespace PSB.InGame
{
    public class MaleBreedState : BaseState
    {
        enum Stage
        {
            Move,
            Mating,
        }

        // �Z����Scale�� 1 �̏ꍇ�ɁA�אڂ���Z�������C�L���X�g�Ŏ擾�ł��锼�a
        public const float NeighbourCellRadius = 1.45f;

        readonly MoveModule _move;
        readonly FieldModule _field;
        Stage _stage;
        float _matingTimer = 0;
        // ���͔��ߖT�̃Z���̕��������肷��̂� 8 �ŌŒ�
        Collider[] _detected = new Collider[8];
        // �e��s���̃t���O�A�ォ�珇�ɏ��������
        bool _firstStep; // �o�H�̃X�^�[�g�n�_���玟�̃Z���Ɉړ���
        bool _isMating;  // �����
        bool _completed; // �������

        public MaleBreedState(DataContext context) : base(context, StateType.MaleBreed) 
        {
            _move = new(context);
            _field = new(context);
        }

        protected override void Enter()
        {
            TryStepNextCell();
            _field.SetActorOnCell();
            _stage = Stage.Move;
            _matingTimer = 0;
            _firstStep = true;
            _isMating = false;
            _completed = false;
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

                    if (TryStepNextCell())
                    {
                        // �o�H�̓r���̃Z���̏ꍇ�̏���
                    }
                    else
                    {
                        _stage = Stage.Mating;
                    }
                }
                else
                {
                    _move.Move();
                }
            }
            // ���
            else if (_stage == Stage.Mating)
            {
                // �ɐB����ׂ̗Ɉړ������A���͔��ߖT�̎��ɑ΂��Č��
                if (!_isMating)
                {
                    _isMating = true;
                    // ������鎓�����Ȃ��ꍇ�͕]���X�e�[�g�ɖ߂�
                    if (!CallNeighbourFemale()) { ToEvaluateState(); return; }
                }

                // ������Ɍ���ɂ����鎞�Ԉȏ�o�߂����ꍇ�́A������s�Ƃ݂Ȃ��]���X�e�[�g�ɖ߂�
                if (_isMating) _matingTimer += Time.deltaTime;
                if (_matingTimer > Context.Base.MatingTime)
                {
                    // �A���ŔɐB�X�e�[�g�ɑJ�ڂ��Ă��Ȃ��悤�ɔɐB����50���ɂ���
                    Context.BreedingRate.Value = StatusBase.Max / 2;
                    ToEvaluateState();
                    return;
                }

                // ������͎���ł��W���ł����̃X�e�[�g�ɑJ�ڂ��Ȃ��B

                // ��������t���O���������ꍇ�͕]���X�e�[�g�ɖ߂�
                if (_completed)
                {
                    Context.BreedingRate.Value = 0;
                    ToEvaluateState();
                    return;
                }
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
        /// ���͔��ߖT�̔ɐB�X�e�[�g�̎��ɑ΂��Č�����Ăт�����B
        /// </summary>
        /// <returns>�������:true ������s:false</returns>
        bool CallNeighbourFemale()
        {
            Vector3 pos = Context.Transform.position;
            LayerMask layer = Context.Base.SightTargetLayer;
            int count = Physics.OverlapSphereNonAlloc(pos, NeighbourCellRadius, _detected, layer);
            if (count == 0) return false;

            foreach (Collider collider in _detected)
            {
                if (collider == null) break;
                if (collider.transform == Context.Transform) continue; // ������e��
                // ���ȊO��e��
                if (!(collider.TryGetComponent(out Actor female) && female.Sex == Sex.Female)) continue;
                // �Z���̏�񂪕K�v�Ȃ��̂ŁA���Ɨׂ荇���Ă��邩�ǂ����̔����������2��ōs���B
                Vector3 vec = Context.Transform.position - female.transform.position;
                float sq = NeighbourCellRadius * NeighbourCellRadius;
                if (Vector3.SqrMagnitude(vec) > sq) continue;
                // �����ɐB�X�e�[�g�̏ꍇ�̂�
                if (female.State != StateType.FemaleBreed) continue;

                // �q�����Y�񂾏ꍇ�͔ɐB�����t���O�𗧂Ă�R�[���o�b�N���Ăяo���Ă��炤�B
                female.SpawnChild(Context.Gene, () => _completed = true);
                return true;
            }

            return false;
        }
    }
}