using Unity.Burst;
using Unity.Entities;

namespace MiniGameECS
{
    /// <summary>
    /// �x�C�N���I�������Ɋe�X�|�i�[�̖��������s���̂ŏ������O���[�v�̍Ō�Ɏ��s
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
            // �e�X�|�i�[�𖳌���
            EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery query = manager.CreateEntityQuery(typeof(DebrisSpawnData));
            manager.SetComponentEnabled<DebrisSpawnData>(query, false);

            state.Enabled = false;
        }
    }
}
