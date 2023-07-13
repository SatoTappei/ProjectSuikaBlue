using System.Collections.Generic;
using UnityEngine;
using VectorField;

/// <summary>
/// ���������x�N�g���t�B�[���h��̔C�ӂ�2���W�Ԃ̃x�N�g���̗�����v�Z���鏈���𔲂��o�����N���X
/// �\�߃O���b�h���̊e�Z���̃x�N�g�����v�Z�ς݂ł���K�v������
/// </summary>
public class FlowCalculator
{
    Cell[,] _grid;
    GridData _data;

    public FlowCalculator(Cell[,] grid, GridData data)
    {
        _grid = grid;
        _data = data;
    }

    /// <summary>
    /// �x�N�g���̗���������œn�����L���[�ɑ}�����Ă���
    /// ���̃N���X���L���[�������A�����Ɍo�H���l�߂ĕԂ��ƕ����̃C���X�^���X����Ăяo���ꂽ�ۂ�
    /// �L���[����ɂ��ĐV���Ȍo�H���l�߂�ƁA�Q�Ƃ���o�H���ς���Ă��܂����߁A�������Ă���
    /// </summary>
    public void InsertVectorFlowToQueue(Vector3 startPos, Queue<Vector3> queue)
    {
        queue.Clear();

        // �n�_�̃x�N�g�����L���[�ɑ}��
        Vector2Int index = GridUtility.WorldPosToGridIndex(startPos, _grid, _data);
        queue.Enqueue(_grid[index.y, index.x].Vector);
        // �x�N�g���̌����ɉ������ׂ̃Z���̃x�N�g�����L���[�ɑ}�����Ă���
        // �Z�����̐��ȏニ�[�v���Ȃ��悤�ɂ��Ė������[�v��h��
        for (int i = 0; i < _data.Height * _data.Width; i++)
        {
            Vector2Int dirIndex = CellVectorToDirIndex(_grid[index.y, index.x]);
            Vector2Int neighbourIndex = index + dirIndex;

            // �O���b�h�O�������̓x�N�g���̗���̏I�[�ɓ��B�����ꍇ
            if (IsFlowEnd(neighbourIndex.y, neighbourIndex.x)) break;

            queue.Enqueue(_grid[neighbourIndex.y, neighbourIndex.x].Vector);
            index = neighbourIndex;
        }
    }

    /// <summary>
    /// �Z���̃x�N�g�����x�N�g������������̃Z���̓Y�����ɕϊ����ĕԂ�
    /// Vector3��x�͂��̂܂�Vector2Int��x�Az��y�ɑΉ����Ă���
    /// </summary>
    Vector2Int CellVectorToDirIndex(Cell cell)
    {
        if (cell.Vector == Vector3.zero) return new Vector2Int(0, 0);

        if (cell.Vector == new Vector3(0, 0, 1)) return new Vector2Int(0, 1);
        else if (cell.Vector == new Vector3(0, 0, -1)) return new Vector2Int(0, -1);
        else if (cell.Vector == new Vector3(1, 0, 0)) return new Vector2Int(1, 0);
        else if (cell.Vector == new Vector3(-1, 0, 0)) return new Vector2Int(-1, 0);
        else if (cell.Vector == new Vector3(1, 0, 1).normalized) return new Vector2Int(1, 1);
        else if (cell.Vector == new Vector3(1, 0, -1).normalized) return new Vector2Int(1, -1);
        else if (cell.Vector == new Vector3(-1, 0, 1).normalized) return new Vector2Int(-1, 1);
        else if (cell.Vector == new Vector3(-1, 0, -1).normalized) return new Vector2Int(-1, -1);
        else
        {
            throw new System.ArgumentException("�x�N�g���̒l���Z���̓Y�����ɑΉ����Ă��Ȃ�: " + cell.Vector);
        }
    }

    bool IsFlowEnd(int z, int x)
    {
        // �O���b�h�͈͓̔����`�F�b�N
        if (!(0 <= z && z < _grid.GetLength(0) && 0 <= x && x < _grid.GetLength(1))) return true;
        
        // �ڕW�̃Z���̏ꍇ���`�F�b�N
        return _grid[z, x].Vector == Vector3.zero;
    }
}
