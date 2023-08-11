using UnityEngine;

namespace Actor
{
    /// <summary>
    /// 金髪/黒髪を生成するクラスはこのクラスを継承すること
    /// </summary>
    public class ActorSpawner : MonoBehaviour
    {
        static Transform _parent;

        /// <summary>
        /// 生成して初期化メソッドを呼んで返す
        /// </summary>
        /// <returns>初期化済みのActor</returns>
        protected Actor InstantiateActor(Actor prefab, Vector3 pos, ActorType type)
        {
            Actor actor = Instantiate(prefab, pos, Quaternion.identity);
            if (_parent == null) _parent = new GameObject("ActorParent").transform;
            actor.transform.SetParent(_parent);
            actor.Init(type);

            return actor;
        }

        void OnDestroy()
        {
            if (_parent != null) Destroy(_parent);
        }
    }
}
