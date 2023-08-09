using UnityEngine;

namespace Field
{
    public class PerlinNoise : MonoBehaviour
    {
        [SerializeField] float _scale = 0.08f;
        [SerializeField] int _seed = 262;

        /// <summary>
        /// パーリンノイズを用いてグリッドを生成する
        /// </summary>
        /// <returns>[高さ,幅]の二次元配列</returns>
        public float[,] Create(int height, int width)
        {
            float[,] grid = new float[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int k = 0; k < width; k++)
                {
                    grid[i,k] = Mathf.PerlinNoise(i * _scale + _seed, k * _scale + _seed);
                }
            }

            return grid;
        }
    }
}
