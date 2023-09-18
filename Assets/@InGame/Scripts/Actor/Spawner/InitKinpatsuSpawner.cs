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
                    y += Utility.Counterclockwise[index].y * 3; // 3*3�͈̔͂��`�F�b�N����
                    x += Utility.Counterclockwise[index].x * 3; // 3*3�͈̔͂��`�F�b�N����

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
                ActorType type = m == 0 ? ActorType.KinpatsuLeader : ActorType.Kinpatsu;
                // �L�����N�^�[�̐���
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
                // �C�⎑���̂���Z��������
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
                Message = $"<color={color}>{name}</color>���Q��ɉ�������ł��B"
            });
        }
    }
}