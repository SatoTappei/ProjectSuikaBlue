using UnityEngine;
using UniRx;

namespace PSB.InGame
{
    /// <summary>
    /// 金髪/黒髪を生成するクラスはこのクラスを継承すること
    /// </summary>
    public class ActorSpawner : MonoBehaviour
    {
        static int _count;
        static Transform _parent;

        [SerializeField] InvalidActorHolder _holder;

        void Awake()
        {
            ReceiveMessage();
        }

        void ReceiveMessage()
        {
            // 死んだメッセージを受信した際に生成数を1減らす
            MessageBroker.Default.Receive<ActorDeathMessage>()
                .Where(msg => msg.Type == ActionType.Killed || msg.Type == ActionType.Senility)
                .Subscribe(_ => _count--).AddTo(this);
        }

        /// <summary>
        /// 生成して親の遺伝子を子に渡す初期化メソッドを呼んで返す。
        /// 第三引数ががnullの場合、遺伝子が無いので親無しになる。
        /// </summary>
        /// <returns>通常:初期化済みのActor 最大数生成済みの場合:null</returns>
        protected Actor InstantiateActor(ActorType type, Vector3 pos, uint? gene = null)
        {
            // 生成数を増やす
            _count++;

            if (_parent == null) _parent = new GameObject("ActorParent").transform;

            Actor actor = _holder.Rent(type);
            actor.transform.position = pos;
            actor.transform.rotation = Quaternion.identity;
            actor.transform.SetParent(_parent);
            actor.Init(gene);
            // ランダムな名前
            actor.name = Utility.GetRandomString();

            return actor;
        }

        /// <summary>
        /// 生成数を調べたい場合に呼ぶ
        /// </summary>
        /// <returns>生成可能:true 生成上限:false</returns>
        protected bool CheckSpawn()
        {
            if (_count >= InvalidActorHolder.PoolSize)
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