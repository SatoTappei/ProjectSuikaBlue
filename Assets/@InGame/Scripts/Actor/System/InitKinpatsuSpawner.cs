using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public class InitKinpatsuSpawner : ActorSpawner
    {
        /// <summary>
        /// 渦巻きループ用の時計回り方向
        /// </summary>
        readonly Vector2Int[] Dirs =
        {
            Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down,
        };

        [SerializeField] Actor _leaderPrefab;
        [SerializeField] Actor _unitPrefab;
        [Range(2, 9)]
        [SerializeField] int _totalSpawn = 3;
        [SerializeField] float _spawnHeight = 1.0f;

        /// <summary>
        /// フィールドの中央から時計回りに生成箇所を探し、生成する
        /// </summary>
        public void Spawn(Cell[,] field)
        {
            Queue<Cell> temp = new(9);
            // 中央から周囲8近傍を調べていく
            int y = field.GetLength(0) / 2;
            int x = field.GetLength(1) / 2;

            if (TrySpawn(temp, field, y, x)) return;

            int max = field.Length / 3; // ざっくりとした全体を探索可能な回数
            int sideLength = 1;
            int sideCount = 0;
            for (int i = 0; i < max; i++)
            {
                int index = sideCount % 4;
                for(int k = 0; k < sideLength; k++)
                {
                    y += Dirs[index].y * 3;
                    x += Dirs[index].x * 3;

                    if (TrySpawn(temp, field, y, x)) return;
                }

                // 2辺移動したら一辺当たりの長さが1増える
                if (index == 1 || index == 3) sideLength++;

                sideCount++;
            }

            throw new System.InvalidOperationException("金髪の初期生成地点が見つからない");
        }

        bool TrySpawn(Queue<Cell> temp, Cell[,] field, int y, int x)
        {
            // このセルと生成可能な周囲八近傍のセルを一時保存のキューに挿入する
            // 生成個数より生成可能なセルが多ければ生成
            InsertNeighbourCells(temp, field, y, x);
            if (temp.Count >= _totalSpawn)
            {
                Spawn(temp);
                return true;
            }

            return false;
        }

        void Spawn(Queue<Cell> temp)
        {
            for (int m = 0; m < _totalSpawn; m++)
            {
                Cell cell = temp.Dequeue();
                // 最初にリーダーを生成し、その後に普通の金髪を生成する
                Actor prefab = m == 0 ? _leaderPrefab : _unitPrefab;
                // キャラクターの生成
                Vector3 pos = new Vector3(cell.Pos.x, _spawnHeight, cell.Pos.z);
                InstantiateActor(prefab, pos);
            }

            temp.Clear();
        }

        void InsertNeighbourCells(Queue<Cell> temp, Cell[,] field, int y, int x)
        {
            temp.Clear();
            temp.Enqueue(field[y, x]);

            foreach (Vector2Int dir in Utility.EightDirections)
            {
                int nx = x + dir.x;
                int ny = y + dir.y;
                
                if (!FieldUtility.IsWithinGrid(field, y, x)) continue;
                // 海や資源のあるセルを除く
                if (!field[ny, nx].IsWalkable) continue;

                temp.Enqueue(field[ny, nx]);
            }
        }
    }
}