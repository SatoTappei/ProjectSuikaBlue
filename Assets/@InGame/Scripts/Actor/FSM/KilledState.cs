using UniRx;
using UnityEngine;

namespace PSB.InGame
{
    public class KilledState : BaseState
    {
        IBlackBoardForState _blackBoard;

        public KilledState(IBlackBoardForState blackBoard) : base(StateType.Killed)
        {
            _blackBoard = blackBoard;
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
            _blackBoard.Transform.GetComponent<SphereCollider>().enabled = false;
            _blackBoard.Transform.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        }

        void SendMessage()
        {
            // ���񂾃��b�Z�[�W
            MessageBroker.Default.Publish(new ActorDeathMessage()
            {
                Pos = _blackBoard.Transform.position,
                Type = ActionType.Killed,
            });
        }
    }
}
