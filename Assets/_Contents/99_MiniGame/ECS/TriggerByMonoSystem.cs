using Unity.Entities;

namespace MiniGameECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial class TriggerByMonoSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            // ���͂��󂯎���Đ����ʒu/�������Z�b�g����
            if(MonoToEcsTransfer.Instance.TryGetData(out MonoToEcsTransfer.Data data))
            {
                // Prefab������ConfigData�͗B��(�ɂ���K�v������)�Ȃ̂ł��̕��@�Ŏ擾�\
                Entity spawner = SystemAPI.GetSingletonEntity<DebrisConfigData>(); // <- InvalidOperationException: GetSingleton() requires that exactly one entity exists that matches this query, but there are 0.
                RefRW<DebrisSpawnData> spawnData = SystemAPI.GetComponentRW<DebrisSpawnData>(spawner);
                spawnData.ValueRW.Pos = data.Pos;
                spawnData.ValueRW.Dir = data.Dir;
                SystemAPI.SetComponent(spawner, spawnData.ValueRW);
                // �����t���O�𗧂ĂėL����
                RefRW<SpawnFlagData> flagData = SystemAPI.GetComponentRW<SpawnFlagData>(spawner);
                flagData.ValueRW.Flag = true;
                SystemAPI.SetComponent(spawner, spawnData.ValueRW);
            }
        }
    }
}