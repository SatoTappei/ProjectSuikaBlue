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
        // Query��OnUpdate�̃^�C�~���O�Ő�������ƁAOnCreate�ō쐬����Ƒ啝�ȍ��������o����B
        // �Ƃ����G���[���o�͂����
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
