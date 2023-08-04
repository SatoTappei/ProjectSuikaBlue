using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

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
            RandomData randomData = SystemAPI.GetSingleton<RandomData>();
            foreach (Entity entity in entities)
            {
                // 生成位置をセット
                RefRW<LocalTransform> transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                transform.ValueRW.Position = spawnData.ValueRO.Pos;
                transform.ValueRW.Rotation = GetRandomRotation(ref randomData);
                // 飛ばすためのコンポーネントを追加
                float3 dirOffset = GetRandomDirection(ref randomData, configData.ValueRO.Diffusion);
                float spdOffset = GetRandomFloat(ref randomData, configData.ValueRO.SpeedVariation);
                float ltOffset = GetRandomFloat(ref randomData, configData.ValueRO.LifeTimeVariation);
                DebrisData debrisData = new()
                {
                    Dir = spawnData.ValueRO.Dir + dirOffset,
                    Speed = configData.ValueRO.Speed + spdOffset,
                    LifeTime = configData.ValueRO.LifeTime + ltOffset,
                    MagicValue = DebrisData.InitMagicValue + ltOffset, 
                };
                ecb.AddComponent(entity, debrisData);
            }

            // シード値の更新、循環させる
            if (randomData.Seed == uint.MaxValue) randomData.Seed = uint.MinValue;
            randomData.Seed++;
            SystemAPI.SetSingleton(randomData);

            ecb.Playback(state.EntityManager);
            ecb.Dispose();

            // 次のフレームでも生成されるのを防ぐために無効化
            SystemAPI.SetComponentEnabled<DebrisSpawnData>(spawner, false);
        }

        float3 GetRandomDirection(ref RandomData randomData, float diffusion)
        {
            float r = randomData.Random.NextFloat(-diffusion, diffusion);
            float3 dir = randomData.Random.NextFloat3Direction();
            return dir * r;
        }

        float GetRandomFloat(ref RandomData randomData, float variation)
        {
            return randomData.Random.NextFloat(-variation, variation);
        }

        quaternion GetRandomRotation(ref RandomData randomData)
        {
            float x = randomData.Random.NextFloat() * 360f;
            float y = randomData.Random.NextFloat() * 360f;
            float z = randomData.Random.NextFloat() * 360f;
            return quaternion.EulerXYZ(new float3(x, y, z));
        }
    }
}
