using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace PSB.InGame
{
    /// <summary>
    /// �L�����N�^�[�����񂾃��b�Z�[�W����M���A�R�[���o�b�N���Ăяo��
    /// ��������GameObject����\���ɂȂ����^�C�~���O�ŃR�[���o�b�N���폜����
    /// </summary>
    public class ActorDeathReceiver
    {
        public event UnityAction<ActorDeathMessage> OnDeath;

        public ActorDeathReceiver(UnityAction<ActorDeathMessage> action, GameObject gameObject)
        {
            OnDeath += action;

            MessageBroker.Default.Receive<ActorDeathMessage>()
                .Where(msg => msg.Type == ActionType.Killed || msg.Type == ActionType.Senility)
                .Subscribe(msg => OnDeath?.Invoke(msg)).AddTo(gameObject);

            // gameObject������\���ɂȂ����^�C�~���O�ŃR�[���o�b�N���폜����
            gameObject.OnDisableAsObservable().Subscribe(_ => OnDeath = null);
        }
    }
}
