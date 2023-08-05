using Unity.Entities;
using UnityEngine;

namespace MiniGameECS
{
    public class DebrisAuthoring : MonoBehaviour
    {
        [SerializeField] GameObject _prefab;
        [SerializeField] int _quantity;
        [SerializeField] float _speed;
        [SerializeField] float _lifeTime;
        [Header("�g�U��")]
        [SerializeField] float _diffusion;
        [Header("�΂��")]
        [SerializeField] float _speedVariation;
        [SerializeField] float _lifeTimeVariation;

        class Baker : Baker<DebrisAuthoring>
        {
            public override void Bake(DebrisAuthoring authoring)
            {
                DebrisConfigData configData = new()
                {
                    Prefab = GetEntity(authoring._prefab, TransformUsageFlags.Dynamic),
                    Quantity = authoring._quantity,
                    Speed = authoring._speed,
                    LifeTime = authoring._lifeTime,
                    Diffusion = authoring._diffusion,
                    SpeedVariation = authoring._speedVariation,
                    LifeTimeVariation = authoring._lifeTimeVariation,
                };
                SpawnData spawnData = new()
                {
                    Pos = default,
                    Dir = default,
                };
                SpawnFlagData flagData = new() { Flag = false };

                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, configData);
                AddComponent(entity, spawnData);
                AddComponent(entity, flagData);
            }
        }
    }
}
