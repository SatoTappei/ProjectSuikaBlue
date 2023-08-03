using System.Collections.Generic;
using UnityEngine;

namespace MiniGameECS
{
    public enum EntityType
    {
        Debris
    }

    public class MonoToEcsTransfer : MonoBehaviour
    {
        public struct Data
        {
            public Vector3 Pos { get; set; }
            public Vector3 Dir { get; set; }
            public EntityType Type { get; set; }
        }

        public static MonoToEcsTransfer Instance { get; private set; }
        Queue<Data> _queue = new();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            // staticなのでキューのクリアが必要
            _queue.Clear();
        }

        /// <summary>
        /// MonoBehavior側で呼び出すメソッド
        /// 生成用のデータをキューに詰めていく
        /// </summary>
        public void AddData(Vector3 pos, Vector3 dir, EntityType type)
        {
            Data data = new Data
            {
                Pos = pos,
                Dir = dir,
                Type = type,
            };
            _queue.Enqueue(data);
        }

        /// <summary>
        /// ECS側で呼び出すメソッド
        /// 生成用のデータのキューから1つ取り出す
        /// </summary>
        /// <returns>データがある: true データがない: false</returns>
        public bool TryGetData(out Data data) => _queue.TryDequeue(out data);
    }
}
