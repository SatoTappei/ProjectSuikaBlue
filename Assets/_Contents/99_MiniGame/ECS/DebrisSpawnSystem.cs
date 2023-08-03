using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

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
            RefRO<DebrisSpawnData> spawnData = SystemAPI.GetComponentRO<DebrisSpawnData>(spawner);

            // 生成＆配置
            NativeArray<Entity> entities = state.EntityManager.Instantiate(configData.ValueRO.Prefab,
                configData.ValueRO.Quantity, Allocator.Temp);
            EntityCommandBuffer ecb = new(Allocator.Temp, PlaybackPolicy.MultiPlayback);
            foreach (Entity entity in entities)
            {
                // 生成位置をセット
                RefRW<LocalTransform> transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                transform.ValueRW.Position = spawnData.ValueRO.Pos;
                // 飛ばすためのコンポーネントを追加
                DebrisData debrisData = new()
                {
                    Dir = spawnData.ValueRO.Dir,
                    Speed = configData.ValueRO.Speed,
                    LifeTime = configData.ValueRO.LifeTime,
                };
                ecb.AddComponent(entity, debrisData);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();

            // 次のフレームでも生成されるのを防ぐために無効化
            SystemAPI.SetComponentEnabled<DebrisSpawnData>(spawner, false);
        }
    }
}
