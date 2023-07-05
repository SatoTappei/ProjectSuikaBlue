using System.Collections.Generic;
using UnityEngine;

namespace PathTableGraph
{
    /// <summary>
    /// �e���_�̗אڂ��Ă��钸�_���R���N�V�����ŕێ����邽�߂̃N���X
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
    /// �o�H�e�[�u���̒��_�I�u�W�F�N�g
    /// PathTableManager�N���X�������_��GameObject�ɑ΂���AddComponent�����
    /// </summary>
    public class VertexObject : MonoBehaviour
    {
        public List<Neighbour> _neighbourList = new List<Neighbour>();

        /// <summary>
        /// 1�n�܂�̒��_�ԍ�
        /// </summary>
        public int Number { get; set; } = -1;

        /// <summary>
        /// ���_�Ƃ��̋�����אڂ��Ă��郊�X�g�ɒǉ�
        /// ���_�ԍ��Œǉ����Ă���킯�ł͂Ȃ��̂Œ���
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
