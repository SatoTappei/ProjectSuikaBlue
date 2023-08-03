using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace MiniGameECS
{
    /// <summary>
    /// �����͓��͂��g���K�[������Ɏ��s����K�v������
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
            // Prefab������ConfigData�͗B��(�ɂ���K�v������)�Ȃ̂ł��̕��@�Ŏ擾�\
            Entity spawner = SystemAPI.GetSingletonEntity<DebrisConfigData>();
            // SpawnData�������̏ꍇ(�����t���O�������Ă��Ȃ�)�ꍇ
            if (!SystemAPI.IsComponentEnabled<DebrisSpawnData>(spawner)) return;

            RefRO<DebrisConfigData> configData = SystemAPI.GetComponentRO<DebrisConfigData>(spawner);
            RefRO<DebrisSpawnData> spawnData = SystemAPI.GetComponentRO<DebrisSpawnData>(spawner);

            // �������z�u
            NativeArray<Entity> entities = state.EntityManager.Instantiate(configData.ValueRO.Prefab,
                configData.ValueRO.Quantity, Allocator.Temp);
            EntityCommandBuffer ecb = new(Allocator.Temp, PlaybackPolicy.MultiPlayback);
            foreach (Entity entity in entities)
            {
                // �����ʒu���Z�b�g
                RefRW<LocalTransform> transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                transform.ValueRW.Position = spawnData.ValueRO.Pos;
                // ��΂����߂̃R���|�[�l���g��ǉ�
                DebrisData debrisData = new()
                {
                    Dir = spawnData.ValueRO.Dir,
                    Speed = configData.ValueRO.Speed,
                    LifeTime = configData.ValueRO.LifeTime,
                };
                ecb.AddComponent(entity, debrisData);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();

            // ���̃t���[���ł����������̂�h�����߂ɖ�����
            SystemAPI.SetComponentEnabled<DebrisSpawnData>(spawner, false);
        }
    }
}
