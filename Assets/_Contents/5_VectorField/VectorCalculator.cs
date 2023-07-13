using System.Collections.Generic;
using UnityEngine;
using VectorField;

/// <summary>
/// �x�N�^�[�t�B�[���h�̊e�Z���̃x�N�g�����v�Z�̏��������𔲂��o�����N���X
/// �x�N�g���̌v�Z���s���ɂ͗\�߃O���b�h�𐶐����ăR�X�g���t�^���Ă���K�v������
/// </summary>
public class VectorCalculator
{
    /// <summary>
    /// �O���b�h��ŏ㉺���E�̓Y�������w�肷�邽�߂̔z��
    /// </summary>
    static readonly Vector2Int[] FourDirections =
    {
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1),
    };

    /// <summary>
    /// �O���b�h��Ŏ��v���ɔ��ߖT�̓Y�������w�肷�邽�߂̔z��
    /// </summary>
    static readonly Vector2Int[] EightDirections =
    {
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1),
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
        new Vector2Int(1, 0),
        new Vector2Int(1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, -1),
    };

    Queue<Cell> _openQueue = new();
    Queue<Cell> _neighbourQueue = new(8);
    Cell[,] _grid;
    GridData _data;

    public VectorCalculator(Cell[,] grid, GridData data)
    {
        _grid = grid;
        _data = data;
    }

    /// <summary>
    /// ���̃��\�b�h���O������Ăяo�����ƂŁA�w�肵���ʒu�Ɍ������x�N�g���t�B�[���h���쐬����
    /// </summary>
    public Cell CreateVectorField(Vector3 targetPos)
    {
        InitCellAll();
        Cell targetCell = SetTargetCell(targetPos);
        CalculateNeighbourCellCost(targetCell);
        CreateVectorFlow();

        return targetCell;
    }

    void InitCellAll()
    {
        foreach (Cell cell in _grid)
        {
            cell.CalculatedCost = ushort.MaxValue;
            cell.Vector = Vector3.zero;
        }
    }

    /// <summary>
    /// �w�肵���ʒu�ɑΉ�����Z���̃R�X�g/�v�Z�ς݃R�X�g����0�ɂ��ĕԂ�
    /// ���̃Z������Ƀx�N�g���̗�������
    /// </summary>
    Cell SetTargetCell(Vector3 targetPos)
    {
        Vector2Int index = GridUtility.WorldPosToGridIndex(targetPos, _grid, _data);
        Cell targetCell = _grid[index.y, index.x];
        targetCell.Cost = 0;
        targetCell.CalculatedCost = 0;

        return targetCell;
    }

    /// <summary>
    /// ���D��T����p���ăR�X�g/�v�Z�ς݃R�X�g��0�̃Z���̏㉺���E�̃Z���̃R�X�g/�v�Z�ς݂��v�Z����
    /// �f�t�H���g�̌v�Z�ς݃R�X�g��65535�Ȃ̂ōŏ���1�񂾂��͕K���X�V�\
    /// </summary>
    void CalculateNeighbourCellCost(Cell targetCell)
    {
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
    }

    /// <summary>
    /// ���͔��ߖT�𒲂ׂăR�X�g�����Ƀx�N�g���̗�����쐬����
    /// </summary>
    void CreateVectorFlow()
    {
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
    }

    /// <summary>
    /// �I�������Z����8��������4�����̃Z�����J�����Z���̃L���[�ɑ}������
    /// </summary>
    void InsertNeighbours(Cell current, Vector2Int[] directions)
    {
        foreach (Vector2Int dir in directions)
        {
            int neighbourIndexZ = current.Z + dir.y;
            int neighbourIndexX = current.X + dir.x;

            if (IsWithinGrid(neighbourIndexZ, neighbourIndexX))
            {
                _neighbourQueue.Enqueue(_grid[neighbourIndexZ, neighbourIndexX]);
            }
        }
    }

    bool IsWithinGrid(int z, int x)
    {
        return 0 <= z && z < _grid.GetLength(0) && 0 <= x && x < _grid.GetLength(1);
    }

    /// <summary>
    /// �אڂ����Z���Ƃ̈ʒu�֌W���琳�K�����ꂽ�����x�N�g�������߂�
    /// </summary>
    Vector3 CalculateVectorToNeighbourCell(Cell current, Cell neighbour)
    {
        int indexDirZ = neighbour.Z - current.Z;
        int indexDirX = neighbour.X - current.X;

        Vector3 vector = new Vector3(indexDirX, 0, indexDirZ);
        if (indexDirZ * indexDirX != 0)
        {
            vector.Normalize();
        }

        return vector;
    }
}
