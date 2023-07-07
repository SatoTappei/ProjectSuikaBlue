using PathTableGraph;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// グラフ上の経路探索を行い、テキストアセットとして書き出すクラス
/// 事前にシーン上に頂点となるオブジェクトを配置している必要がある
/// Vertexタグ/レイヤーの頂点オブジェクト、Obstacleレイヤーの障害物が必要
/// </summary>
public class PathfindingSystem : MonoBehaviour
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

    AStarTask _aStarTask;
    Vertex[] _graph;
    /// <summary>
    /// 経路テーブル
    /// </summary>
    Stack<Vector3>[,] _table;

    /// <summary>
    /// 経路テーブルの作成が完了したかのフラグ
    /// このフラグがtrueになった後に経路の取得が可能
    /// </summary>
    bool _pathTableCreated;

    void Awake()
    {
        // 1始まりの頂点番号で取得したいので 0番目をダミーにしておく
        _graph = new Vertex[_vertexObjects.Length + 1];
        _aStarTask = new(_graph);
        _table = new Stack<Vector3>[_vertexObjects.Length + 1, _vertexObjects.Length + 1];
    }

    /// <summary>
    /// 外部からこのメソッドを呼んで経路テーブルを作成する
    /// このメソッドが完了した後に経路探索が可能になる
    /// </summary>
    public IEnumerator CreatePathTableCoroutine()
    {
        CreateVertex();
        CreateGraph();
        yield return CreateTableCoroutine();
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
    /// 外部からこのメソッドを呼ぶことで、指定した2地点から一番近い頂点同士の経路が求まる
    /// </summary>
    public Stack<Vector3> GetPath(Vector3 startPos, Vector3 goalPos)
    {
        if (!_pathTableCreated)
        {
            Debug.LogWarning("経路テーブル未作成: その場に移動する経路を作成して返す");
            Stack<Vector3> path = new();
            path.Push(startPos);
            return path;
        }

        // TOOD:頂点をソートして一番近い点を求めているので毎回ソートが2回入っている
        int startNumber = PosToNeighbourVertexNumber(startPos);
        int goalNumber = PosToNeighbourVertexNumber(goalPos);
        return _table[startNumber, goalNumber];
    }

    int PosToNeighbourVertexNumber(Vector3 pos)
    {
        Vertex neighbour = _graph.OrderBy(v => (v.transform.position - pos).sqrMagnitude).FirstOrDefault();
        return neighbour.Number;
    }

    /// <summary>
    /// グラフ上の全ての頂点の経路探索を行い、経路テーブルを作成する
    /// Editor上でステージに配置したオブジェクトを変更した際に実行される想定
    /// </summary>
    IEnumerator CreateTableCoroutine()
    {
#if UNITY_EDITOR
        Stopwatch stopwatch = new("経路テーブルの作成");
        stopwatch.Start();
#endif

        for (int i = 1; i <= _vertexObjects.Length; i++)
        {
            for (int k = 1; k <= _vertexObjects.Length; k++)
            {
                _table[i, k] = _aStarTask.Pathfinding(i, k);
            }
            yield return null;
        }

        // 経路テーブルの作成が完了したフラグを立てる
        _pathTableCreated = true;

#if UNITY_EDITOR
        stopwatch.Stop();
#endif
    }
}