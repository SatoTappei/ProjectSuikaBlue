using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Burst;

namespace MiniGameECS
{
    /// <summary>
    /// �����͓��͂��g���K�[������Ɏ��s����K�v������
    /// </summary>
    [UpdateAfter(typeof(TriggerByMonoSystem))]
    public partial struct DebrisSpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DebrisConfigData>();
            state.RequireForUpdate<DebrisSpawnData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Prefab������ConfigData�͗B��(�ɂ���K�v������)�Ȃ̂ł��̕��@�Ŏ擾�\
            Entity spawner = SystemAPI.GetSingletonEntity<DebrisConfigData>();
            // SpawnData�������̏ꍇ(�����t���O�������Ă��Ȃ�)�ꍇ
            if (!SystemAPI.IsComponentEnabled<DebrisSpawnData>(spawner)) return;

            RefRO<DebrisConfigData> configData = SystemAPI.GetComponentRO<DebrisConfigData>(spawner);
            RefRW<DebrisSpawnData> spawnData = SystemAPI.GetComponentRW<DebrisSpawnData>(spawner);

            // �������z�u
            NativeArray<Entity> entities = state.EntityManager.Instantiate(configData.ValueRO.Prefab,
                configData.ValueRO.Quantity, Allocator.Temp);
            foreach (Entity entity in entities)
            {
                RefRW<LocalTransform> transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                transform.ValueRW.Position = spawnData.ValueRO.Pos;

                // TODO:�e���΂�������e���Ɏ�������K�v������B
            }

            // ���̃t���[���ł����������̂�h�����߂ɖ�����
            SystemAPI.SetComponentEnabled<DebrisSpawnData>(spawner, false);
        }
    }
}
