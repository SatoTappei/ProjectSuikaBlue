using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public class InitKinpatsuSpawner : ActorSpawner
    {
        /// <summary>
        /// �Q�������[�v�p�̎��v������
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
        /// �t�B�[���h�̒������玞�v���ɐ����ӏ���T���A��������
        /// </summary>
        public void Spawn(Cell[,] field)
        {
            Queue<Cell> temp = new(9);
            // �����������8�ߖT�𒲂ׂĂ���
            int y = field.GetLength(0) / 2;
            int x = field.GetLength(1) / 2;

            if (TrySpawn(temp, field, y, x)) return;

            int max = field.Length / 3; // ��������Ƃ����S�̂�T���\�ȉ�
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

                // 2�ӈړ��������ӓ�����̒�����1������
                if (index == 1 || index == 3) sideLength++;

                sideCount++;
            }

            throw new System.InvalidOperationException("�����̏��������n�_��������Ȃ�");
        }

        bool TrySpawn(Queue<Cell> temp, Cell[,] field, int y, int x)
        {
            // ���̃Z���Ɛ����\�Ȏ��͔��ߖT�̃Z�����ꎞ�ۑ��̃L���[�ɑ}������
            // ��������萶���\�ȃZ����������ΐ���
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
                // �ŏ��Ƀ��[�_�[�𐶐����A���̌�ɕ��ʂ̋����𐶐�����
                Actor prefab = m == 0 ? _leaderPrefab : _unitPrefab;
                // �L�����N�^�[�̐���
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
                // �C�⎑���̂���Z��������
                if (!field[ny, nx].IsWalkable) continue;

                temp.Enqueue(field[ny, nx]);
            }
        }
    }
}