using UniRx;
using UnityEngine;

namespace PSB.InGame
{
    public class ActorDeathEffector : MonoBehaviour
    {
        const float LifeTime = 3.0f; // �K���Ȓl

        [SerializeField] GameObject _killedPrefab;
        [SerializeField] GameObject _senility;
        [SerializeField] float _spawnHeight = 0.6f;

        void Awake()
        {
            MessageBroker.Default.Receive<ActorDeathMessage>()
                .Where(msg => msg.Type == ActionType.Killed || msg.Type == ActionType.Senility)
                .Subscribe(Execute).AddTo(this);
        }

        void Execute(ActorDeathMessage msg)
        {
            // ������Killed��������Senility�Ȃ̂�2��
            GameObject prefab = msg.Type == ActionType.Killed ? _killedPrefab : _senility;

            GameObject go = Instantiate(prefab);
            go.transform.position = new Vector3(msg.Pos.x, _spawnHeight, msg.Pos.z);
            Destroy(go, LifeTime);
        }
    }
}
