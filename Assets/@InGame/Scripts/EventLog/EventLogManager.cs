using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PSB.InGame
{
    public class EventLogManager : MonoBehaviour
    {
        [SerializeField] Transform _parent;
        [SerializeField] Transform _holder;

        void Awake()
        {
            SubscribeMessage();
        }

        void SubscribeMessage()
        {
            MessageBroker.Default.Receive<EventLogMessage>().Subscribe(OnMessageReceived).AddTo(this);
        }

        void OnMessageReceived(EventLogMessage msg)
        {
            if (_holder.childCount > 0)
            {
                Transform label = _holder.GetChild(0);
                label.SetParent(_parent);
                label.GetComponent<Text>().text = msg.Message;
            }
            else
            {
                Transform label = _parent.GetChild(0);
                label.SetAsLastSibling();
                label.GetComponent<Text>().text = msg.Message;
            }
        }
    }
}