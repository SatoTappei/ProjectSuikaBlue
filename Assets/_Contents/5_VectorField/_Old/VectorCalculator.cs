using System.Collections.Generic;
using UnityEngine;

namespace Old
{
    /// <summary>
    /// 8�����̗񋓌^
    /// </summary>
    public enum EightDirection
    {
        Neutral, // �ǂ̕����ł��Ȃ�
        North,
        South,
        West,
        East,
        NorthEast,
        NorthWest,
        SouthEast,
        SouthWest,
    }

    /// <summary>
    /// �x�N�^�[�t�B�[���h�̊e�Z���̃x�N�g�����v�Z�̏��������𔲂��o�����N���X
    /// �\�߃O���b�h�𐶐����ăR�X�g���t�^���Ă���K�v������
    /// </summary>
    public class VectorCalculator
    {
        /// <summary>
        /// �O���b�h��ŏ㉺���E�̓Y�������w�肷�邽�߂̔z��
        /// </summary>
        static readonly (int z, int x)[] FourDirections =
        {
            (-1, 0), (1, 0), (0, -1), (0, 1),
        };

        /// <summary>
        /// �O���b�h��Ŕ��ߖT�̓Y�������w�肷�邽�߂̔z��
        /// </summary>
        static readonly (int z, int x)[] EightDirections =
        {
            (-1, 0), (1, 0), (0, -1), (0, 1), 
            (-1, 1), (-1, -1), (1, 1), (1, -1),
        };

        Queue<Cell> _openQueue;
        Queue<Cell> _neighbourQueue;
        Cell[,] _grid;
        Vector3 _gridOrigin;
        float _cellSize;

        public VectorCalculator(Cell[,] grid, Vector3 gridOrigin, float cellSize)
        {
            _openQueue = new Queue<Cell>();
            // 4�������ׂ�̂ŏ����e�ʂ�4�ŌŒ�
            _neighbourQueue = new Queue<Cell>(4);
            _grid = grid;
            _gridOrigin = gridOrigin;
            _cellSize = cellSize;
        }

        /// <summary>
        /// �O������Ăяo�����ƂŎw�肵���ʒu�Ɍ������x�N�g���̗�����쐬����
        /// �^�[�Q�b�g�ƂȂ�Z����Ԃ�
        /// </summary>
        public Cell CreateVectorField(Vector3 targetPos)
        {
            // �w�肵���ʒu�̃R�X�g��0�ɂ���
            WorldPosToGridIndex(targetPos, out int z, out int x);
            Cell targetCell = _grid[z, x];
            targetCell.Cost = 0;
            targetCell.CalculatedCost = 0;

            // �I�������Z������㉺���E4�����̃Z���𒲂ׂĂ���
            _openQueue.Clear();
            _openQueue.Enqueue(targetCell);
            while (_openQueue.Count > 0)
            {
                Cell current = _openQueue.Dequeue();

                // �㉺���E
                _neighbourQueue.Clear();
                InsertNeighbours(current, FourDirections);
                foreach (Cell neighbour in _neighbourQueue)
                {
                    // �ׂ̃Z���̃R�X�g���ő�̏ꍇ�͏������Ȃ�
                    if (neighbour.Cost == byte.MaxValue) continue;
                    // �ׂ̃Z���̌v�Z�ς݃R�X�g����菬�����Ȃ�ꍇ�͍X�V
                    if (neighbour.Cost + current.CalculatedCost < neighbour.CalculatedCost)
                    {
                        neighbour.CalculatedCost = (ushort)(neighbour.Cost + current.CalculatedCost);
                        _openQueue.Enqueue(neighbour);
                    }
                }
            }

            // ���͔��ߖT�𒲂ׂăR�X�g�����Ƀx�N�g���t�B�[���h���쐬����
            foreach (Cell cell in _grid)
            {
                _neighbourQueue.Clear();
                InsertNeighbours(cell, EightDirections);
                int baseCalculatedCost = cell.CalculatedCost;
                foreach (Cell neighbour in _neighbourQueue)
                {
                    // ��ƂȂ�v�Z�ς݃R�X�g�����͂̃Z���̌v�Z�ς݃R�X�g�̕����Ⴂ�ꍇ��
                    // ���̕����ւ̃x�N�g�����쐬���Ċ���X�V
                    if (neighbour.CalculatedCost < baseCalculatedCost)
                    {
                        baseCalculatedCost = neighbour.CalculatedCost;
                        cell.Vector = CalculateVectorToNeighbourCell(cell, neighbour);
                    }
                }
            }

            return targetCell;
        }

