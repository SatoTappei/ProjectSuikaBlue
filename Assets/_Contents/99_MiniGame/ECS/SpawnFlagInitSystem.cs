using Unity.Burst;
using Unity.Entities;

namespace MiniGameECS
{
    /// <summary>
    /// ベイクが終わった後に各スポナーの無効化を行うので初期化グループの最後に実行
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast =true)]
    public partial struct SpawnFlagInitSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DebrisSpawnData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // 各スポナーを無効化
            EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery query = manager.CreateEntityQuery(typeof(DebrisSpawnData));
            manager.SetComponentEnabled<DebrisSpawnData>(query, false);

            state.Enabled = false;
        }
    }
}
