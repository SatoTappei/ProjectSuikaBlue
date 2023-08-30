using UniRx;
using UnityEngine;

namespace PSB.InGame
{
    public class KilledState : BaseState
    {
        public KilledState(DataContext context) : base(context, StateType.Killed)
        {
        }

        protected override void Enter()
        {
            Invalid();
            SendMessage(); // <- ���̃��b�Z�[�W���R���g���[���[������M����Ǝ��S����M�ł���
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

        void SendMessage()
        {
            // ���񂾃��b�Z�[�W
            MessageBroker.Default.Publish(new ActorDeathMessage()
            {
                Pos = Context.Transform.position,
                Type = ActionType.Killed,
            });
            // ���O
            string r = Utility.ColorCodeRed;
            string g = Utility.ColorCodeGreen;
            MessageBroker.Default.Publish(new EventLogMessage()
            {
                Message = $"<color={g}>{Context.Transform.name}</color>��<color={r}>���S����</color>�B���ՏI�A���ȁB",
            });
        }
    }
}
