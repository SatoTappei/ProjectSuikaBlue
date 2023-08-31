using UniRx.Toolkit;
using UnityEngine;

namespace PSB.InGame
{
    public class ActorPool : ObjectPool<Actor>
    {
        readonly Actor _origin;
        readonly Transform _parent;

        public ActorPool(Actor prefab, string name)
        {
            _parent = new GameObject(name).transform;

            // ï°êªå≥Çê∂ê¨
            _origin = Object.Instantiate(prefab);
            _origin.gameObject.SetActive(false);
            _origin.transform.SetParent(_parent);
        }

        protected override Actor CreateInstance()
        {
            return Object.Instantiate(_origin, _parent);
        }
    }
}