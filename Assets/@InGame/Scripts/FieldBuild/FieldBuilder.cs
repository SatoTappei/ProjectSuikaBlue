using UnityEngine;

namespace FieldBuild
{
    [RequireComponent(typeof(PerlinNoise))]
    [RequireComponent(typeof(Visualizer))]
    public class FieldBuilder : MonoBehaviour
    {
        [SerializeField] int _width = 50;
        [SerializeField] int _height = 50;

        void Start()
        {
            Build();
        }

        /// <summary>
        /// ノイズ、生成、セルに設定 と計3回 幅*高さ のfor文を回しているので非効率
        /// </summary>
        void Build()
        {
            // パーリンノイズ
            PerlinNoise perlinNoise = GetComponent<PerlinNoise>();
            float[,] grid = perlinNoise.Create(_height, _width);
            // 対応したオブジェクト生成
            Visualizer visualizer = GetComponent<Visualizer>();
            GameObject[,] field = visualizer.Instantiate(grid);
            // セルに高さを設定
            SetCellHeight(grid, field);
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
