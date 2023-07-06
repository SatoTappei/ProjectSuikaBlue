using PathTableGraph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 経路テーブルを用いた経路探索を行うクラス
/// 事前にシーン上に頂点となるオブジェクトを配置している必要がある
/// </summary>
public class PathTableManager : MonoBehaviour
{
    [SerializeField] Transform[] _vertexObjects;
    [Header("各頂点のレイヤー")]
    [SerializeField] LayerMask _vertexLayer;
    [Header("障害物のレイヤー")]
    [SerializeField] LayerMask _obstacleLayer;
    [Header("頂点のタグ")]
    [SerializeField] string _vertexTag = "Vertex";
    [Header("付近の頂点として検出できる半径")]
    [SerializeField] float _neighbourRadius = 10;
    [Header("ギズモへの描画")]
    [SerializeField] bool _drawGizmos = true;

    AStarTask _aStarTask;
    Vertex[] _graph;
    /// <summary>
    /// ギズモに表示するためにメンバとして保持しておく
    /// </summary>
    Stack<Vector3> _path;
    /// <summary>
    /// 経路テーブル
    /// </summary>
    Stack<Vector3>[,] _table;

    void Awake()
    {
        // 1始まりの頂点番号で取得したいので 0番目をダミーにしておく
        _graph = new Vertex[_vertexObjects.Length + 1];
        _aStarTask = new AStarTask(_graph);

        CreateVertex();
        CreateGraph();
        WritePathToTextAsset();
    }

    /// <summary>
    /// 1始まりの頂点番号を付与して頂点の作成
    /// グラフから頂点番号で取得できるようにする
    /// </summary>
    void CreateVertex()
    {
        for(int i = 0; i < _vertexObjects.Length; i++)
        {
            Vertex vertex = _vertexObjects[i].gameObject.AddComponent<Vertex>();
            vertex.Number = i + 1;
            _graph[i + 1] = vertex;
        }
    }

    /// <summary>
    /// レイキャストを用いて各頂点の隣接している頂点を取得＆追加する
    /// </summary>
    void CreateGraph()
    {
        for (int i = 0; i < _vertexObjects.Length; i++)
        {
            // 周囲の頂点を取得
            Vector3 vertexPos = _vertexObjects[i].position;
            Collider[] neighbours = Physics.OverlapSphere(vertexPos, _neighbourRadius, _vertexLayer);
            foreach (Collider neighbour in neighbours)
            {
                Vector3 dirVector = neighbour.transform.position - vertexPos;
                Vector3 dir = dirVector.normalized;
                // 障害物に遮られていないかチェック
                LayerMask layerMask = _vertexLayer | _obstacleLayer;
                bool isHit = Physics.Raycast(vertexPos, dir, out RaycastHit hitInfo, _neighbourRadius, layerMask);
                if (isHit && hitInfo.collider.CompareTag(_vertexTag))
                {
                    // 隣接している頂点を追加
                    if (hitInfo.collider.TryGetComponent(out Vertex neighbourVertex))
                    {
                        _graph[i + 1].AddNeighbour(neighbourVertex);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 経路探索を行い、テキストファイルに書き込む
    /// </summary>
    public void WritePathToTextAsset()
    {
        //TextAssetGenerator generator = new();


        int i = Random.Range(1, _vertexObjects.Length + 1);
        int k = Random.Range(1, _vertexObjects.Length + 1);

#if UNITY_EDITOR
        Stopwatch stopwatch = new(i, k);
        stopwatch.Start();
#endif
        _path = _aStarTask.Pathfinding(i, k);
#if UNITY_EDITOR
        stopwatch.Stop();
#endif
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying && _drawGizmos)
        {
            DrawPathOnGizmos();
        }
    }

    void DrawPathOnGizmos()
    {
        if (_path == null || _path.Count == 0) return;

        foreach (Vector3 pos in _path)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pos, 0.25f);
        }
    }
}