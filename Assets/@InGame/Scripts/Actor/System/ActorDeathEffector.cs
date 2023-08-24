using UniRx;
using UnityEngine;

namespace PSB.InGame
{
    public class ActorDeathEffector : MonoBehaviour
    {
        const float LifeTime = 3.0f; // ìKìñÇ»íl

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
            // à¯êîÇÕKilledÇ‡ÇµÇ≠ÇÕSenilityÇ»ÇÃÇ≈2ë
            GameObject prefab = msg.Type == ActionType.Killed ? _killedPrefab : _senility;

            GameObject go = Instantiate(prefab);
            go.transform.position = new Vector3(msg.Pos.x, _spawnHeight, msg.Pos.z);
            Destroy(go, LifeTime);
        }
    }
}
