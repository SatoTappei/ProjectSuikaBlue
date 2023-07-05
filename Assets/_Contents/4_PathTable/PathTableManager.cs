using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PathTableGraph;

// OverlapSphereで取ってきた頂点のオブジェクトの番号を知る必要がある
// そのためにはキーがTransformで値が番号の辞書型が必要だが、番号で頂点も取れる必要があるのでそれは無し
// AddCOmponentで頂点のコンポーネントを付与する方法で良く

/// <summary>
/// 経路テーブルを作成するクラス
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

    Vertex[] _vertices;
    List<Edge>[] _graph;
    Stack<Vector3> _path;

    void Awake()
    {
        // 1始まりの頂点番号で取得したいので 0番目をダミーにしておく
        _vertices = new Vertex[_vertexObjects.Length + 1];
        _graph = new List<Edge>[_vertexObjects.Length + 1];

        CreateVertices();
        //CreateGraph();

        //_path = Pathfinding(1, 3);
    }

    /// <summary>
    /// インスペクターに割り当てた順に頂点番号を付与していく
    /// </summary>
    void CreateVertices()
    {
        for (int i = 0; i < _vertexObjects.Length; i++)
        {
            // 1始まりで頂点番号の付与
            VertexObject vertex = _vertexObjects[i].gameObject.AddComponent<VertexObject>();
            vertex.Number = i + 1;
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
                    if(hitInfo.collider.TryGetComponent(out VertexObject neighbourVertex))
                    {
                        vertex.AddNeighbour(neighbourVertex);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 各頂点に隣接している頂点を求めてグラフを作成する
    /// </summary>
    void CreateGraph()
    {
        for (int i = 1; i < _vertexObjects.Length + 1; i++)
        {
            _graph[i] = new();

            // 周囲の頂点を取得
            Vector3 vertexPos = _vertices[i].Transform.position;
            Collider[] neighbours = Physics.OverlapSphere(vertexPos, _neighbourRadius, _vertexLayer);
            foreach (Collider neighbour in neighbours)
            {
                Vector3 dirVector = neighbour.transform.position - vertexPos;
                // 障害物に遮られていないかチェック
                LayerMask layerMask = _vertexLayer | _obstacleLayer;
                bool isHit = Physics.Raycast(vertexPos, dirVector.normalized, out RaycastHit hitInfo, _neighbourRadius, layerMask);
                if (isHit && hitInfo.collider.CompareTag(_vertexTag))
                {
                    // 例がヒットしたらその番号が必要
                    _graph[i].Add(new Edge(_vertices[i], dirVector.sqrMagnitude));
                }
            }
        }
    }

    /// <summary>
    /// 指定した頂点番号から頂点番号までを経路探索
    /// </summary>
    public Stack<Vector3> Pathfinding(int startNumber, int goalNumber)
    {
        Vertex current = _vertices[startNumber];
        Vertex goal = _vertices[goalNumber];

        current.Node.HCost = CalculateHeuristicCost(startNumber, goalNumber);

        List<Vertex> openList = new() { current };
        HashSet<Vertex> closeSet = new();

        while (true)
        {
            if (openList.Count == 0)
            {
                Debug.LogWarning("経路が見つからないので途中までの経路を作成");
                return CreatePath(current);
            }

            // 最小コストの頂点
            current = openList.OrderBy(vertex => vertex.Node.FCost).FirstOrDefault();
            // 同じ番号の場合は経路を生成して返す
            if (current.Number == goalNumber)
            {
                return CreatePath(current);
            }
            // 開いたノードのリストから閉じたノードのリストに移す
            openList.Remove(current);
            closeSet.Add(current);
            // 隣接した頂点のコストを計算
            foreach (Edge neighbour in _graph[current.Number])
            {
                // 閉じたノードのリストに含まれていたら弾く
                if (closeSet.Contains(neighbour.Vertex)) continue;

                float gCost = current.Node.GCost + neighbour.Distance;
                float hCost = CalculateHeuristicCost(neighbour.Vertex.Number, goalNumber);
                float fCost = gCost + hCost;
                bool unContainedInOpenList = !openList.Contains(neighbour.Vertex);
                // 開いたノードのリストに含まれていない
                // もしくはよりコストが低い場合は、コストと親を上書き
                if (fCost < neighbour.Vertex.Node.FCost || unContainedInOpenList)
                {
                    neighbour.Vertex.Node.GCost = gCost;
                    neighbour.Vertex.Node.HCost = hCost;
                    neighbour.Vertex.Node.Parent = current;
                }
                // ノードを開いた場合は開いたノードのリストに追加
                if (unContainedInOpenList) openList.Add(neighbour.Vertex);
            }
        }
    }

    /// <summary>
    /// ヒューリスティックコストの計算
    /// 各枝のコストが距離と同じくゴールまでの距離の2乗
    /// </summary>
    float CalculateHeuristicCost(int currentNumber, int goalNumber)
    {
        Vector3 currentPos = _vertices[currentNumber].Transform.position;
        Vector3 goalPos = _vertices[goalNumber].Transform.position;
        return (goalPos - currentPos).sqrMagnitude;
    }

    /// <summary>
    /// パスの生成
    /// 頂点の親の位置をStackに挿入していく
    /// </summary>
    Stack<Vector3> CreatePath(Vertex current)
    {
        Stack<Vector3> path = new();
        while (current.Node.Parent != null)
        {
            path.Push(current.Transform.position);
            current = current.Node.Parent;
        }
        path.Push(current.Transform.position);

        return path;
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying && _drawGizmos)
        {
            //DrawEdge();
            //DrawPath();
        }
    }

    /// <summary>
    /// 隣接した頂点同士を繋ぐ辺を描画する
    /// </summary>
    void DrawEdge()
    {
        for (int i = 0; i < _vertexObjects.Length; i++)
        {
            Transform vertex = _vertexObjects[i].transform;
            int vertexNumber = i + 1;

            foreach (Edge neighbour in _graph[vertexNumber])
            {
                Gizmos.DrawLine(vertex.position, neighbour.Vertex.Transform.position);
            }
        }
    }

    /// <summary>
    /// 生成したパスを描画する
    /// </summary>
    void DrawPath()
    {
        if (_path == null) return;

        foreach (Vector3 pos in _path)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pos, 0.25f);
        }
    }
}