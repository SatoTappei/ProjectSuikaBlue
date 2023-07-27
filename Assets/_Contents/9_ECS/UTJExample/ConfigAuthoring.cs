using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace MyECS
{
    /// <summary>
    /// ���̃f�[�^�����ɐ�������
    /// </summary>
    public struct ConfigData : IComponentData
    {
        public Entity Prefab;
        public float SpawnRadius;
        public int SpawnCount;
        public uint RandomSeed;
    }

    /// <summary>
    /// �����p�̃f�[�^�����X�|�i�[�p�G���e�B�e�B���쐬���邾���Ő����͂��Ȃ�
    /// </summary>
    public class ConfigAuthoring : MonoBehaviour
    {
        public GameObject _prefab;
        public float _spawnRadius = 5.0f;
        public int _spawnCount = 100;
        public uint _randomSeed = 0;

        class Baker : Baker<ConfigAuthoring>
        {
            public override void Bake(ConfigAuthoring authoring)
            {
                ConfigData data = new()
                {
                    Prefab = GetEntity(authoring._prefab, TransformUsageFlags.Dynamic),
                    SpawnRadius = authoring._spawnRadius,
                    SpawnCount = authoring._spawnCount,
                    RandomSeed = authoring._randomSeed,
                };
                AddComponent(GetEntity(TransformUsageFlags.None), data);
            }
        }
    }
}