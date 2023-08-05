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
        // �������ꂽ�Ƃ��̃����_���Ȋg�U��
        public float Diffusion;
        // ���x�Ɛ������Ԃ̂΂��
        public float SpeedVariation;
        public float LifeTimeVariation;
    }

    public struct TileConfigData : IComponentData
    {
        public Entity GrassPrefab;
        public Entity WallPrefab;
        public Entity SpawnPointPrefab;
    }

    public struct SpawnData : IComponentData
    {
        public float3 Pos;
        public float3 Dir;
    }

    public struct SpawnFlagData : IComponentData
    {
        public bool Flag;
    }

    public struct DebrisData : IComponentData
    {
        /// <summary>
        /// �ǂ������ɔ�Ԃ悤���߂����l
        /// </summary>
        public const float InitMagicValue = 2;

        public float3 Dir;
        public float Speed;
        public float LifeTime;
        // �ǂ������ɔ�΂����߂Ɏg�p����l
        public float MagicValue;
    }

    public struct GrassTileTag : IComponentData { }
    public struct WallTileTag : IComponentData { }
    public struct SpawnPointTileTag : IComponentData { }

    public struct RandomData : IComponentData
    {
        public uint Seed;
        public Random Random;
    }
}
