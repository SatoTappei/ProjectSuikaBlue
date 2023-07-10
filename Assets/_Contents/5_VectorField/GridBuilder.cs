using UnityEngine;

namespace VectorField
{
    /// <summary>
    /// VectorFieldManager�N���X������GridData�����ɁA�O���b�h�Ɗe�Z���̐���
    /// �y�уZ���̏����R�X�g��ݒ肵�ĕԂ��������s���N���X
    /// </summary>
    public class GridBuilder
    {
        public Cell[,] CreateGrid(GridData data)
        {
            // [����,��]
            Cell[,] grid = new Cell[data.Height, data.Width];
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int k = 0; k < grid.GetLength(1); k++)
                {
                    CreateCell(grid, data, i, k);
                }
            }

            return grid;
        }

        void CreateCell(Cell[,] grid, GridData data, int z, int x)
        {
            // �e�Z���̈ʒu�Ɍ����ďォ��Ray���΂��ď�Q�������m
            Vector3 cellPos = GridIndexToWorldPos(data, z, x);
            Vector3 rayOrigin = cellPos + Vector3.up * data.ObstacleHeight;
            bool isHit = Physics.SphereCast(rayOrigin, data.CellSize / 2, Vector3.down,
                out RaycastHit _, data.ObstacleHeight, data.ObstacleLayer);

            // �Z���̍쐬���R�X�g��ݒ�
            grid[z, x] = new Cell(cellPos, z, x)
            {
                Cost = (byte)(isHit ? byte.MaxValue : 1),
                CalculatedCost = ushort.MaxValue,
            };
        }

        Vector3 GridIndexToWorldPos(GridData data, int z, int x)
        {
            // �ӂ̑傫���������̏ꍇ�̓Z���̒��S�ɍ��킹��I�t�Z�b�g���K�v
            float offsetZ = data.Height % 2 == 0 ? 0.5f : 0;
            float offsetX = data.Width % 2 == 0 ? 0.5f : 0;

            float posZ = (data.GridOrigin.z + z - data.Height / 2 + offsetZ) * data.CellSize;
            float posX = (data.GridOrigin.x + x - data.Width / 2 + offsetX) * data.CellSize;
            // y���W�̓O���b�h�̍�������ɂ��ĕԂ��̂Œ���
            return new Vector3(posX, data.GridOrigin.y, posZ);
        }
    }
}
