using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

namespace MyECS
{
    public partial struct ActorMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float elapsedTime = (float)SystemAPI.Time.ElapsedTime;
            // ‰~‰^“®
            ActorMoveJob job = new() { ElapsedTime = elapsedTime };
            job.ScheduleParallel();

            #region Job‚ð—p‚¢‚È‚¢ŽÀ‘•
            //foreach (var (actor, transform) in SystemAPI.Query<RefRO<ActorData>, RefRW<LocalTransform>>())
            //{
            //    // ‰~‰^“®‚³‚¹‚é
            //    float speed = actor.ValueRO.MoveSpeed;
            //    float cos = math.cos(elapsedTime * speed);
            //    float sin = math.sin(elapsedTime * speed);
            //    float radius = actor.ValueRO.MoveRadius;

            //    transform.ValueRW.Position = new float3(cos, 0, sin) * radius;
            //}
            #endregion
        }
    }

    [BurstCompile]
    partial struct ActorMoveJob : IJobEntity
    {
        public float ElapsedTime;

        void Execute(in ActorData actor, ref LocalTransform transform)
        {
            float speed = actor.MoveSpeed;
            float cos = math.cos(ElapsedTime * speed);
            float sin = math.sin(ElapsedTime * speed);
            float radius = actor.MoveRadius;

            transform.Position += new float3(cos, 0, sin) * radius;
        }
    }
}
