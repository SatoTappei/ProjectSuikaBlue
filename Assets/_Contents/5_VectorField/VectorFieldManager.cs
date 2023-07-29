using System.Collections.Generic;
using UnityEngine;
using VectorField;

/// <summary>
/// ベクトルの流れの列挙型
/// 指定した地点に向かう/指定した地点から離れるを決定する
/// </summary>
public enum FlowMode
{
    Toward, // 向かう
    Away,   // 離れる
}

/// <summary>
/// ベクターフィールド用のグリッドに関する読み取り専用のデータ
/// グリッドの生成/ベクトルの計算時に、複数のクラスでグリッドの情報を共有出来る
/// </summary>
[System.Serializable]
public class GridData
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

    public int Width => _width;
    public int Height => _height;
    public float CellSize => _cellSize;
    public LayerMask ObstacleLayer => _obstacleLayer;
    public float ObstacleHeight => _obstacleHeight;

    public Vector3 GridOrigin { get; set; }
}

/// <summary>
/// ベクトルフィールドの生成を管理するクラス
/// 外部からはこのクラスのメソッドを用いてベクトルフィールドを操作する
/// </summary>
public class VectorFieldManager : MonoBehaviour
{
    [SerializeField] GridData _gridData;

    Cell[,] _grid;
    VectorCalculator _vectorCalculator;
    FlowCalculator _flowCalculator;
    DebugVectorVisualizer _vectorVisualizer;
    DebugGridVisualizer _gridVisualizer;

    /// <summary>
    /// 一度CreateVectorFieldメソッドを呼び、ベクトルフィールドを生成したフラグ
    /// ベクトルフィールドを作成しないとベクトルの流れを取得できない
    /// </summary>
    bool _vectorFieldCreated;

    /// <summary>
    /// 外部から呼び出すことでベクトルフィールドの作成に必要なグリッドを作成する
    /// グリッド自体はステージに合わせて作成する必要があるので、それらの生成処理が終わった後に呼ぶ
    /// </summary>
    public void CreateGrid()
    {
        // グリッド生成
        GridBuilder gridBuilder = new();
        _grid = gridBuilder.CreateGrid(_gridData);

        _vectorCalculator = new(_grid, _gridData);
        _flowCalculator = new(_grid, _gridData);
        TryGetComponent(out _vectorVisualizer);
        TryGetComponent(out _gridVisualizer);
    }

    /// <summary>
    /// 外部から呼び出すことで、指定した位置を基準にベクトルフィールドを作成する
    /// Y座標はグリッドの高さを基準にするので無視される
    /// </summary>
    public void CreateVectorField(Vector3 pos, FlowMode mode)
    {
        if (_grid == null)
        {
            throw new System.NullReferenceException("グリッド未作成");
        }

        _vectorCalculator.CreateVectorField(pos);
        _vectorFieldCreated = true;

#if UNITY_EDITOR
        // ベクトルの流れの描画
        if (_vectorVisualizer != null)
        {
            _vectorVisualizer.VisualizeVectorFlow(_grid);
        }
#endif
    }

    /// <summary>
    /// 位置に対応したセルのベクトルを取得する
    /// </summary>
    public Vector3 GetCellVector(in Vector3 pos)
    {
        Vector2Int index = GridUtility.WorldPosToGridIndex(in pos, _grid, _gridData);
        return _grid[index.y, index.x].Vector;
    }

    /// <summary>
    /// 外部から呼び出すことで、指定した位置からの正規化されたベクトルの流れを取得する
    /// Y座標はグリッドの高さを基準にするので無視される
    /// </summary>
    public void InsertVectorFlowToQueue(Vector3 pos, Queue<Vector3> queue)
    {
        if (!_vectorFieldCreated)
        {
            throw new System.InvalidOperationException("ベクトルフィールド未作成");
        }

        _flowCalculator.InsertVectorFlowToQueue(pos, queue);
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying && _gridVisualizer != null)
        {
            // グリッドと各セルのコストの描画
            _gridVisualizer.DrawGridOnGizmos(_grid, _gridData);
        }
    }
}
