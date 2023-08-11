using UnityEngine;

namespace PSB.InGame
{
    [RequireComponent(typeof(PerlinNoise))]
    [RequireComponent(typeof(TerrainGenerator))]
    [RequireComponent(typeof(InitResourceSpawner))]
    [RequireComponent(typeof(InitKinpatsuSpawner))]
    public class FieldBuilder : MonoBehaviour
    {
        [SerializeField] int _width = 50;
        [SerializeField] int _height = 50;

        public Cell[,] Build()
        {
            // パーリンノイズ
            PerlinNoise perlinNoise = GetComponent<PerlinNoise>();
            float[,] grid = perlinNoise.Create(_height, _width);
            // 対応したオブジェクト生成
            TerrainGenerator terrain = GetComponent<TerrainGenerator>();
            Cell[,] field = terrain.Create(grid);
            // 初期資源を配置
            InitResourceSpawner resource = GetComponent<InitResourceSpawner>();
            resource.Spawn(field);
            // 初期金髪を配置
            InitKinpatsuSpawner kinpatsu = GetComponent<InitKinpatsuSpawner>();
            kinpatsu.Spawn(field);

            return field;
        }
    }
}
