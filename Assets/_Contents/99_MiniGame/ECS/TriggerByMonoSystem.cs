using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
                Entity spawner = SystemAPI.GetSingletonEntity<DebrisConfigData>();
                RefRW<DebrisSpawnData> spawnData = SystemAPI.GetComponentRW<DebrisSpawnData>(spawner);
                spawnData.ValueRW.Pos = data.Pos;
                spawnData.ValueRW.Dir = data.Dir;
                SystemAPI.SetComponent(spawner, spawnData.ValueRW);

                // 生成フラグを立てる
                SystemAPI.SetComponentEnabled<DebrisSpawnData>(spawner, true);
            }
        }
    }
}
