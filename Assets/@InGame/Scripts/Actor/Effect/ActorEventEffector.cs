using UnityEngine;
using UniRx;

namespace PSB.InGame
{
    public class ActorEventEffector : MonoBehaviour
    {
        [SerializeField] Particle _killed;
        [SerializeField] Particle _senility;
        [SerializeField] Particle _spawn;
        [SerializeField] float _height = 0.6f;

        ParticlePool _killedPool;
        ParticlePool _senilityPool;
        ParticlePool _spawnPool;

        void Awake()
        {
            CreatePool();
            ReceiveMessage();
        }

        void OnDisable()
        {
            _killedPool.Dispose();
            _senilityPool.Dispose();
            _spawnPool.Dispose();
        }

        void CreatePool()
        {
            _killedPool = new(_killed, "KilledParticlePool");
            _senilityPool = new(_senility, "SenilityParticlePool");
            _spawnPool = new(_spawn, "SpawnParticlePool");
        }

        void ReceiveMessage()
        {
            // 死亡メッセージを受信
            MessageBroker.Default.Receive<ActorDeathMessage>()
                .Where(msg => msg.Type == ActionType.Killed || msg.Type == ActionType.Senility)
                .Subscribe(PlayDeathParticle).AddTo(this);
            // 生成メッセージを受信
            MessageBroker.Default.Receive<ActorSpawnMessage>().Subscribe(PlaySpawnParticle).AddTo(this);
        }

        void PlayDeathParticle(ActorDeathMessage msg)
        {
            Particle particle = msg.Type == ActionType.Killed ? _killedPool.Rent() : _senilityPool.Rent();
            particle.transform.position = new Vector3(msg.Pos.x, _height, msg.Pos.z);
        }

        void PlaySpawnParticle(ActorSpawnMessage msg)
        {
            Particle particle = _spawnPool.Rent();
            particle.transform.position = new Vector3(msg.Pos.x, _height, msg.Pos.z);
        }
    }
}
