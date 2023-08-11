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
            // �p�[�����m�C�Y
            PerlinNoise perlinNoise = GetComponent<PerlinNoise>();
            float[,] grid = perlinNoise.Create(_height, _width);
            // �Ή������I�u�W�F�N�g����
            TerrainGenerator terrain = GetComponent<TerrainGenerator>();
            Cell[,] field = terrain.Create(grid);
            // ����������z�u
            InitResourceSpawner resource = GetComponent<InitResourceSpawner>();
            resource.Spawn(field);
            // ����������z�u
            InitKinpatsuSpawner kinpatsu = GetComponent<InitKinpatsuSpawner>();
            kinpatsu.Spawn(field);

            return field;
        }
    }
}
