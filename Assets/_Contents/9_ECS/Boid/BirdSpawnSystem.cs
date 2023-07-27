using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe; // <- UnSafe�ȃR���N�V�����ւ̃A�N�Z�X�\
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Profiling;

namespace BoidECS
{
    [RequireMatchingQueriesForUpdate] // <- Query����̏ꍇ�͎��s���Ȃ�����
    [BurstCompile]
    public partial struct BirdSpawnSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new(Allocator.Temp);
            WorldUnmanaged world = state.World.Unmanaged;
            // ???:���b�N�A�b�v�e�[�u��(�s�x�v�Z����̂ł͂Ȃ��A�\�ߌv�Z���Ă����A�v�f�ւ̃A�N�Z�X
            //     �Ńf�[�^�����o����f�[�^�\���̂���)�Ǝ����悤�Ȃ��̂��Ǝv����B
            ComponentLookup<LocalToWorld> localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>();

            foreach ((RefRO<ConfigData> config, RefRO<LocalToWorld> localToWorld, Entity entity) in 
                SystemAPI.Query<RefRO<ConfigData>, RefRO<LocalToWorld>>().WithEntityAccess())
            {
                // ???:Rewindable(�����߂��\)��NativeArray���쐬����B�����߂��\�Ƃ́H
                //     �s���Ă��鏈�����̂͗N����(��)�̐��Ɠ����z����쐬���Ă��邾��
                NativeArray<Entity> flock = CollectionHelper.CreateNativeArray<Entity, 
                    RewindableAllocator>(config.ValueRO.SpawnCount, ref world.UpdateAllocator);

                // ��:�G���e�B�e�B�𐶐�����NativeArray�Ɋi�[���鏈��
                //    �����Ώۂ�Entity�Ɨv�f�Ƃ��Ēǉ����邽�߂�NativeArray�����Ȃ̂ŃV���v���ȏ���
                state.EntityManager.Instantiate(config.ValueRO.Prefab, flock);

                // ����������(��)�ɑ΂��ăW���u�����s
                // �S�Ă̐������I���܂Ŏ��ɍs���Ȃ��悤�AJob��ҋ@����
                SetLocalToWorldJob job = new SetLocalToWorldJob
                {
                    LocalToWorldLookup = localToWorldLookup,
                    Flock = flock,
                    Center = localToWorld.ValueRO.Position,
                    Radius = config.ValueRO.SpawnRadius,
                };
                state.Dependency = job.Schedule(config.ValueRO.SpawnCount, 64, state.Dependency);
                state.Dependency.Complete();

               // ???:�X�|�i�[��Entity�̔j��������ECB�ɒǉ����Ă��邪
               //     ������s���̂Ȃ�OnCreate�ŏ�������̂������I�ł́H
               ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }

    /// <summary>
    /// ��(��)�������_���ȕ������ʒu��ݒ肷��W���u
    /// </summary>
    [BurstCompile]
    struct SetLocalToWorldJob : IJobParallelFor // <- IJobEntity�ł͂Ȃ��AJobSystem�̕��񏈗��C���^�[�t�F�[�X
    {
        // ��:���̑������g�p����ƃl�C�e�B�u�R���e�i�̈��S�V�X�e��(������Ԃ̃o�O���o�Ȃ�)�𖳌�������B
        //    ���S�V�X�e���Ɉ���������悤�ȃf�[�^�ւ̃A�N�Z�X���\�ƂȂ邪�A���Ƃ���Job�̎��s����NativeArray��
        //    Dispose�����ꍇ�ɃG���[���b�Z�[�W��S���\�������AUnity���N���b�V������\��������B
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
                // �ʒu�A�X�P�[���A��]�̕ϊ���\�����\�b�h��p���ď�����
                Value = float4x4.TRS(pos, quaternion.LookRotationSafe(dir, math.up()), new float3(1.0f, 1.0f, 1.0f))
            };

            // ���b�N�A�b�v�e�[�u������Ώۂ̒�(��)�̃G���e�B�e�B���w�肵��LTW�����蓖�Ă�
            // �����ɃL�[���w�肵�Ēl��ύX���Ă���̂Ƃقړ����H
            LocalToWorldLookup[bird] = localToWorld;
        }
    }
}