using UnityEngine;

namespace PSB.InGame
{
    public class InitResourceSpawner : MonoBehaviour
    {
        [SerializeField] ResourceType[] _soilResources;
        [SerializeField] ResourceType[] _grassResources;
        [Range(0, 1)]
        [SerializeField] float _spawnRate = 0.018f;
        [SerializeField] uint _seed = 109;

        public void Spawn(Cell[,] field)
        {
            Unity.Mathematics.Random random = new(_seed);
            for(int i = 0; i < field.GetLength(0); i++)
            {
                for(int k = 0; k < field.GetLength(1); k++)
                {
                    // �m���Ŏ�����z�u
                    if (random.NextFloat() <= _spawnRate) CellResourceSpawn(field[i, k], random);
                }
            }
        }

        /// <summary>
        /// �Z���Ƀ����_���Ɏ�����z�u����
        /// </summary>
        void CellResourceSpawn(Cell cell, Unity.Mathematics.Random random)
        {
            if (cell.TileType == TileType.Soil)
            {
                ResourceType type = _soilResources[random.NextInt(_soilResources.Length)];
                cell.ResourceType = type;
            }
            else if (cell.TileType == TileType.Grass)
            {
                ResourceType type = _grassResources[random.NextInt(_grassResources.Length)];
                cell.ResourceType = type;
            }
        }

#if ���g�p
        void InsertNeighbourCells(Queue<Cell> queue, Cell[,] field, int y, int x)
        {
            queue.Clear();

            foreach(Vector2Int dir in Utility.EightDirections)
            {
                int nx = x + dir.x;
                int ny = y + dir.y;

                if (!IsWithinGrid(field, y, x)) continue;
                queue.Enqueue(field[ny, nx]);
            }
        }

        bool IsWithinGrid(Cell[,] field, int y, int x)
        {
            return 0 <= y && y < field.GetLength(0) && 0 <= x && x < field.GetLength(1);
        }
#endif
    }
}
