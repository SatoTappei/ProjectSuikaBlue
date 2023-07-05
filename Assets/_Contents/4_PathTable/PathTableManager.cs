using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PathTableGraph;

// OverlapSphere�Ŏ���Ă������_�̃I�u�W�F�N�g�̔ԍ���m��K�v������
// ���̂��߂ɂ̓L�[��Transform�Œl���ԍ��̎����^���K�v�����A�ԍ��Œ��_������K�v������̂ł���͖���
// AddCOmponent�Œ��_�̃R���|�[�l���g��t�^������@�ŗǂ�

/// <summary>
/// �o�H�e�[�u�����쐬����N���X
/// </summary>
public class PathTableManager : MonoBehaviour
{
    [SerializeField] Transform[] _vertexObjects;
    [Header("�e���_�̃��C���[")]
    [SerializeField] LayerMask _vertexLayer;
    [Header("��Q���̃��C���[")]
    [SerializeField] LayerMask _obstacleLayer;
    [Header("���_�̃^�O")]
    [SerializeField] string _vertexTag = "Vertex";
    [Header("�t�߂̒��_�Ƃ��Č��o�ł��锼�a")]
    [SerializeField] float _neighbourRadius = 10;
    [Header("�M�Y���ւ̕`��")]
    [SerializeField] bool _drawGizmos = true;

    Vertex[] _vertices;
    List<Edge>[] _graph;
    Stack<Vector3> _path;

    void Awake()
    {
        // 1�n�܂�̒��_�ԍ��Ŏ擾�������̂� 0�Ԗڂ��_�~�[�ɂ��Ă���
        _vertices = new Vertex[_vertexObjects.Length + 1];
        _graph = new List<Edge>[_vertexObjects.Length + 1];

        CreateVertices();
        //CreateGraph();

        //_path = Pathfinding(1, 3);
    }

    /// <summary>
    /// �C���X�y�N�^�[�Ɋ��蓖�Ă����ɒ��_�ԍ���t�^���Ă���
    /// </summary>
    void CreateVertices()
    {
        for (int i = 0; i < _vertexObjects.Length; i++)
        {
            // 1�n�܂�Œ��_�ԍ��̕t�^
            VertexObject vertex = _vertexObjects[i].gameObject.AddComponent<VertexObject>();
            vertex.Number = i + 1;
            // ���͂̒��_���擾
            Vector3 vertexPos = _vertexObjects[i].position;
            Collider[] neighbours = Physics.OverlapSphere(vertexPos, _neighbourRadius, _vertexLayer);
            foreach (Collider neighbour in neighbours)
            {
                Vector3 dirVector = neighbour.transform.position - vertexPos;
                Vector3 dir = dirVector.normalized;
                // ��Q���ɎՂ��Ă��Ȃ����`�F�b�N
                LayerMask layerMask = _vertexLayer | _obstacleLayer;
                bool isHit = Physics.Raycast(vertexPos, dir, out RaycastHit hitInfo, _neighbourRadius, layerMask);
                if (isHit && hitInfo.collider.CompareTag(_vertexTag))
                {
                    // �אڂ��Ă��钸�_��ǉ�
                    if(hitInfo.collider.TryGetComponent(out VertexObject neighbourVertex))
                    {
                        vertex.AddNeighbour(neighbourVertex);
                    }
                }
            }
        }
    }

    /// <summary>
    /// �e���_�ɗאڂ��Ă��钸�_�����߂ăO���t���쐬����
    /// </summary>
    void CreateGraph()
    {
        for (int i = 1; i < _vertexObjects.Length + 1; i++)
        {
            _graph[i] = new();

            // ���͂̒��_���擾
            Vector3 vertexPos = _vertices[i].Transform.position;
            Collider[] neighbours = Physics.OverlapSphere(vertexPos, _neighbourRadius, _vertexLayer);
            foreach (Collider neighbour in neighbours)
            {
                Vector3 dirVector = neighbour.transform.position - vertexPos;
                // ��Q���ɎՂ��Ă��Ȃ����`�F�b�N
                LayerMask layerMask = _vertexLayer | _obstacleLayer;
                bool isHit = Physics.Raycast(vertexPos, dirVector.normalized, out RaycastHit hitInfo, _neighbourRadius, layerMask);
                if (isHit && hitInfo.collider.CompareTag(_vertexTag))
                {
                    // �Ⴊ�q�b�g�����炻�̔ԍ����K�v
                    _graph[i].Add(new Edge(_vertices[i], dirVector.sqrMagnitude));
                }
            }
        }
    }

