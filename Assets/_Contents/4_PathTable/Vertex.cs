using System.Collections.Generic;
using UnityEngine;

namespace PathTableGraph
{
    /// <summary>
    /// 各頂点の隣接している頂点をコレクションで保持するためのクラス
    /// </summary>
    public class Neighbour
    {
        Vertex _vertex;
        float _distance;

        public Neighbour(Vertex vertex, float distance)
        {
            _vertex = vertex;
            _distance = distance;
        }

        public Vertex Vertex => _vertex;
        public float Distance => _distance;
    }

    /// <summary>
    /// 経路テーブルの頂点オブジェクト
    /// PathTableManagerクラスが持つ頂点のGameObjectに対してAddComponentされる
    /// </summary>
    public class Vertex : MonoBehaviour, IBinaryHeapCollectable<Vertex>
    {
        /// <summary>
        /// 隣接した頂点のリスト
        /// </summary>
        List<Neighbour> _neighbourList = new List<Neighbour>();

        public IReadOnlyList<Neighbour> NeighbourList => _neighbourList;
        public Vertex Parent { get; set; }
        public int Number { get; set; } = -1;
        public float GCost { get; set; }
        public float HCost { get; set; }
        public float FCost => GCost + HCost;
        public int BinaryHeapIndex { get; set; }

        public int CompareTo(Vertex other)
        {
            int result = FCost.CompareTo(other.FCost);
            if (result == 0)
            {
                result = HCost.CompareTo(other.HCost);
            }

            return result;
        }

        /// <summary>
        /// 頂点とその距離を隣接しているリストに追加
        /// 頂点番号で追加しているわけではないので注意
        /// </summary>
        public void AddNeighbour(Vertex vertex)
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
