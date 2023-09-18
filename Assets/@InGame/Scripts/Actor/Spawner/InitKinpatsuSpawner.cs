using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace PSB.InGame
{
    public class InitKinpatsuSpawner : ActorSpawner
    {
        [SerializeField] float _spawnHeight = 1.0f;
        [Range(1, 9)]
        [SerializeField] int _totalSpawn = 3;

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
                    y += Utility.Counterclockwise[index].y * 3; // 3*3の範囲をチェックする
                    x += Utility.Counterclockwise[index].x * 3; // 3*3の範囲をチェックする

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
                ActorType type = m == 0 ? ActorType.KinpatsuLeader : ActorType.Kinpatsu;
                // キャラクターの生成
                Vector3 pos = new Vector3(cell.Pos.x, _spawnHeight, cell.Pos.z);
                if(TryInstantiate(type, pos, out Actor actor))
                {
                    SendSpawnMessage(actor.name);
                }
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
                
                if (!IsWithinGrid(field, y, x)) continue;
                // 海や資源のあるセルを除く
                if (!field[ny, nx].IsWalkable) continue;

                temp.Enqueue(field[ny, nx]);
            }
        }

        bool IsWithinGrid(Cell[,] field, int y, int x)
        {
            return 0 <= y && y < field.GetLength(0) && 0 <= x && x < field.GetLength(1);
        }

        void SendSpawnMessage(string name)
        {
            string color = Utility.ColorCodeGreen;
            MessageBroker.Default.Publish(new EventLogMessage() 
            {
                Message = $"<color={color}>{name}</color>が群れに加わったです。"
            });
        }
    }
}