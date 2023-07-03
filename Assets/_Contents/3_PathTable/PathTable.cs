using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �o�H�e�[�u���ɂ��o�H�T��
/// </summary>
public class PathTable : MonoBehaviour
{
    class Vertex
    {
        public int Number { get; set; }
        public float Distance { get; set; }
    }

    [SerializeField] GameObject[] _vertices;
    [Header("�e���_�̃��C���[")]
    [SerializeField] LayerMask _vertexLayer;
    [Header("���_�Ə�Q���̃��C���[")]
    [SerializeField] LayerMask _vertexAndObstacleLayer;
    [Header("�t�߂̒��_�Ƃ��Č��o�ł��锼�a")]
    [SerializeField] float _neighbourRadius = 10;
    [Header("�M�Y���ւ̕`��")]
    [SerializeField] bool _drawGizmos = true;

    List<Vertex>[] _graph;
    List<int>[] _table;

    void Awake()
    {
        // �e�n�_����S�Ă̒n�_�ւ̍ŒZ�o�H�����߂ĕ\�ɂ��Ă���
        // �n�_v�Ɛڑ�����Ă���n�_�̏�񂪕K�v
        // �n�_v�Ɛڑ�����Ă���e�n�_�Ƃ̋������K�v

        // �O���t�ƃe�[�u���͒��_�ԍ��Ŏ擾�������̂� 0�Ԗڂ��_�~�[�ɂ��Ă���
        _graph = new List<Vertex>[_vertices.Length + 1];
        _table = new List<int>[_vertices.Length + 1];

        AddVertexNumber();
        CreateGraph();
    }

    /// <summary>
    /// �C���X�y�N�^�[�Ɋ��蓖�Ă����ɒ��_�ԍ���t�^���Ă���
    /// </summary>
    void AddVertexNumber()
    {
        for (int i = 0; i < _vertices.Length; i++)
        {
            PathVertex vertex = _vertices[i].AddComponent<PathVertex>();
            // ���_�ԍ��� 1 �n�܂�Ȃ̂Œ���
            vertex.Number = i + 1;
        }
    }

    /// <summary>
    /// �e���_�ɗאڂ��Ă��钸�_�����߂ăO���t���쐬����
    /// </summary>
    void CreateGraph()
    {
        for (int i = 0; i < _vertices.Length; i++)
        {
            Transform vertex = _vertices[i].transform;

            // ���͂̒��_���擾
            Collider[] neighbours = Physics.OverlapSphere(vertex.position, _neighbourRadius, _vertexLayer);
            foreach (Collider neighbour in neighbours)
            {
                Vector3 dirVector = neighbour.transform.position - vertex.position;
                Vector3 dir = dirVector.normalized;
                // ��Q���ɎՂ��Ă��Ȃ����`�F�b�N
                bool isHit = Physics.Raycast(vertex.position, dir, out RaycastHit hitInfo, _neighbourRadius, _vertexAndObstacleLayer);
                if (isHit && hitInfo.collider.CompareTag("Vertex"))
                {
                    // �אڒ��_�̔ԍ��Ƌ���
                    // ���_�ԍ��Ŏ擾����̂� 0 �Ԗڂ̓_�~�[�Ȃ̂� 1 �Ԗڂ���ǉ����Ă���
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
