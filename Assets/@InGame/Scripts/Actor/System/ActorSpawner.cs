using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// 金髪/黒髪を生成するクラスはこのクラスを継承すること
    /// </summary>
    public class ActorSpawner : MonoBehaviour
    {
        const int Max = 50;
        static int _count;
        static Transform _parent;

        void Awake()
        {
            ReceiveMessage();
        }

        void ReceiveMessage()
        {
            ActorDeathReceiver receiver = new(_ => _count--, gameObject);
        }

        /// <summary>
        /// 生成して親の遺伝子を子に渡す初期化メソッドを呼んで返す。
        /// 第三引数ががnullの場合、遺伝子が無いので親無しになる。
        /// </summary>
        /// <returns>通常:初期化済みのActor 最大数生成済みの場合:null</returns>
        protected Actor InstantiateActor(Actor prefab, Vector3 pos, uint? gene = null)
        {
            // 生成数を数える
            _count++;

            if (_parent == null) _parent = new GameObject("ActorParent").transform;

            Actor actor = Instantiate(prefab, pos, Quaternion.identity, _parent);
            actor.Init(gene);
            // ランダムな名前
            actor.gameObject.name = Utility.GetRandomString();

            return actor;
        }

        protected bool Check()
        {
            if (_count >= Max)
            {
                Debug.LogWarning("キャラクターの生成数が最大に達しているので繁殖不可能");
                return false;
            }

            return true;
        }

        void OnDestroy()
        {
            if (_parent != null) Destroy(_parent);
            _count = 0;
        }
    }
}