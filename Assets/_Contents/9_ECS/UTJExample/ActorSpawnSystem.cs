using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

namespace MyECS
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct ActorSpawnSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ConfigData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // 指定した個数のエンティティを生成
            ConfigData config = SystemAPI.GetSingleton<ConfigData>();
            NativeArray<Entity> entities = state.EntityManager.Instantiate(config.Prefab, 
                config.SpawnCount, Allocator.Temp);

            // ランダムに分布
            Random seed = new Random(config.RandomSeed);
            foreach (Entity entity in entities)
            {
                RefRW<LocalTransform> transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                // ???: entityからActorDataコンポーネントを取得する処理のはずだが
                // このentityはGameObjectをEntityに変換しただけであり、ActorDataは付いていないはず…
                RefRW<ActorData> actor = SystemAPI.GetComponentRW<ActorData>(entity);

                // 範囲内のランダムな位置にランダムな角度で生成する
                float2 dir = seed.NextFloat2Direction();
                float3 pos = new float3(dir.x, 0, dir.y) * config.SpawnRadius * seed.NextFloat(1.0f);
                quaternion rot = quaternion.RotateY(seed.NextFloat(math.PI * 2));
                transform.ValueRW = LocalTransform.FromPositionRotation(pos, rot);
                // ???: ここでActorDataの値を設定しているということは既にコンポーネントが追加されている？
                actor.ValueRW = ActorData.Random(seed.NextUInt());
            }

            state.Enabled = false;
        }
    }
}