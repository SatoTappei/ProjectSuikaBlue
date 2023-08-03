using Unity.Entities;
using Unity.Mathematics;

namespace MiniGameECS
{
    public struct DebrisConfigData : IComponentData
    {
        public Entity Prefab;
        public int Quantity;
    }

    public struct DebrisSpawnData : IComponentData, IEnableableComponent
    {
        public float3 Pos;
        public float3 Dir;
    }
}
