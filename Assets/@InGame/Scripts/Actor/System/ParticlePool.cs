using UniRx.Toolkit;
using UnityEngine;

namespace PSB.InGame
{
    public class ParticlePool : ObjectPool<Particle>
    {
        readonly Particle _origin;
        readonly Transform _parent;

        public ParticlePool(Particle prefab, string name)
        {
            _parent = new GameObject(name).transform;

            // ï°êªå≥Çê∂ê¨
            _origin = Object.Instantiate(prefab);
            _origin.Init(this);
            _origin.gameObject.SetActive(false);
            _origin.transform.SetParent(_parent);
        }

        protected override Particle CreateInstance()
        {
            Particle particle = Object.Instantiate(_origin, _parent);
            particle.Init(this);
            return particle;
        }
    }
}
