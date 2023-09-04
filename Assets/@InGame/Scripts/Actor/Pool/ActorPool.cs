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

            // 複製元を生成
            _origin = Object.Instantiate(prefab);
            _origin.gameObject.SetActive(false);
            _origin.transform.SetParent(_parent);
            // プールを渡して任意のタイミングで返却出来るようにする
            _origin.GetComponent<DataContext>().ReturnToPool = () => ReturnToPool(_origin);
        }

        protected override Actor CreateInstance()
        {
            Actor actor = Object.Instantiate(_origin, _parent);
            // プールを渡して任意のタイミングで返却出来るようにする
            actor.GetComponent<DataContext>().ReturnToPool = () => ReturnToPool(actor);
            return actor;
        }

        /// <summary>
        /// プールに戻し、親を生成時の状態に戻す
        /// </summary>
        public void ReturnToPool(Actor actor)
        {
            Return(actor);
            actor.transform.SetParent(_parent);
        }
    }
}