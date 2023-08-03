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
                };
                DebrisSpawnData spawnData = new()
                {
                    Pos = default,
                    Dir = default,
                };

                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, configData);
                AddComponent(entity, spawnData);
            }
        }
    }
}
