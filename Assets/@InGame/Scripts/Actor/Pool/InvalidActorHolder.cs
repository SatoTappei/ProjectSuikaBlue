using UniRx;
using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// キャラクターのプーリングをするが、インスタンスの生成だけ行い、初期化メソッドの呼び出しは行わない
    /// 各Spawnerはこのクラスからキャラクターを引き出し、初期化メソッドを呼ぶ。
    /// </summary>
    public class InvalidActorHolder : MonoBehaviour
    {
        /// <summary>
        /// プーリングする数
        /// </summary>
        public const int PoolSize = 50;

        [SerializeField] Actor _kinpatsuPrefab;
        [SerializeField] Actor _kurokamiPrefab;

        ActorPool _kinpatsuPool;
        ActorPool _kurokamiPool;

        void Awake()
        {
            CreatePool();
        }

        void CreatePool()
        {
            _kinpatsuPool = new(_kinpatsuPrefab, "KinpatsuPool");
            _kurokamiPool = new(_kurokamiPrefab, "KurokamiPool");

            // UniRxのオブプーの機能で非同期でプールの中身を生成していく
            _kinpatsuPool.PreloadAsync(PoolSize / 2, 1).Subscribe().AddTo(this);
            _kurokamiPool.PreloadAsync(PoolSize / 2, 1).Subscribe().AddTo(this);
        }

        public Actor Rent(ActorType type)
        {
            if (type == ActorType.Kinpatsu)
            {
                return _kinpatsuPool.Rent();
            }
            else if (type == ActorType.Kurokami)
            {
                return _kurokamiPool.Rent();
            }
            else if (type == ActorType.KinpatsuLeader)
            {
                // TODO:金髪プールから取得時、リーダー専用の処理が必要？
                return _kinpatsuPool.Rent();
            }
            else
            {
                throw new System.ArgumentException("ActorPoolにはNoneに対応汁キャラクターは無い");
            }
        }

        void OnDestroy()
        {
            _kinpatsuPool.Dispose();
            _kurokamiPool.Dispose();
            _kinpatsuPool = null;
            _kurokamiPool = null;
        }
    }
}
