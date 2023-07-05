//using System.Linq;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//// 自作便利クラス
//using CommonUtility;

///// <summary>
///// 経路テーブルによる経路探索
///// </summary>
//public class PathTableOld : MonoBehaviour
//{
//    class Vertex
//    {
//        public Transform Transform { get; set; }
//        public int Number { get; set; }
//        public float Distance { get; set; }
//        public float GCost { get; set; }
//        public float HCost { get; set; }
//        public float FCost => GCost + HCost;
//        public Vertex Parent { get; set; }
//    }

//    [SerializeField] GameObject[] _vertexObjects;
//    [Header("各頂点のレイヤー")]
//    [SerializeField] LayerMask _vertexLayer;
//    [Header("頂点と障害物のレイヤー")]
//    [SerializeField] LayerMask _vertexAndObstacleLayer;
//    [Header("頂点のタグ")]
//    [SerializeField] string _vertexTag = "Vertex";
//    [Header("付近の頂点として検出できる半径")]
//    [SerializeField] float _neighbourRadius = 10;
//    [Header("ギズモへの描画")]
//    [SerializeField] bool _drawGizmos = true;

//    Vertex[] _vertices;
//    List<Vertex>[] _graph;
//    List<int>[] _table;
//    Stack<Vector3> _path;

//    void Awake()
//    {
//        // 各地点から全ての地点への最短経路を求めて表にしておく
//        // 地点vと接続されている地点の情報が必要
//        // 地点vと接続されている各地点との距離が必要

//        // 頂点番号で取得したいので 0番目をダミーにしておく
//        _vertices = new Vertex[_vertexObjects.Length + 1];
//        _graph = new List<Vertex>[_vertexObjects.Length + 1];
//        _table = new List<int>[_vertexObjects.Length + 1];

//        AddVertexNumber();
//        CreateGraph();

//        _path = Pathfinding(1, 3);
//    }

//    /// <summary>
//    /// インスペクターに割り当てた順に頂点番号を付与していく
//    /// </summary>
//    void AddVertexNumber()
//    {
//        for (int i = 0; i < _vertexObjects.Length; i++)
//        {
//            PathVertex vertex = _vertexObjects[i].AddComponent<PathVertex>();
//            // 頂点番号は 1 始まりなので注意
//            vertex.Number = i + 1;
//        }
//    }

//    /// <summary>
//    /// 各頂点に隣接している頂点を求めてグラフを作成する
//    /// </summary>
//    void CreateGraph()
//    {
//        for (int i = 0; i < _vertexObjects.Length; i++)
//        {
//            Transform vertex = _vertexObjects[i].transform;
//            _graph[i + 1] = new();

//            // 周囲の頂点を取得
//            Collider[] neighbours = Physics.OverlapSphere(vertex.position, _neighbourRadius, _vertexLayer);
//            foreach (Collider neighbour in neighbours)
//            {
//                Vector3 dirVector = neighbour.transform.position - vertex.position;
//                Vector3 dir = dirVector.normalized;
//                // 障害物に遮られていないかチェック
//                bool isHit = Physics.Raycast(vertex.position, dir, out RaycastHit hitInfo, _neighbourRadius, _vertexAndObstacleLayer);
//                if (isHit && hitInfo.collider.CompareTag(_vertexTag))
//                {
//                    // 隣接頂点の番号と距離
//                    // 頂点番号で取得するので 0 番目はダミーなので 1 番目から追加していく
//                    _graph[i + 1].Add(new Vertex()
//                    {
//                        Transform = neighbour.transform,
//                        Number = neighbour.GetComponent<PathVertex>().Number,
//                        Distance = dirVector.sqrMagnitude,
//                    });
//                }
//            }
//        }
//    }

//    /// <summary>
//    /// 指定した頂点番号から頂点番号までを経路探索
//    /// </summary>
//    public Stack<Vector3> Pathfinding(int startNumber, int goalNumber)
//    {
//        Vertex current = _vertices[startNumber];
//        Vertex goal = _vertices[goalNumber];

//        current.HCost = CalculateHeuristicCost(startNumber, goalNumber);

//        List<Vertex> openList = new() { current };
//        HashSet<Vertex> closeSet = new();

//        while (true)
//        {
//            if (openList.Count == 0)
//            {
//                Debug.LogWarning("経路が見つからないので途中までの経路を作成");
//                return CreatePath(current);
//            }

//            // 最小コストの頂点
//            current = openList.OrderBy(v => v.Distance).FirstOrDefault();
//            // 同じ番号の場合は経路を生成して返す
//            if (current.Number == goalNumber)
//            {
//                return CreatePath(current);
//            }
//            // 開いたノードのリストから閉じたノードのリストに移す
//            openList.Remove(current);
//            closeSet.Add(current);
//            // 隣接した頂点のコストを計算
//            foreach(Vertex neighbour in _graph[current.Number])
//            {
//                // 閉じたノードのリストに含まれていたら弾く
//                if (closeSet.Contains(neighbour)) continue;

//                float gCost = current.GCost + neighbour.Distance;
//                float hCost = CalculateHeuristicCost(neighbour.Number, goalNumber);
//                float fCost = gCost + hCost;
//                bool unContainedInOpenList = !openList.Contains(neighbour);
//                // 開いたノードのリストに含まれていない
//                // もしくはよりコストが低い場合は、コストと親を上書き
//                if (fCost < neighbour.FCost || unContainedInOpenList)
//                {
//                    neighbour.GCost = gCost;
//                    neighbour.HCost = hCost;
//                    neighbour.Parent = current;
//                }
//                // ノードを開いた場合は開いたノードのリストに追加
//                if (unContainedInOpenList) openList.Add(neighbour);
//            }
//        }
//    }

//    /// <summary>
//    /// ヒューリスティックコストの計算
//    /// 各枝のコストが距離と同じくゴールまでの距離の2乗
//    /// </summary>
//    float CalculateHeuristicCost(int currentNumber, int goalNumber)
//    {
//        Vector3 currentPos = _vertices[currentNumber].Transform.position;
//        Vector3 goalPos = _vertices[goalNumber].Transform.position;
//        return (goalPos - currentPos).sqrMagnitude;
//    }

//    /// <summary>
//    /// パスの生成
//    /// 頂点の親の位置をStackに挿入していく
//    /// </summary>
//    Stack<Vector3> CreatePath(Vertex current)
//    {
//        Stack<Vector3> path = new();
//        while (current.Parent != null)
//        {
//            path.Push(current.Transform.position);
//            current = current.Parent;
//        }
//        path.Push(current.Transform.position);

//        return path;
//    }

//    void OnDrawGizmos()
//    {
//        if (Application.isPlaying && _drawGizmos)
//        {
//            DrawEdge();
//            DrawPath();
//        }
//    }

//    /// <summary>
//    /// 隣接した頂点同士を繋ぐ辺を描画する
//    /// </summary>
//    void DrawEdge()
//    {
//        for (int i = 0; i < _vertexObjects.Length; i++)
//        {
//            Transform vertex = _vertexObjects[i].transform;
//            int vertexNumber = i + 1;

//            foreach (Vertex neighbour in _graph[vertexNumber])
//            {
//                Gizmos.DrawLine(vertex.position, neighbour.Transform.position);
//            }
//        }
//    }

//    /// <summary>
//    /// 生成したパスを描画する
//    /// </summary>
//    void DrawPath()
//    {
//        if (_path == null) return;

//        foreach(Vector3 pos in _path)
//        {
//            Gizmos.color = Color.red;
//            Gizmos.DrawSphere(pos, 0.25f);
//        }
//    }
//}
