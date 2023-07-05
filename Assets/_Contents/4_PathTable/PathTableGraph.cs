using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathTableGraph
{
    /// <summary>
    /// �o�H�e�[�u���ŗp���钸�_�̃N���X
    /// �C���X�y�N�^�[���犄�蓖�Ă��I�u�W�F�N�g�ɑ΂��Ē��_�ԍ������蓖�Ă�
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
    /// �o�H�e�[�u���ŗp����ӂ̃N���X
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
    /// �o�H�e�[�u���ŗp���钸�_���璸�_�ւ̊e��f�[�^�̃N���X
    /// </summary>
    public class Node
    {
        public Vertex Parent { get; set; }
        public float GCost { get; set; }
        public float HCost { get; set; }
        public float FCost => GCost + HCost;
    }
}
