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

            // �������𐶐�
            _origin = Object.Instantiate(prefab);
            _origin.gameObject.SetActive(false);
            _origin.transform.SetParent(_parent);
            // �v�[����n���ĔC�ӂ̃^�C�~���O�ŕԋp�o����悤�ɂ���
            _origin.GetComponent<DataContext>().ReturnToPool = () => ReturnToPool(_origin);
        }

        protected override Actor CreateInstance()
        {
            Actor actor = Object.Instantiate(_origin, _parent);
            // �v�[����n���ĔC�ӂ̃^�C�~���O�ŕԋp�o����悤�ɂ���
            actor.GetComponent<DataContext>().ReturnToPool = () => ReturnToPool(actor);
            return actor;
        }

        /// <summary>
        /// �v�[���ɖ߂��A�e�𐶐����̏�Ԃɖ߂�
        /// </summary>
        public void ReturnToPool(Actor actor)
        {
            Return(actor);
            actor.transform.SetParent(_parent);
        }
    }
}