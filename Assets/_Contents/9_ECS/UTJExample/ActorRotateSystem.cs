using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

namespace MyECS
{
    public partial struct ActorRotateSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float elapsedTime = (float)SystemAPI.Time.ElapsedTime;
            // 回転させる
            ActorRotateJob job = new() { ElapsedTime = elapsedTime };
            job.ScheduleParallel();

            #region Jobを用いない実装
            //foreach (var (actor, transform) in SystemAPI.Query<RefRO<ActorData>, RefRW<LocalTransform>>())
            //{
            //    // 経過時間に基づいてY軸で回転させる
            //    float value = actor.ValueRO.RotSpeed * elapsedTime;
            //    transform.ValueRW.Rotation = quaternion.Euler(math.up() * value);
            //}
            #endregion
        }
    }

    [BurstCompile]
    partial struct ActorRotateJob : IJobEntity
    {
        public float ElapsedTime;

        void Execute(in ActorData actor, ref LocalTransform transform)
        {
            float value = actor.RotSpeed * ElapsedTime;
            transform.Rotation = quaternion.Euler(math.up() * value);
        }
    }
}