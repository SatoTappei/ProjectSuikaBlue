using Unity.Entities;
using UnityEngine;

namespace MiniGameECS
{
    public class TileAuthoring : MonoBehaviour
    {
        [SerializeField] GameObject _grassPrefab;
        [SerializeField] GameObject _wallPrefab;
        [SerializeField] GameObject _spawnPointPrefab;

        class Baker : Baker<TileAuthoring>
        {
            public override void Bake(TileAuthoring authoring)
            {
                TileConfigData configData = new()
                {
                    GrassPrefab = GetEntity(authoring._grassPrefab, TransformUsageFlags.Renderable),
                    WallPrefab = GetEntity(authoring._wallPrefab, TransformUsageFlags.Renderable),
                    SpawnPointPrefab = GetEntity(authoring._spawnPointPrefab, TransformUsageFlags.Renderable),

                };
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, configData);
            }
        }
    }
}