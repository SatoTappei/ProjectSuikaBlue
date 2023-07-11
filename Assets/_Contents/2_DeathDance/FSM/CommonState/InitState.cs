namespace FSM
{
    /// <summary>
    /// �L�����N�^�[���������ꂽ�ۂ̏������
    /// �{�X�ƓG�ŋ��ʂ̏��
    /// </summary>
    public class InitState : EnemyStateBase
    {
        CommonLayerBlackBoard _blackBoard;

        public InitState(EnemyStateType type, CommonLayerBlackBoard blackBoard) : base(type)
        {
            _blackBoard = blackBoard;
        }

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            // ��������Ă��炱�̏�Ԃŏ��X�̏����ݒ���I������Ƀv���C���[��������ԂɑJ�ڂ���
            TryChangeState(_blackBoard[EnemyStateType.PlayerUndetected]);
        }
    }
}
