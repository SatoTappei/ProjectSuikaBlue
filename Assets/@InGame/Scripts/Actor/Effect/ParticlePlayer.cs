using UnityEngine;
using UniRx;

namespace PSB.InGame
{
    public enum ParticleType
    {
        Killed,
        Senility,
        Spawn,
        Eat,
        Mating,
        Damage,
        Gather,
        MatingReady,
    }

    public class ParticlePlayer : MonoBehaviour
    {
        [SerializeField] Particle _killed;
        [SerializeField] Particle _senility;
        [SerializeField] Particle _spawn;
        [SerializeField] Particle _eat;
        [SerializeField] Particle _mating;
        [SerializeField] Particle _damage;
        [SerializeField] Particle _gather;
        [SerializeField] Particle _matingReady;
        [SerializeField] float _height = 0.6f;

        ParticlePool _killedPool;
        ParticlePool _senilityPool;
        ParticlePool _spawnPool;
        ParticlePool _eatPool;
        ParticlePool _matingPool;
        ParticlePool _damagePool;
        ParticlePool _gatherPool;
        ParticlePool _matingReadyPool;

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
            _eatPool.Dispose();
            _matingPool.Dispose();
            _damagePool.Dispose();
            _gatherPool.Dispose();
            _matingReadyPool.Dispose();
        }

        void CreatePool()
        {
            _killedPool = new(_killed, "KilledParticlePool");
            _senilityPool = new(_senility, "SenilityParticlePool");
            _spawnPool = new(_spawn, "SpawnParticlePool");
            _eatPool = new(_eat, "EatParticlePool");
            _matingPool = new(_mating, "MatingParticlePool");
            _damagePool = new(_damage, "DamageParticlePool");
            _gatherPool = new(_gather, "GatherParticlePool");
            _matingReadyPool = new(_matingReady, "MatingReadyParticlePool");
        }

        void ReceiveMessage()
        {
            MessageBroker.Default.Receive<PlayParticleMessage>().Subscribe(Play).AddTo(this);
        }

        void Play(PlayParticleMessage msg)
        {
            Particle particle = null;
            if      (msg.Type == ParticleType.Killed) particle = _killedPool.Rent();
            else if (msg.Type == ParticleType.Senility) particle = _senilityPool.Rent();
            else if (msg.Type == ParticleType.Spawn) particle = _spawnPool.Rent();
            else if (msg.Type == ParticleType.Eat) particle = _eatPool.Rent();
            else if (msg.Type == ParticleType.Mating) particle = _matingPool.Rent();
            else if (msg.Type == ParticleType.Damage) particle = _damagePool.Rent();
            else if (msg.Type == ParticleType.Gather) particle = _gatherPool.Rent();
            else if (msg.Type == ParticleType.MatingReady) particle = _matingReadyPool.Rent();

            particle.transform.position = new Vector3(msg.Pos.x, _height, msg.Pos.z);
        }
    }
}
