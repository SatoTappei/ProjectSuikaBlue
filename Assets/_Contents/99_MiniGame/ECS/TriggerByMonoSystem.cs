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

            // 入力を受け取って生成位置/方向をセットする
            if(MonoToEcsTransfer.Instance.TryGetDebrisData(out MonoToEcsTransfer.Data debrisData))
            {
                // Prefabを持つConfigDataは唯一(にする必要がある)なのでこの方法で取得可能
                Entity spawner = SystemAPI.GetSingletonEntity<DebrisConfigData>(); // <- InvalidOperationException: GetSingleton() requires that exactly one entity exists that matches this query, but there are 0.
                RefRW<SpawnData> spawnData = SystemAPI.GetComponentRW<SpawnData>(spawner);
                spawnData.ValueRW.Pos = debrisData.Pos;
                spawnData.ValueRW.Dir = debrisData.Dir;
                SystemAPI.SetComponent(spawner, spawnData.ValueRW);
                // 生成フラグを立てて有効化
                RefRW<SpawnFlagData> flagData = SystemAPI.GetComponentRW<SpawnFlagData>(spawner);
                flagData.ValueRW.Flag = true;
                SystemAPI.SetComponent(spawner, spawnData.ValueRW);
            }

            // 生成するべきタイルが無いかチェック
            // TODO:タイルのスポナーシステムを作ってそちらに処理を移したい
            while (MonoToEcsTransfer.Instance.TryGetTileData(out MonoToEcsTransfer.Data tileData))
            {
                SystemAPI.TryGetSingleton(out TileConfigData tileConfigData);

                // 対応するEntityを設定
                Entity prefab;
                if (tileData.Type == EntityType.Grass) prefab = tileConfigData.GrassPrefab;
                else if (tileData.Type == EntityType.Wall) prefab = tileConfigData.WallPrefab;
                else if (tileData.Type == EntityType.SpawnPoint) prefab = tileConfigData.SpawnPointPrefab;
                else break;
                // 生成
                Entity tile = EntityManager.Instantiate(prefab);
                RefRW<LocalTransform> transform = SystemAPI.GetComponentRW<LocalTransform>(tile);
                transform.ValueRW.Position = tileData.Pos;
            }
        }
    }
}