    /// <summary>
    /// �w�肵�����_�ԍ����璸�_�ԍ��܂ł��o�H�T��
    /// </summary>
    public Stack<Vector3> Pathfinding(int startNumber, int goalNumber)
    {
        Vertex current = _vertices[startNumber];
        Vertex goal = _vertices[goalNumber];

        current.Node.HCost = CalculateHeuristicCost(startNumber, goalNumber);

        List<Vertex> openList = new() { current };
        HashSet<Vertex> closeSet = new();

        while (true)
        {
            if (openList.Count == 0)
            {
                Debug.LogWarning("�o�H��������Ȃ��̂œr���܂ł̌o�H���쐬");
                return CreatePath(current);
            }

            // �ŏ��R�X�g�̒��_
            current = openList.OrderBy(vertex => vertex.Node.FCost).FirstOrDefault();
            // �����ԍ��̏ꍇ�͌o�H�𐶐����ĕԂ�
            if (current.Number == goalNumber)
            {
                return CreatePath(current);
            }
            // �J�����m�[�h�̃��X�g��������m�[�h�̃��X�g�Ɉڂ�
            openList.Remove(current);
            closeSet.Add(current);
            // �אڂ������_�̃R�X�g���v�Z
            foreach (Edge neighbour in _graph[current.Number])
            {
                // �����m�[�h�̃��X�g�Ɋ܂܂�Ă�����e��
                if (closeSet.Contains(neighbour.Vertex)) continue;

                float gCost = current.Node.GCost + neighbour.Distance;
                float hCost = CalculateHeuristicCost(neighbour.Vertex.Number, goalNumber);
                float fCost = gCost + hCost;
                bool unContainedInOpenList = !openList.Contains(neighbour.Vertex);
                // �J�����m�[�h�̃��X�g�Ɋ܂܂�Ă��Ȃ�
                // �������͂��R�X�g���Ⴂ�ꍇ�́A�R�X�g�Ɛe���㏑��
                if (fCost < neighbour.Vertex.Node.FCost || unContainedInOpenList)
                {
                    neighbour.Vertex.Node.GCost = gCost;
                    neighbour.Vertex.Node.HCost = hCost;
                    neighbour.Vertex.Node.Parent = current;
                }
                // �m�[�h���J�����ꍇ�͊J�����m�[�h�̃��X�g�ɒǉ�
                if (unContainedInOpenList) openList.Add(neighbour.Vertex);
            }
        }
    }

    /// <summary>
    /// �q���[���X�e�B�b�N�R�X�g�̌v�Z
    /// �e�}�̃R�X�g�������Ɠ������S�[���܂ł̋�����2��
    /// </summary>
    float CalculateHeuristicCost(int currentNumber, int goalNumber)
    {
        Vector3 currentPos = _vertices[currentNumber].Transform.position;
        Vector3 goalPos = _vertices[goalNumber].Transform.position;
        return (goalPos - currentPos).sqrMagnitude;
    }

    /// <summary>
    /// �p�X�̐���
    /// ���_�̐e�̈ʒu��Stack�ɑ}�����Ă���
    /// </summary>
    Stack<Vector3> CreatePath(Vertex current)
    {
        Stack<Vector3> path = new();
        while (current.Node.Parent != null)
        {
            path.Push(current.Transform.position);
            current = current.Node.Parent;
        }
        path.Push(current.Transform.position);

        return path;
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying && _drawGizmos)
        {
            //DrawEdge();
            //DrawPath();
        }
    }

    /// <summary>
    /// �אڂ������_���m���q���ӂ�`�悷��
    /// </summary>
    void DrawEdge()
    {
        for (int i = 0; i < _vertexObjects.Length; i++)
        {
            Transform vertex = _vertexObjects[i].transform;
            int vertexNumber = i + 1;

            foreach (Edge neighbour in _graph[vertexNumber])
            {
                Gizmos.DrawLine(vertex.position, neighbour.Vertex.Transform.position);
            }
        }
    }

    /// <summary>
    /// ���������p�X��`�悷��
    /// </summary>
    void DrawPath()
    {
        if (_path == null) return;

        foreach (Vector3 pos in _path)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pos, 0.25f);
        }
    }
}