using Unity.Entities;
using Unity.Mathematics;

namespace MiniGameECS
{
    public struct DebrisConfigData : IComponentData
    {
        public Entity Prefab;
        public int Quantity;
        public float Speed;
        public float LifeTime;
    }

    public struct DebrisSpawnData : IComponentData, IEnableableComponent
    {
        public float3 Pos;
        public float3 Dir;
    }

    public struct DebrisData : IComponentData
    {
        public float3 Dir;
        public float Speed;
        public float LifeTime;
    }
}
