using UnityEngine;
using System.Collections.Generic;

namespace Old
{


    /// <summary>
    /// ベクターフィールドのグリッドのクラス
    /// グリッドの生成＆各コストの設定を行う
    /// </summary>
    public class GridController : MonoBehaviour
    {
        [Header("グリッドの設定")]
        [SerializeField] int _width = 20;
        [SerializeField] int _height = 20;
        [SerializeField] float _cellSize = 5.0f;
        [Header("障害物のレイヤー")]
        [SerializeField] LayerMask _obstacleLayer;
        [Header("障害物の高さ")]
        [Tooltip("この高さからRayを飛ばして障害物にヒットしたかどうかで検出する")]
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
        /// 外部から呼び出すことで指定した位置を中心にベクトルの流れを作成する
        /// </summary>
        public void SetVectorFlowCenterCell(Vector3 pos, FlowMode mode)
        {
            // ↓ここ修正必要
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

        // posからのベクトルの流れを返す
        // 
        public List<Vector3> GetFlow(Vector3 pos)
        {
            List<Vector3> list = _calculator.GetFlow(pos);
            return list;
        }

        void CreateGrid()
        {
            // [高さ,幅]
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
            // 各セルの位置に向けて上からRayを飛ばして障害物を検知
            Vector3 cellPos = GridIndexToWorldPos(z, x);
            Vector3 rayOrigin = cellPos + Vector3.up * _obstacleHeight;
            bool isHit = Physics.SphereCast(rayOrigin, _cellSize / 2, Vector3.down,
                out RaycastHit hit, _obstacleHeight, _obstacleLayer);

            // セルの作成＆コストを設定
            _grid[z, x] = new Cell(cellPos, z, x)
            {
                Cost = (byte)(isHit ? byte.MaxValue : 1),
                CalculatedCost = ushort.MaxValue,
            };
        }

        Vector3 GridIndexToWorldPos(int z, int x)
        {
            // 辺の大きさが偶数の場合はセルの中心に合わせるオフセットが必要
            float offsetZ = _height % 2 == 0 ? 0.5f : 0;
            float offsetX = _width % 2 == 0 ? 0.5f : 0;

            float posZ = (_transform.position.z + z - _height / 2 + offsetZ) * _cellSize;
            float posX = (_transform.position.x + x - _width / 2 + offsetX) * _cellSize;
            return new Vector3(posX, _transform.position.y, posZ);
        }
    }
}