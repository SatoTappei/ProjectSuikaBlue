using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace MiniGameECS
{
    /// <summary>
    /// 動かす処理なので生成の後に行う
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

            // TODO: 良い感じに飛んでいく感じの計算式にする
            //       破片1つずつの移動処理なのでここだけ弄れば良い
            float temp = data.Speed * Friction;
            data.Speed = temp < Threshold ? 0 : temp;
        }
    }
}