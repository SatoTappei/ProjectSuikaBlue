using Unity.Burst;
using Unity.Entities;

namespace MiniGameECS
{
    /// <summary>
    /// ����������A�������Ԃ�0�ɂȂ�������j�󂷂�
    /// </summary>
    [UpdateAfter(typeof(DebrisMoveSystem))]
    public partial struct DebrisDestroySystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new(Unity.Collections.Allocator.Temp, PlaybackPolicy.MultiPlayback);
            foreach ((RefRO<DebrisData> debrisData, Entity entity) in
                SystemAPI.Query<RefRO<DebrisData>>().WithEntityAccess())
            {
                if (debrisData.ValueRO.LifeTime < 0)
                {
                    ecb.DestroyEntity(entity);
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
