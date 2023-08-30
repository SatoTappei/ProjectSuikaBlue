using UnityEngine;

namespace PSB.InGame
{
    public class Particle : MonoBehaviour
    {
        [SerializeField] float _lifeTime = 3.0f;

        ParticlePool _pool;
        float Timer;

        public void Init(ParticlePool pool)
        {
            _pool = pool;
        }

        void Update()
        {
            Timer += Time.deltaTime;
            if (Timer > _lifeTime)
            {
                Timer = 0;
                _pool.Return(this);
            }
        }
    }
}
