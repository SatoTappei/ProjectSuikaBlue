using Unity.Entities;

namespace MiniGameECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial class TriggerByMonoSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            // 入力を受け取って生成位置/方向をセットする
            if(MonoToEcsTransfer.Instance.TryGetData(out MonoToEcsTransfer.Data data))
            {
                // Prefabを持つConfigDataは唯一(にする必要がある)なのでこの方法で取得可能
                Entity spawner = SystemAPI.GetSingletonEntity<DebrisConfigData>(); // <- InvalidOperationException: GetSingleton() requires that exactly one entity exists that matches this query, but there are 0.
                RefRW<DebrisSpawnData> spawnData = SystemAPI.GetComponentRW<DebrisSpawnData>(spawner);
                spawnData.ValueRW.Pos = data.Pos;
                spawnData.ValueRW.Dir = data.Dir;
                SystemAPI.SetComponent(spawner, spawnData.ValueRW);
                // 生成フラグを立てて有効化
                RefRW<SpawnFlagData> flagData = SystemAPI.GetComponentRW<SpawnFlagData>(spawner);
                flagData.ValueRW.Flag = true;
                SystemAPI.SetComponent(spawner, spawnData.ValueRW);
            }
        }
    }
}