using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace PSB.InGame
{
    /// <summary>
    /// キャラクターが死んだメッセージを受信し、コールバックを呼び出す
    /// 第二引数のGameObjectが非表示になったタイミングでコールバックを削除する
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

            // gameObject側が非表示になったタイミングでコールバックを削除する
            gameObject.OnDisableAsObservable().Subscribe(_ => OnDeath = null);
        }
    }
}
