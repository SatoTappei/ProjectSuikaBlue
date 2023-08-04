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
        // 生成されたときのランダムな拡散率
        public float Diffusion;
        // 速度と生存時間のばらつき
        public float SpeedVariation;
        public float LifeTimeVariation;
    }

    public struct DebrisSpawnData : IComponentData, IEnableableComponent
    {
        public float3 Pos;
        public float3 Dir;
    }

    public struct DebrisData : IComponentData
    {
        public const float InitMagicValue = 2;

        public float3 Dir;
        public float Speed;
        public float LifeTime;
        // 良い感じに飛ばすために使用する値
        public float MagicValue;
    }

    public struct RandomData : IComponentData
    {
        public uint Seed;
        public Random Random;
    }
}
