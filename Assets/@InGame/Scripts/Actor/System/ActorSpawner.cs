using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// 金髪/黒髪を生成するクラスはこのクラスを継承すること
    /// </summary>
    public class ActorSpawner : MonoBehaviour
    {
        static Transform _parent;

        /// <summary>
        /// 生成して親の遺伝子を子に渡す初期化メソッドを呼んで返す。
        /// 第三引数ががnullの場合、遺伝子が無いので親無しになる。
        /// </summary>
        /// <returns>初期化済みのActor</returns>
        protected Actor InstantiateActor(Actor prefab, Vector3 pos, uint? gene = null)
        {
            if (_parent == null) _parent = new GameObject("ActorParent").transform;

            Actor actor = Instantiate(prefab, pos, Quaternion.identity, _parent);
            actor.Init(gene);
            // ランダムな名前
            actor.gameObject.name = Utility.GetRandomString();

            return actor;
        }

        void OnDestroy()
        {
            if (_parent != null) Destroy(_parent);
        }
    }
}
