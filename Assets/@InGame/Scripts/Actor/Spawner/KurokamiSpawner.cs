using UniRx;
using UnityEngine;

namespace PSB.InGame
{
    public class KurokamiSpawner : ActorSpawner
    {
        [SerializeField] float _spawnHeight = 0.5f;

        void Awake()
        {
            MessageBroker.Default.Receive<KurokamiSpawnMessage>()
                .Subscribe(msg => Spawn(msg.Pos)).AddTo(this);
        }

        void Spawn(Vector3 pos)
        {
            pos += Vector3.up * _spawnHeight;
            // TODO:ñ{óàÇÕçïîØÇ‡à‚ì`éqÇìnÇ∑
            TryInstantiate(ActorType.Kurokami, pos, out Actor actor);
        }
    }
}
