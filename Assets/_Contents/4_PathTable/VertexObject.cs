using System.Collections.Generic;
using UnityEngine;

namespace PathTableGraph
{
    /// <summary>
    /// 各頂点の隣接している頂点をコレクションで保持するためのクラス
    /// </summary>
    public class Neighbour
    {
        VertexObject _vertex;
        float _distance;

        public Neighbour(VertexObject vertex, float distance)
        {
            _vertex = vertex;
            _distance = distance;
        }

        public VertexObject Vertex => _vertex;
        public float Distance => _distance;
    }

    /// <summary>
    /// 経路テーブルの頂点オブジェクト
    /// PathTableManagerクラスが持つ頂点のGameObjectに対してAddComponentされる
    /// </summary>
    public class VertexObject : MonoBehaviour
    {
        public List<Neighbour> _neighbourList = new List<Neighbour>();

        /// <summary>
        /// 1始まりの頂点番号
        /// </summary>
        public int Number { get; set; } = -1;

        /// <summary>
        /// 頂点とその距離を隣接しているリストに追加
        /// 頂点番号で追加しているわけではないので注意
        /// </summary>
        public void AddNeighbour(VertexObject vertex)
        {
            float distance = (vertex.transform.position - transform.position).sqrMagnitude;
            _neighbourList.Add(new Neighbour(vertex, distance));
        }

        void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                DrawNumberOnGizmos();
                DrawNeighbourEdgeOnGizmos();
            }
        }

        void DrawNumberOnGizmos()
        {
#if UNITY_EDITOR
            Vector3 pos = transform.position + Vector3.up * 1.5f;
            string str = $"頂点: {Number}";
            UnityEditor.Handles.color = Color.black;
            UnityEditor.Handles.Label(pos, str);
#endif
        }

        void DrawNeighbourEdgeOnGizmos()
        {
            for (int i = 0; i < _neighbourList.Count; i++)
            {
                Transform neighbour = _neighbourList[i].Vertex.transform;
                Gizmos.DrawLine(transform.position, neighbour.position);
            }
        }
    }
}
