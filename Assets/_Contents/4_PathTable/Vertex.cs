using System.Collections.Generic;
using UnityEngine;

namespace PathTableGraph
{
    /// <summary>
    /// �e���_�̗אڂ��Ă��钸�_���R���N�V�����ŕێ����邽�߂̃N���X
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
    /// �o�H�e�[�u���̒��_�I�u�W�F�N�g
    /// PathTableManager�N���X�������_��GameObject�ɑ΂���AddComponent�����
    /// </summary>
    public class Vertex : MonoBehaviour, IBinaryHeapCollectable<Vertex>
    {
        /// <summary>
        /// �אڂ������_�̃��X�g
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
        /// ���_�Ƃ��̋�����אڂ��Ă��郊�X�g�ɒǉ�
        /// ���_�ԍ��Œǉ����Ă���킯�ł͂Ȃ��̂Œ���
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
            string str = $"���_: {Number}";
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
