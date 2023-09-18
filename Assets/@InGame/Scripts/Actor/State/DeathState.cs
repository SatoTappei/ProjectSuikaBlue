using UnityEngine;
using UniRx;

namespace PSB.InGame
{
    /// <summary>
    /// ���S�����ۂɃR���C�_�[�ƃ����_���[�𖳌����B
    /// �p�[�e�B�N���̔����A���O�ɕ\�����郁�b�Z�[�W�𑗐M����B
    /// </summary>
    public class DeathState : BaseState
    {
        readonly FieldModule _field;
        readonly ParticleType _particleType;
        readonly string _msg;

        public DeathState(DataContext context, StateType stateType, ParticleType particleType, string msg) 
            : base(context, stateType)
        {
            _field = new(context);
            _particleType = particleType;
            _msg = msg;
        }

        Vector3 Position => Context.Transform.position;

        protected override void Enter()
        {
            // ���S���ɏW���̕]���l�𑝉�������
            PublicBlackBoard.DeathRate += Context.Base.DeathGatherScore;
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            // ���S�����ۂɂ͂���ȏ�J�ڂ��Ȃ����߃Z���̗\������Ȃ��B

            Invalid();
            ResetValue();
            SendParticleMessage();
            SendLogMessage();
        }

        void Invalid()
        {
            // �����̃Z���̗\��������A�v�[���ɖ߂�
            _field.DeleteOnCell(Position);
            Context.ReturnToPool?.Invoke();
            // Enter�̃^�C�~���O�Ńv�[���ɖ߂��̂ŁA���Ɏ��o�����ۂ�Enter����n�܂�悤�Ƀ��Z�b�g����
            ResetStage();

            MessageBroker.Default.Publish(new ActorDeathMessage() { Type = Context.Type });
        }

        void ResetValue()
        {
            Context.Path.Clear();
            Context.Enemy = null;
            Context.Leader = null;
            Context.NextAction = ActionType.Wander;
            Context.IsLeader = false;
        }

        void SendParticleMessage()
        {
            MessageBroker.Default.Publish(new PlayParticleMessage()
            {
                Pos = Context.Transform.position,
                Type = _particleType,
            });
        }

        void SendLogMessage()
        {
            string r = Utility.ColorCodeRed;
            string g = Utility.ColorCodeGreen;
            string msg = $"<color={g}>{Context.Transform.name}</color>��<color={r}>{_msg}</color>�B���ՏI�A���ȁB";
            MessageBroker.Default.Publish(new EventLogMessage()
            {
                Message = msg,
            });
        }
    }
}
