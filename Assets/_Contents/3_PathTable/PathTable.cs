using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 経路テーブルによる経路探索
/// </summary>
public class PathTable : MonoBehaviour
{
    class Vertex
    {
        public int Number { get; set; }
        public float Distance { get; set; }
    }

    [SerializeField] GameObject[] _vertices;
    [Header("各頂点のレイヤー")]
    [SerializeField] LayerMask _vertexLayer;
    [Header("頂点と障害物のレイヤー")]
    [SerializeField] LayerMask _vertexAndObstacleLayer;
    [Header("付近の頂点として検出できる半径")]
    [SerializeField] float _neighbourRadius = 10;
    [Header("ギズモへの描画")]
    [SerializeField] bool _drawGizmos = true;

    List<Vertex>[] _graph;
    List<int>[] _table;

    void Awake()
    {
        // 各地点から全ての地点への最短経路を求めて表にしておく
        // 地点vと接続されている地点の情報が必要
        // 地点vと接続されている各地点との距離が必要

        // グラフとテーブルは頂点番号で取得したいので 0番目をダミーにしておく
        _graph = new List<Vertex>[_vertices.Length + 1];
        _table = new List<int>[_vertices.Length + 1];

        AddVertexNumber();
        CreateGraph();
    }

    /// <summary>
    /// インスペクターに割り当てた順に頂点番号を付与していく
    /// </summary>
    void AddVertexNumber()
    {
        for (int i = 0; i < _vertices.Length; i++)
        {
            PathVertex vertex = _vertices[i].AddComponent<PathVertex>();
            // 頂点番号は 1 始まりなので注意
            vertex.Number = i + 1;
        }
    }

    /// <summary>
    /// 各頂点に隣接している頂点を求めてグラフを作成する
    /// </summary>
    void CreateGraph()
    {
        for (int i = 0; i < _vertices.Length; i++)
        {
            Transform vertex = _vertices[i].transform;

            // 周囲の頂点を取得
            Collider[] neighbours = Physics.OverlapSphere(vertex.position, _neighbourRadius, _vertexLayer);
            foreach (Collider neighbour in neighbours)
            {
                Vector3 dirVector = neighbour.transform.position - vertex.position;
                Vector3 dir = dirVector.normalized;
                // 障害物に遮られていないかチェック
                bool isHit = Physics.Raycast(vertex.position, dir, out RaycastHit hitInfo, _neighbourRadius, _vertexAndObstacleLayer);
                if (isHit && hitInfo.collider.CompareTag("Vertex"))
                {
                    // 隣接頂点の番号と距離
                    // 頂点番号で取得するので 0 番目はダミーなので 1 番目から追加していく
                    _graph[i + 1].Add(new Vertex()
                    {
                        Number = neighbour.GetComponent<PathVertex>().Number,
                        Distance = dirVector.sqrMagnitude,
                    });
                }
            }
        }
    }

    void Pathfinding(int startIndex, int goalIndex)
    {


        //List<Vertex> openList = new() { current };
        //HashSet<Vertex> closeList = new();

        //while (true)
        //{

        //}
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying || !_drawGizmos) return;

        for (int i = 0; i < _vertices.Length; i++)
        {
            foreach (Vertex neighbour in _graph[i])
            {
                Gizmos.DrawLine(_vertices[i].transform.position, _vertices[neighbour.Number].transform.position);
            }
        }
    }


}
