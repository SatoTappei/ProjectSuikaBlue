using UnityEngine;

namespace PSB.InGame
{
    [RequireComponent(typeof(PerlinNoise))]
    [RequireComponent(typeof(TerrainGenerator))]
    [RequireComponent(typeof(InitResourceSpawner))]
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

            return field;
        }
    }
}
