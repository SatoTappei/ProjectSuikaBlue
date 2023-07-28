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
    /// </summary>
    bool _vectorFieldCreated;

    void Awake()
    {
        // グリッド生成
        GridBuilder gridBuilder = new();
        _grid = gridBuilder.CreateGrid(_gridData);

        _vectorCalculator = new(_grid, _gridData);
        _flowCalculator = new(_grid, _gridData);
        TryGetComponent(out _vectorVisualizer);
        TryGetComponent(out _gridVisualizer);

        Debug.Log("VFMのAwake");
    }

    /// <summary>
    /// 外部から呼び出すことで、指定した位置を基準にベクトルフィールドを作成する
    /// Y座標はグリッドの高さを基準にするので無視される
    /// </summary>
    public void CreateVectorField(Vector3 pos, FlowMode mode)
    {
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
    /// 外部から呼び出すことで、指定した位置からの正規化されたベクトルの流れを取得する
    /// Y座標はグリッドの高さを基準にするので無視される
    /// </summary>
    public void InsertVectorFlowToQueue(Vector3 targetPos, Queue<Vector3> queue)
    {
        if (!_vectorFieldCreated)
        {
            throw new System.InvalidOperationException("ベクトルフィールド未作成");
        }

        _flowCalculator.InsertVectorFlowToQueue(targetPos, queue);
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
