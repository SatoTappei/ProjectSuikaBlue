using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathTableGraph
{
    /// <summary>
    /// 経路テーブルで用いる頂点のクラス
    /// インスペクターから割り当てたオブジェクトに対して頂点番号を割り当てる
    /// </summary>
    public class Vertex
    {
        Node _node;
        Transform _transform;
        int _number;

        public Vertex(Transform transform, int number)
        {
            _transform = transform;
            _number = number;
        }

        public Node Node => _node;
        public Transform Transform => _transform;
        public int Number => _number;
    }

    /// <summary>
    /// 経路テーブルで用いる辺のクラス
    /// </summary>
    public class Edge
    {
        Vertex _vertex;
        float _distance;

        public Edge(Vertex vertex, float distance)
        {
            _vertex = vertex;
            _distance = distance;
        }

        public Vertex Vertex => _vertex;
        public float Distance => _distance;
    }

    /// <summary>
    /// 経路テーブルで用いる頂点から頂点への各種データのクラス
    /// </summary>
    public class Node
    {
        public Vertex Parent { get; set; }
        public float GCost { get; set; }
        public float HCost { get; set; }
        public float FCost => GCost + HCost;
    }
}
