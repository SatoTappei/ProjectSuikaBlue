using System.Collections.Generic;
using UnityEngine;

namespace MiniGameECS
{
    public enum EntityType
    {
        // 演出用
        Debris,
        // 以下ダンジョンのタイル用
        Grass,
        Wall,
        SpawnPoint,
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
        Queue<Data> _debrisQueue = new();
        Queue<Data> _tileQueue = new();

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
            _debrisQueue.Clear();
            _tileQueue.Clear();
        }

        // MonoBehavior側で呼び出すメソッド
        public void AddData(Vector3 pos, Vector3 dir, EntityType type)
        {
            Data data = new Data
            {
                Pos = pos,
                Dir = dir,
                Type = type,
            };

            if (type == EntityType.Debris)
            {
                _debrisQueue.Enqueue(data);
            }
            else
            {
                _tileQueue.Enqueue(data);
            }
        }

        // ECS側で呼び出すメソッド
        public bool TryGetDebrisData(out Data data) => _debrisQueue.TryDequeue(out data);
        public bool TryGetTileData(out Data data) => _tileQueue.TryDequeue(out data);
    }
}
