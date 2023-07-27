using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace BoidECS
{
    public struct ConfigData : IComponentData
    {
        public Entity Prefab;
        public float SpawnRadius;
        public int SpawnCount;
    }

    public class ConfigAuthoring : MonoBehaviour
    {
        public GameObject _prefab;
        public float _spawnRadius;
        public int _spawnCount;

        class Baker : Baker<ConfigAuthoring>
        {
            public override void Bake(ConfigAuthoring authoring)
            {
                // ???: 生成するエンティティなのになぜレンダリング可能フラグを立てているのか？
                AddComponent(GetEntity(TransformUsageFlags.Renderable), new ConfigData
                {
                    Prefab = GetEntity(authoring._prefab, TransformUsageFlags.Dynamic),
                    SpawnRadius = authoring._spawnRadius,
                    SpawnCount = authoring._spawnCount,
                });
            }
        }
    }
}
