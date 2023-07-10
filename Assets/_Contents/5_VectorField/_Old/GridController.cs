using UnityEngine;
using System.Collections.Generic;

namespace Old
{


    /// <summary>
    /// �x�N�^�[�t�B�[���h�̃O���b�h�̃N���X
    /// �O���b�h�̐������e�R�X�g�̐ݒ���s��
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
        VectorCalculator _calculator;
        VectorVisualizer _visualizer;

        public Cell[,] Grid => _grid;
        public int Width => _width;
        public int Height => _height;
        public float CellSize => _cellSize;

        void Awake()
        {
            _transform = transform;
            CreateGrid();
            TryGetComponent(out _visualizer);
            _calculator = new(_grid, transform.position, _cellSize);
        }

        /// <summary>
        /// �O������Ăяo�����ƂŎw�肵���ʒu�𒆐S�Ƀx�N�g���̗�����쐬����
        /// </summary>
        public void SetVectorFlowCenterCell(Vector3 pos, FlowMode mode)
        {
            // �������C���K�v
            _targetCell = _calculator.CreateVectorField(Vector3.zero);
#if UNITY_EDITOR
            if (_visualizer != null)
            {
                _visualizer.RemoveAll();
                foreach (var cell in _grid)
                {
                    _visualizer.VisualizeCellVector(cell);
                }
                //_visualizer.Add(cell.Pos, GetDir(cell.Vector));
            }
#endif
        }

        // pos����̃x�N�g���̗����Ԃ�
        // 
        public List<Vector3> GetFlow(Vector3 pos)
        {
            List<Vector3> list = _calculator.GetFlow(pos);
            return list;
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
            Vector3 cellPos = GridIndexToWorldPos(z, x);
            Vector3 rayOrigin = cellPos + Vector3.up * _obstacleHeight;
            bool isHit = Physics.SphereCast(rayOrigin, _cellSize / 2, Vector3.down,
                out RaycastHit hit, _obstacleHeight, _obstacleLayer);

            // �Z���̍쐬���R�X�g��ݒ�
            _grid[z, x] = new Cell(cellPos, z, x)
            {
                Cost = (byte)(isHit ? byte.MaxValue : 1),
                CalculatedCost = ushort.MaxValue,
            };
        }

        Vector3 GridIndexToWorldPos(int z, int x)
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