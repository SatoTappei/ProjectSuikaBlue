using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

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
            RandomData randomData = SystemAPI.GetSingleton<RandomData>();
            foreach (Entity entity in entities)
            {
                // �����ʒu���Z�b�g
                RefRW<LocalTransform> transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                transform.ValueRW.Position = spawnData.ValueRO.Pos;
                transform.ValueRW.Rotation = GetRandomRotation(ref randomData);
                // ��΂����߂̃R���|�[�l���g��ǉ�
                float3 dirOffset = GetRandomDirection(ref randomData, configData.ValueRO.Diffusion);
                float spdOffset = GetRandomFloat(ref randomData, configData.ValueRO.SpeedVariation);
                float ltOffset = GetRandomFloat(ref randomData, configData.ValueRO.LifeTimeVariation);
                DebrisData debrisData = new()
                {
                    Dir = spawnData.ValueRO.Dir + dirOffset,
                    Speed = configData.ValueRO.Speed + spdOffset,
                    LifeTime = configData.ValueRO.LifeTime + ltOffset,
                    MagicValue = DebrisData.InitMagicValue + ltOffset, 
                };
                ecb.AddComponent(entity, debrisData);
            }

            // �V�[�h�l�̍X�V�A�z������
            if (randomData.Seed == uint.MaxValue) randomData.Seed = uint.MinValue;
            randomData.Seed++;
            SystemAPI.SetSingleton(randomData);

            ecb.Playback(state.EntityManager);
            ecb.Dispose();

            // ���̃t���[���ł����������̂�h�����߂ɖ�����
            SystemAPI.SetComponentEnabled<DebrisSpawnData>(spawner, false);
        }

        float3 GetRandomDirection(ref RandomData randomData, float diffusion)
        {
            float r = randomData.Random.NextFloat(-diffusion, diffusion);
            float3 dir = randomData.Random.NextFloat3Direction();
            return dir * r;
        }

        float GetRandomFloat(ref RandomData randomData, float variation)
        {
            return randomData.Random.NextFloat(-variation, variation);
        }

        quaternion GetRandomRotation(ref RandomData randomData)
        {
            float x = randomData.Random.NextFloat() * 360f;
            float y = randomData.Random.NextFloat() * 360f;
            float z = randomData.Random.NextFloat() * 360f;
            return quaternion.EulerXYZ(new float3(x, y, z));
        }
    }
}
