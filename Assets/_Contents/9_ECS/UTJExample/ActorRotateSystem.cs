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
            // ��]������
            ActorRotateJob job = new() { ElapsedTime = elapsedTime };
            job.ScheduleParallel();

            #region Job��p���Ȃ�����
            //foreach (var (actor, transform) in SystemAPI.Query<RefRO<ActorData>, RefRW<LocalTransform>>())
            //{
            //    // �o�ߎ��ԂɊ�Â���Y���ŉ�]������
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