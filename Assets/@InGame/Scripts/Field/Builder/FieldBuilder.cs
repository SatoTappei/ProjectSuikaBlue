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

        // TODO:アドレッサブルを開放しないとリークするため一時的に
        Cell[,] _field = new Cell[50,50];

        void Start()
        {
            Build();
        }

        void Build()
        {
            // パーリンノイズ
            PerlinNoise perlinNoise = GetComponent<PerlinNoise>();
            float[,] grid = perlinNoise.Create(_height, _width);
            // 対応したオブジェクト生成
            TerrainGenerator terrain = GetComponent<TerrainGenerator>();
            _field = terrain.Create(grid);
            // 初期資源を配置
            InitResourceSpawner resource = GetComponent<InitResourceSpawner>();
            resource.Spawn(_field);

        }

        void OnDestroy()
        {
            foreach (Cell cell in _field) cell.Release();
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
                string message = "セルの高さを設定するのに必要なIHeightProviderが実装されていない";
                throw new System.NullReferenceException(message);
            }
            holder.SetHeight(height);
        }
    }
}
