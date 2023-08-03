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
        // QueryはOnUpdateのタイミングで生成すると、OnCreateで作成すると大幅な高速化が出来る。
        // というエラーが出力される
        EntityQuery _query;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DebrisSpawnData>();

            _query = state.GetEntityQuery(typeof(DebrisSpawnData));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.SetComponentEnabled<DebrisSpawnData>(_query, false);
            state.Enabled = false;
        }
    }
}
