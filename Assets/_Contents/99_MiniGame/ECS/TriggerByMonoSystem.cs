using Unity.Entities;
using Unity.Transforms;

namespace MiniGameECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial class TriggerByMonoSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (MonoToEcsTransfer.Instance == null) return;

            // ���͂��󂯎���Đ����ʒu/�������Z�b�g����
            if(MonoToEcsTransfer.Instance.TryGetDebrisData(out MonoToEcsTransfer.Data debrisData))
            {
                // Prefab������ConfigData�͗B��(�ɂ���K�v������)�Ȃ̂ł��̕��@�Ŏ擾�\
                Entity spawner = SystemAPI.GetSingletonEntity<DebrisConfigData>(); // <- InvalidOperationException: GetSingleton() requires that exactly one entity exists that matches this query, but there are 0.
                RefRW<SpawnData> spawnData = SystemAPI.GetComponentRW<SpawnData>(spawner);
                spawnData.ValueRW.Pos = debrisData.Pos;
                spawnData.ValueRW.Dir = debrisData.Dir;
                SystemAPI.SetComponent(spawner, spawnData.ValueRW);
                // �����t���O�𗧂ĂėL����
                RefRW<SpawnFlagData> flagData = SystemAPI.GetComponentRW<SpawnFlagData>(spawner);
                flagData.ValueRW.Flag = true;
                SystemAPI.SetComponent(spawner, spawnData.ValueRW);
            }

            // ��������ׂ��^�C�����������`�F�b�N
            // TODO:�^�C���̃X�|�i�[�V�X�e��������Ă�����ɏ������ڂ�����
            while (MonoToEcsTransfer.Instance.TryGetTileData(out MonoToEcsTransfer.Data tileData))
            {
                SystemAPI.TryGetSingleton(out TileConfigData tileConfigData);

                // �Ή�����Entity��ݒ�
                Entity prefab;
                if (tileData.Type == EntityType.Grass) prefab = tileConfigData.GrassPrefab;
                else if (tileData.Type == EntityType.Wall) prefab = tileConfigData.WallPrefab;
                else if (tileData.Type == EntityType.SpawnPoint) prefab = tileConfigData.SpawnPointPrefab;
                else break;
                // ����
                Entity tile = EntityManager.Instantiate(prefab);
                RefRW<LocalTransform> transform = SystemAPI.GetComponentRW<LocalTransform>(tile);
                transform.ValueRW.Position = tileData.Pos;
            }
        }
    }
}