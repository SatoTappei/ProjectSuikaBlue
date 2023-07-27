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
            // �w�肵�����̃G���e�B�e�B�𐶐�
            ConfigData config = SystemAPI.GetSingleton<ConfigData>();
            NativeArray<Entity> entities = state.EntityManager.Instantiate(config.Prefab, 
                config.SpawnCount, Allocator.Temp);

            // �����_���ɕ��z
            Random seed = new Random(config.RandomSeed);
            foreach (Entity entity in entities)
            {
                RefRW<LocalTransform> transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                // ???: entity����ActorData�R���|�[�l���g���擾���鏈���̂͂�����
                // ����entity��GameObject��Entity�ɕϊ����������ł���AActorData�͕t���Ă��Ȃ��͂��c
                RefRW<ActorData> actor = SystemAPI.GetComponentRW<ActorData>(entity);

                // �͈͓��̃����_���Ȉʒu�Ƀ����_���Ȋp�x�Ő�������
                float2 dir = seed.NextFloat2Direction();
                float3 pos = new float3(dir.x, 0, dir.y) * config.SpawnRadius * seed.NextFloat(1.0f);
                quaternion rot = quaternion.RotateY(seed.NextFloat(math.PI * 2));
                transform.ValueRW = LocalTransform.FromPositionRotation(pos, rot);
                // ???: ������ActorData�̒l��ݒ肵�Ă���Ƃ������Ƃ͊��ɃR���|�[�l���g���ǉ�����Ă���H
                actor.ValueRW = ActorData.Random(seed.NextUInt());
            }

            state.Enabled = false;
        }
    }
}