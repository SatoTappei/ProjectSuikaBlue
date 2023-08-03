using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace MiniGameECS
{
    /// <summary>
    /// “®‚©‚·ˆ—‚È‚Ì‚Å¶¬‚ÌŒã‚És‚¤
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

            // TODO: —Ç‚¢Š´‚¶‚É”ò‚ñ‚Å‚¢‚­Š´‚¶‚ÌŒvZ®‚É‚·‚é
            //       ”j•Ğ1‚Â‚¸‚Â‚ÌˆÚ“®ˆ—‚È‚Ì‚Å‚±‚±‚¾‚¯˜M‚ê‚Î—Ç‚¢
            float temp = data.Speed * Friction;
            data.Speed = temp < Threshold ? 0 : temp;
        }
    }
}