using UniRx;
using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// ���S�����ۂɃR���C�_�[�ƃ����_���[�𖳌����B
    /// �p�[�e�B�N���̔����A���O�ɕ\�����郁�b�Z�[�W�𑗐M����B
    /// </summary>
    public class DeathState : BaseState
    {
        ParticleType _particleType;
        string _msg;

        public DeathState(DataContext context, StateType stateType, ParticleType particleType, string msg) 
            : base(context, stateType)
        {
            _particleType = particleType;
            _msg = msg;
        }

        protected override void Enter()
        {
            Invalid();
            SendParticleMessage(); 
            SendLogMessage();
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
        }

        void Invalid()
        {
            // �R���C�_�[�ƃ����_���[�𖳌������āA�N���b�N��h������ʂɔ�\��
            Context.Transform.GetComponent<SphereCollider>().enabled = false;
            Context.Transform.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
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
