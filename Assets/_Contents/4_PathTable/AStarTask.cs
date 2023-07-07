using System.Collections.Generic;
using UnityEngine;

namespace PathTableGraph
{
    /// <summary>
    /// �o�H�e�[�u���ŗp����A*�����s����N���X
    /// </summary>
    public class AStarTask : IPathfinding
    {
        Vertex[] _graph;

        public AStarTask(Vertex[] graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// �w�肵�����_�ԍ����璸�_�ԍ��܂ł��o�H�T��
        /// �O������R�����\�b�h���ĂԂ��Ƃŋ@�\����
        /// </summary>
        public Stack<Vector3> Pathfinding(int startNumber, int goalNumber)
        {
            ResetCost();

            Vertex current = _graph[startNumber];
            current.HCost = CalculateHeuristicCost(startNumber, goalNumber);

            BinaryHeap<Vertex> openHeap = new(_graph.Length);
            openHeap.Add(current);
            HashSet<Vertex> closeSet = new();

            while (true)
            {
                if (openHeap.Count == 0)
                {
                    Debug.LogWarning("�o�H��������Ȃ��̂œr���܂ł̌o�H���쐬");
                    return CreatePath(current);
                }

                // �ŏ��R�X�g�̒��_
                current = openHeap.Pop();
                // �����ԍ��̏ꍇ�͌o�H�𐶐����ĕԂ�
                if (current.Number == goalNumber)
                {
                    return CreatePath(current);
                }
                // �J�����m�[�h�̃��X�g��������m�[�h�̃��X�g�Ɉڂ�
                closeSet.Add(current);
                // �אڂ������_�̃R�X�g���v�Z
                foreach (Neighbour neighbour in current.NeighbourList)
                {
                    // �����m�[�h�̃��X�g�Ɋ܂܂�Ă�����e��
                    if (closeSet.Contains(neighbour.Vertex)) continue;

                    float gCost = current.GCost + neighbour.Distance;
                    float hCost = CalculateHeuristicCost(neighbour.Vertex.Number, goalNumber);
                    float fCost = gCost + hCost;
                    bool unContainedInOpenList = !openHeap.Contains(neighbour.Vertex);

                    // �J�����m�[�h�̃��X�g�Ɋ܂܂�Ă��Ȃ�
                    // �������͂��R�X�g���Ⴂ�ꍇ�́A�R�X�g�Ɛe���㏑��
                    if (fCost < neighbour.Vertex.FCost || unContainedInOpenList)
                    {
                        neighbour.Vertex.GCost = gCost;
                        neighbour.Vertex.HCost = hCost;
                        neighbour.Vertex.Parent = current;
                    }
                    // �m�[�h���J�����ꍇ�͊J�����m�[�h�̃��X�g�ɒǉ�
                    if (unContainedInOpenList) openHeap.Add(neighbour.Vertex);
                }
            }
        }

        /// <summary>
        /// �o�H�T���̑O�Ɋe���_�̃R�X�g�����Z�b�g����
        /// </summary>
        void ResetCost()
        {
            for (int i = 1; i < _graph.Length; i++)
            {
                _graph[i].GCost = float.MaxValue;
                _graph[i].HCost = float.MaxValue;
                _graph[i].Parent = null;
            }
        }

        /// <summary>
        /// �q���[���X�e�B�b�N�R�X�g�̌v�Z
        /// �e�}�̃R�X�g�������Ɠ������S�[���܂ł̋�����2��
        /// </summary>
        float CalculateHeuristicCost(int currentNumber, int goalNumber)
        {
            Vector3 currentPos = _graph[currentNumber].transform.position;
            Vector3 goalPos = _graph[goalNumber].transform.position;
            return (goalPos - currentPos).sqrMagnitude;
        }

        /// <summary>
        /// �p�X�̐���
        /// ���_�̐e�̈ʒu��Stack�ɑ}�����Ă���
        /// </summary>
        Stack<Vector3> CreatePath(Vertex current)
        {
            Stack<Vector3> path = new();
            while (current.Parent != null)
            {
                path.Push(current.transform.position);
                current = current.Parent;
            }
            path.Push(current.transform.position);

            return path;
        }
    }
}
