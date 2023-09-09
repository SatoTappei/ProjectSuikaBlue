using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace PSB.InGame
{
    public class KinpatsuCountView : MonoBehaviour
    {
        [SerializeField] Text _text;

        int _count;

        void Awake()
        {
            MessageBroker.Default.Receive<ActorSpawnMessage>().Where(msg => msg.Type == ActorType.Kinpatsu)
                .Subscribe(_ => _text.text = (++_count).ToString()).AddTo(this);
            MessageBroker.Default.Receive<ActorDeathMessage>().Where(msg => msg.Type == ActorType.Kinpatsu)
                .Subscribe(_ => _text.text = (--_count).ToString()).AddTo(this);
        }
    }
}