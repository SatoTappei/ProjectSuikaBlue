using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe; // <- UnSafeなコレクションへのアクセス可能
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Profiling;

namespace BoidECS
{
    [RequireMatchingQueriesForUpdate] // <- Queryが空の場合は実行しない属性
    [BurstCompile]
    public partial struct BirdSpawnSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new(Allocator.Temp);
            WorldUnmanaged world = state.World.Unmanaged;
            // ???:ルックアップテーブル(都度計算するのではなく、予め計算しておき、要素へのアクセス
            //     でデータを取り出せるデータ構造のこと)と似たようなものだと思われる。
            ComponentLookup<LocalToWorld> localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>();

            foreach ((RefRO<ConfigData> config, RefRO<LocalToWorld> localToWorld, Entity entity) in 
                SystemAPI.Query<RefRO<ConfigData>, RefRO<LocalToWorld>>().WithEntityAccess())
            {
                // ???:Rewindable(巻き戻し可能)なNativeArrayを作成する。巻き戻し可能とは？
                //     行っている処理自体は湧く鳥(魚)の数と同じ配列を作成しているだけ
                NativeArray<Entity> flock = CollectionHelper.CreateNativeArray<Entity, 
                    RewindableAllocator>(config.ValueRO.SpawnCount, ref world.UpdateAllocator);

                // ★:エンティティを生成してNativeArrayに格納する処理
                //    生成対象のEntityと要素として追加するためのNativeArrayだけなのでシンプルな処理
                state.EntityManager.Instantiate(config.ValueRO.Prefab, flock);

                // 生成した鳥(魚)に対してジョブを実行
                // 全ての生成が終わるまで次に行かないよう、Jobを待機する
                SetLocalToWorldJob job = new SetLocalToWorldJob
                {
                    LocalToWorldLookup = localToWorldLookup,
                    Flock = flock,
                    Center = localToWorld.ValueRO.Position,
                    Radius = config.ValueRO.SpawnRadius,
                };
                state.Dependency = job.Schedule(config.ValueRO.SpawnCount, 64, state.Dependency);
                state.Dependency.Complete();

               // ???:スポナーのEntityの破棄処理をECBに追加しているが
               //     これを行うのならOnCreateで処理するのが合理的では？
               ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    /// <summary>
    /// 鳥(魚)をランダムな方向＆位置を設定するジョブ
    /// </summary>
    [BurstCompile]
    struct SetLocalToWorldJob : IJobParallelFor // <- IJobEntityではなく、JobSystemの並列処理インターフェース
    {
        // ★:この属性を使用するとネイティブコンテナの安全システム(競合状態のバグ検出など)を無効化する。
        //    安全システムに引っかかるようなデータへのアクセスが可能となるが、たとえばJobの実行中にNativeArrayを
        //    Disposeした場合にエラーメッセージを全く表示せず、Unityがクラッシュする可能性がある。
        [NativeDisableContainerSafetyRestriction]
        [NativeDisableParallelForRestriction]
        public ComponentLookup<LocalToWorld> LocalToWorldLookup;

        public NativeArray<Entity> Flock;
        public float3 Center;
        public float Radius;

        public void Execute(int index)
        {
            Entity bird = Flock[index];
            Random random = new Random((uint)(bird.Index + index + 1) * 0x9F6ABC1);
            float3 dir = math.normalizesafe(random.NextFloat3() - new float3(0.5f, 0.5f, 0.5f));
            float3 pos = Center + (dir * Radius);
            LocalToWorld localToWorld = new LocalToWorld
            {
                // 位置、スケール、回転の変換を表すメソッドを用いて初期化
                Value = float4x4.TRS(pos, quaternion.LookRotationSafe(dir, math.up()), new float3(1.0f, 1.0f, 1.0f))
            };

            // ルックアップテーブルから対象の鳥(魚)のエンティティを指定してLTWを割り当てる
            // 辞書にキーを指定して値を変更しているのとほぼ同じ？
            LocalToWorldLookup[bird] = localToWorld;
        }
    }
}