using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

namespace MiniGameECS
{
    /// <summary>
    /// ìÆÇ©Ç∑èàóùÇ»ÇÃÇ≈ê∂ê¨ÇÃå„Ç…çsÇ§
    /// </summary>
    [UpdateAfter(typeof(DebrisSpawnSystem))]
    public partial struct DebrisMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DebrisData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            DebrisMoveJob job = new()
            {
                DeltaTime = deltaTime,
            };
            job.ScheduleParallel();
        }
    }

    [BurstCompile]
    partial struct DebrisMoveJob : IJobEntity
    {
        const float Friction = 0.9f;
        const float Threshold = 0.02f;

        public float DeltaTime;

        void Execute(ref DebrisData data, ref LocalTransform transform)
        {
            transform.Position += data.Dir * data.Speed * DeltaTime;
            data.LifeTime -= DeltaTime;
            data.MagicValue -= DeltaTime;

            float temp = data.Speed * Friction * math.sin(data.MagicValue);
            data.Speed = temp < Threshold ? 0 : temp;
        }
    }
}