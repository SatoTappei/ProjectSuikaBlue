using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

namespace VectorField
{
    /// <summary>
    /// �x�N�g���̗���̗񋓌^
    /// �w�肵���n�_�Ɍ�����/�w�肵���n�_���痣�������肷��
    /// </summary>
    public enum FlowMode
    {
        Toward,
        Away,
    }

    /// <summary>
    /// �x�N�^�[�t�B�[���h�̃O���b�h�̃N���X
    /// </summary>
    public class GridController : MonoBehaviour
    {
        [Header("�O���b�h�̐ݒ�")]
        [SerializeField] int _width = 20;
        [SerializeField] int _height = 20;
        [SerializeField] float _cellSize = 5.0f;
        [Header("��Q���̃��C���[")]
        [SerializeField] LayerMask _obstacleLayer;
        [Header("��Q���̍���")]
        [Tooltip("���̍�������Ray���΂��ď�Q���Ƀq�b�g�������ǂ����Ō��o����")]
        [SerializeField] float _obstacleHeight = 10.0f;

        Transform _transform;
        Cell[,] _grid;
        Cell _targetCell;
        VectorCalculator _vectorCalculator;

        public Cell[,] Grid => _grid;
        public int Width => _width;
        public int Height => _height;
        public float CellSize => _cellSize;

        void Awake()
        {
            _transform = transform;
            CreateGrid();
            TryGetComponent(out VectorVisualizer vectorVisualizer);
            _vectorCalculator = new(_grid, vectorVisualizer);
        }

        /// <summary>
        /// �O������Ăяo�����ƂŎw�肵���ʒu�𒆐S�Ƀx�N�g���̗�����쐬����
        /// </summary>
        public void SetVectorFlowCenterCell(Vector3 pos, FlowMode mode)
        {
            _vectorCalculator.SetTargetCell(Vector3.zero);
        }

        void CreateGrid()
        {
            // [����,��]
            _grid = new Cell[_height, _width];
            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int k = 0; k < _grid.GetLength(1); k++)
                {
                    CreateCell(i, k);
                }
            }
        }

        void CreateCell(int z, int x)
        {
            // �e�Z���̈ʒu�Ɍ����ďォ��Ray���΂��ď�Q�������m
            Vector3 cellPos = GetCellWorldPos(z, x);
            Vector3 rayOrigin = cellPos + Vector3.up * _obstacleHeight;
            bool isHit = Physics.SphereCast(rayOrigin, _cellSize / 2, Vector3.down,
                out RaycastHit hit, _obstacleHeight, _obstacleLayer);

            _grid[z, x] = new Cell(cellPos, z, x);
            _grid[z, x].Cost = (byte)(isHit ? byte.MaxValue : 1);
            _grid[z, x].CalculatedCost = ushort.MaxValue;
        }

        Vector3 GetCellWorldPos(int z, int x)
        {
            // �ӂ̑傫���������̏ꍇ�̓Z���̒��S�ɍ��킹��I�t�Z�b�g���K�v
            float offsetZ = _height % 2 == 0 ? 0.5f : 0;
            float offsetX = _width % 2 == 0 ? 0.5f : 0;

            float posZ = (_transform.position.z + z - _height / 2 + offsetZ) * _cellSize;
            float posX = (_transform.position.x + x - _width / 2 + offsetX) * _cellSize;
            return new Vector3(posX, _transform.position.y, posZ);
        }
    }
}