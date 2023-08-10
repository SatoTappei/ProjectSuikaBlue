using UnityEngine;

namespace Field
{
    [RequireComponent(typeof(PerlinNoise))]
    [RequireComponent(typeof(TerrainGenerator))]
    [RequireComponent(typeof(InitResourceSpawner))]
    public class FieldBuilder : MonoBehaviour
    {
        [SerializeField] int _width = 50;
        [SerializeField] int _height = 50;

        void Start()
        {
            Build();
        }

        void Build()
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

        }

        void SetCellHeight(float[,] grid, GameObject[,] field)
        {
            for(int i = 0; i < field.GetLength(0); i++)
            {
                for(int k = 0; k < field.GetLength(1); k++)
                {
                    SetHeight(grid[i, k], field[i, k]);
                }
            }
        }

        void SetHeight(float height, GameObject cell)
        {
            if (!cell.TryGetComponent(out IHeightProvider holder))
            {
                string message = "�Z���̍�����ݒ肷��̂ɕK�v��IHeightProvider����������Ă��Ȃ�";
                throw new System.NullReferenceException(message);
            }
            holder.SetHeight(height);
        }
    }
}
