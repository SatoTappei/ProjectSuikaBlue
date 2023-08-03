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
    /// 生成は入力をトリガーした後に実行する必要がある
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
            // Prefabを持つConfigDataは唯一(にする必要がある)なのでこの方法で取得可能
            Entity spawner = SystemAPI.GetSingletonEntity<DebrisConfigData>();
            // SpawnDataが無効の場合(生成フラグが立っていない)場合
            if (!SystemAPI.IsComponentEnabled<DebrisSpawnData>(spawner)) return;

            RefRO<DebrisConfigData> configData = SystemAPI.GetComponentRO<DebrisConfigData>(spawner);
            RefRW<DebrisSpawnData> spawnData = SystemAPI.GetComponentRW<DebrisSpawnData>(spawner);

            // 生成＆配置
            NativeArray<Entity> entities = state.EntityManager.Instantiate(configData.ValueRO.Prefab,
                configData.ValueRO.Quantity, Allocator.Temp);
            foreach (Entity entity in entities)
            {
                RefRW<LocalTransform> transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                transform.ValueRW.Position = spawnData.ValueRO.Pos;

                // TODO:弾を飛ばす方向を弾側に持たせる必要がある。
            }

            // 次のフレームでも生成されるのを防ぐために無効化
            SystemAPI.SetComponentEnabled<DebrisSpawnData>(spawner, false);
        }
    }
}