        /// <summary>
        /// �w�肵���ʒu����^�[�Q�b�g�������͍s���~�܂�܂ł̃x�N�g���̗����Ԃ�
        /// </summary>
        public List<Vector3> GetFlow(Vector3 pos)
        {
            WorldPosToGridIndex(pos, out int z, out int x);
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = _grid[z, x].Pos;

            return null;
        }

        /// <summary>
        /// ���[���h���W�ɑΉ������O���b�h�̓Y������Ԃ�
        /// </summary>
        void WorldPosToGridIndex(Vector3 targetPos, out int z, out int x)
        {
            float forwardZ = _grid[0, 0].Pos.z;
            float backZ = _grid[_grid.GetLength(0) - 1, _grid.GetLength(1) - 1].Pos.z;
            float leftX = _grid[0, 0].Pos.x;
            float rightX = _grid[_grid.GetLength(0) - 1, _grid.GetLength(1) - 1].Pos.x;

            float lengthZ = backZ - forwardZ;
            float lengthX = rightX - leftX;

            float fromPosZ = targetPos.z - forwardZ;
            float fromPosX = targetPos.x - leftX;
            // �O���b�h�̒[���牽���̈ʒu��
            float percentZ = Mathf.Abs(fromPosZ / lengthZ);
            float percentX = Mathf.Abs(fromPosX / lengthX);
            // �Y�����ɑΉ�������
            z = Mathf.RoundToInt((_grid.GetLength(0) - 1) * percentZ);
            x = Mathf.RoundToInt((_grid.GetLength(1) - 1) * percentX);

            //Vector3 relativePos = targetPos - _gridOrigin;
            //Vector3 cellPos = relativePos / _cellSize;

            //z = Mathf.FloorToInt(cellPos.x);
            //x = Mathf.FloorToInt(cellPos.z);
        }

        /// <summary>
        /// �I�������Z����8��������4�����̃Z�����J�����Z���̃L���[�ɑ}������
        /// </summary>
        void InsertNeighbours(Cell current, (int z, int x)[] directions)
        {
            foreach ((int z, int x) dir in directions)
            {
                int neighbourZ = current.Z + dir.z;
                int neighbourX = current.X + dir.x;

                if (IsWithinGridRange(neighbourZ, neighbourX))
                {
                    _neighbourQueue.Enqueue(_grid[neighbourZ, neighbourX]);
                }
            }
        }

        bool IsWithinGridRange(int z, int x)
        {
            return (0 <= z && z < _grid.GetLength(0) && 0 <= x && x < _grid.GetLength(1));
        }

        List<Cell> GetEightNeighbours(Cell cell)
        {
            List<Cell> neighbourList = new();
            //foreach ((int z, int x) dir in EightDirections)
            //{
            //    if (!IsWithinGridRange(cell.Z + dir.z, cell.X + dir.x)) continue;
            //    Cell neighbour = GetCellAtRelativePos(cell, dir);
            //    neighbourList.Add(neighbour);
            //}

            return neighbourList;
        }

        Cell GetCellAtRelativePos(Cell cell, (int z, int x) dir)
        {
            return _grid[cell.Z + dir.z, cell.X + dir.x];
        }



        Vector3 CalculateVectorToNeighbourCell(Cell current, Cell neighbour)
        {
            int indexDirZ = neighbour.Z - current.Z;
            int indexDirX = neighbour.X - current.X;
            return new Vector3(-indexDirX, 0, -indexDirZ);

            //foreach ((int z, int x) dir in EightDirections)
            //{
            //    return new Vector3(-indexDirX, 0, -indexDirZ);
            //}

            //if (nz - cz == 0 && nx - cx == 0)
            //{
            //    return Vector3.zero;
            //}

            //foreach(var dir in EightDirections)
            //{
            //    if(nz-cz == dir.z && nx - cx == dir.x)
            //    {
            //        return new Vector3(dir.x, 0, dir.z);
            //    }
            //}

            //Debug.LogError($"�Ⴄ: z {nz - cz} x {nx - cx}");
            //return Vector3.zero;
        }
    }
}
