namespace PSB.InGame
{
    public class EscapeState : BaseState
    {
        readonly MoveModule _move;
        readonly FieldModule _field;
        bool _firstStep; // �o�H�̃X�^�[�g�n�_���玟�̃Z���Ɉړ���

        public EscapeState(DataContext context) : base(context, StateType.Escape)
        {
            _move = new(context);
            _field = new(context);
        }

        protected override void Enter()
        {
            TryStepNextCell();
            _field.SetOnCell();
            _firstStep = true;

            // �т�����}�[�N�Đ�
            Context.PlayDiscoverEffect();
        }

        protected override void Exit()
        {
            Context.Enemy = null;
            // �g���I������o�H������
            Context.Path.Clear();
        }

        protected override void Stay()
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

                if (!TryStepNextCell()) { ToEvaluateState(); return; }
                // �ʂ̃X�e�[�g���I������Ă����ꍇ�͑J�ڂ���
                if (Context.ShouldChangeState(this)) { ToEvaluateState(); return; }
            }
            else
            {
                _move.Move();
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
    }
}
