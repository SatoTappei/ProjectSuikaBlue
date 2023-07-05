//using System.Linq;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//// ����֗��N���X
//using CommonUtility;

///// <summary>
///// �o�H�e�[�u���ɂ��o�H�T��
///// </summary>
//public class PathTableOld : MonoBehaviour
//{
//    class Vertex
//    {
//        public Transform Transform { get; set; }
//        public int Number { get; set; }
//        public float Distance { get; set; }
//        public float GCost { get; set; }
//        public float HCost { get; set; }
//        public float FCost => GCost + HCost;
//        public Vertex Parent { get; set; }
//    }

//    [SerializeField] GameObject[] _vertexObjects;
//    [Header("�e���_�̃��C���[")]
//    [SerializeField] LayerMask _vertexLayer;
//    [Header("���_�Ə�Q���̃��C���[")]
//    [SerializeField] LayerMask _vertexAndObstacleLayer;
//    [Header("���_�̃^�O")]
//    [SerializeField] string _vertexTag = "Vertex";
//    [Header("�t�߂̒��_�Ƃ��Č��o�ł��锼�a")]
//    [SerializeField] float _neighbourRadius = 10;
//    [Header("�M�Y���ւ̕`��")]
//    [SerializeField] bool _drawGizmos = true;

//    Vertex[] _vertices;
//    List<Vertex>[] _graph;
//    List<int>[] _table;
//    Stack<Vector3> _path;

//    void Awake()
//    {
//        // �e�n�_����S�Ă̒n�_�ւ̍ŒZ�o�H�����߂ĕ\�ɂ��Ă���
//        // �n�_v�Ɛڑ�����Ă���n�_�̏�񂪕K�v
//        // �n�_v�Ɛڑ�����Ă���e�n�_�Ƃ̋������K�v

//        // ���_�ԍ��Ŏ擾�������̂� 0�Ԗڂ��_�~�[�ɂ��Ă���
//        _vertices = new Vertex[_vertexObjects.Length + 1];
//        _graph = new List<Vertex>[_vertexObjects.Length + 1];
//        _table = new List<int>[_vertexObjects.Length + 1];

//        AddVertexNumber();
//        CreateGraph();

//        _path = Pathfinding(1, 3);
//    }

//    /// <summary>
//    /// �C���X�y�N�^�[�Ɋ��蓖�Ă����ɒ��_�ԍ���t�^���Ă���
//    /// </summary>
//    void AddVertexNumber()
//    {
//        for (int i = 0; i < _vertexObjects.Length; i++)
//        {
//            PathVertex vertex = _vertexObjects[i].AddComponent<PathVertex>();
//            // ���_�ԍ��� 1 �n�܂�Ȃ̂Œ���
//            vertex.Number = i + 1;
//        }
//    }

//    /// <summary>
//    /// �e���_�ɗאڂ��Ă��钸�_�����߂ăO���t���쐬����
//    /// </summary>
//    void CreateGraph()
//    {
//        for (int i = 0; i < _vertexObjects.Length; i++)
//        {
//            Transform vertex = _vertexObjects[i].transform;
//            _graph[i + 1] = new();

//            // ���͂̒��_���擾
//            Collider[] neighbours = Physics.OverlapSphere(vertex.position, _neighbourRadius, _vertexLayer);
//            foreach (Collider neighbour in neighbours)
//            {
//                Vector3 dirVector = neighbour.transform.position - vertex.position;
//                Vector3 dir = dirVector.normalized;
//                // ��Q���ɎՂ��Ă��Ȃ����`�F�b�N
//                bool isHit = Physics.Raycast(vertex.position, dir, out RaycastHit hitInfo, _neighbourRadius, _vertexAndObstacleLayer);
//                if (isHit && hitInfo.collider.CompareTag(_vertexTag))
//                {
//                    // �אڒ��_�̔ԍ��Ƌ���
//                    // ���_�ԍ��Ŏ擾����̂� 0 �Ԗڂ̓_�~�[�Ȃ̂� 1 �Ԗڂ���ǉ����Ă���
//                    _graph[i + 1].Add(new Vertex()
//                    {
//                        Transform = neighbour.transform,
//                        Number = neighbour.GetComponent<PathVertex>().Number,
//                        Distance = dirVector.sqrMagnitude,
//                    });
//                }
//            }
//        }
//    }

//    /// <summary>
//    /// �w�肵�����_�ԍ����璸�_�ԍ��܂ł��o�H�T��
//    /// </summary>
//    public Stack<Vector3> Pathfinding(int startNumber, int goalNumber)
//    {
//        Vertex current = _vertices[startNumber];
//        Vertex goal = _vertices[goalNumber];

//        current.HCost = CalculateHeuristicCost(startNumber, goalNumber);

//        List<Vertex> openList = new() { current };
//        HashSet<Vertex> closeSet = new();

//        while (true)
//        {
//            if (openList.Count == 0)
//            {
//                Debug.LogWarning("�o�H��������Ȃ��̂œr���܂ł̌o�H���쐬");
//                return CreatePath(current);
//            }

//            // �ŏ��R�X�g�̒��_
//            current = openList.OrderBy(v => v.Distance).FirstOrDefault();
//            // �����ԍ��̏ꍇ�͌o�H�𐶐����ĕԂ�
//            if (current.Number == goalNumber)
//            {
//                return CreatePath(current);
//            }
//            // �J�����m�[�h�̃��X�g��������m�[�h�̃��X�g�Ɉڂ�
//            openList.Remove(current);
//            closeSet.Add(current);
//            // �אڂ������_�̃R�X�g���v�Z
//            foreach(Vertex neighbour in _graph[current.Number])
//            {
//                // �����m�[�h�̃��X�g�Ɋ܂܂�Ă�����e��
//                if (closeSet.Contains(neighbour)) continue;

//                float gCost = current.GCost + neighbour.Distance;
//                float hCost = CalculateHeuristicCost(neighbour.Number, goalNumber);
//                float fCost = gCost + hCost;
//                bool unContainedInOpenList = !openList.Contains(neighbour);
//                // �J�����m�[�h�̃��X�g�Ɋ܂܂�Ă��Ȃ�
//                // �������͂��R�X�g���Ⴂ�ꍇ�́A�R�X�g�Ɛe���㏑��
//                if (fCost < neighbour.FCost || unContainedInOpenList)
//                {
//                    neighbour.GCost = gCost;
//                    neighbour.HCost = hCost;
//                    neighbour.Parent = current;
//                }
//                // �m�[�h���J�����ꍇ�͊J�����m�[�h�̃��X�g�ɒǉ�
//                if (unContainedInOpenList) openList.Add(neighbour);
//            }
//        }
//    }

//    /// <summary>
//    /// �q���[���X�e�B�b�N�R�X�g�̌v�Z
//    /// �e�}�̃R�X�g�������Ɠ������S�[���܂ł̋�����2��
//    /// </summary>
//    float CalculateHeuristicCost(int currentNumber, int goalNumber)
//    {
//        Vector3 currentPos = _vertices[currentNumber].Transform.position;
//        Vector3 goalPos = _vertices[goalNumber].Transform.position;
//        return (goalPos - currentPos).sqrMagnitude;
//    }

//    /// <summary>
//    /// �p�X�̐���
//    /// ���_�̐e�̈ʒu��Stack�ɑ}�����Ă���
//    /// </summary>
//    Stack<Vector3> CreatePath(Vertex current)
//    {
//        Stack<Vector3> path = new();
//        while (current.Parent != null)
//        {
//            path.Push(current.Transform.position);
//            current = current.Parent;
//        }
//        path.Push(current.Transform.position);

//        return path;
//    }

//    void OnDrawGizmos()
//    {
//        if (Application.isPlaying && _drawGizmos)
//        {
//            DrawEdge();
//            DrawPath();
//        }
//    }

//    /// <summary>
//    /// �אڂ������_���m���q���ӂ�`�悷��
//    /// </summary>
//    void DrawEdge()
//    {
//        for (int i = 0; i < _vertexObjects.Length; i++)
//        {
//            Transform vertex = _vertexObjects[i].transform;
//            int vertexNumber = i + 1;

//            foreach (Vertex neighbour in _graph[vertexNumber])
//            {
//                Gizmos.DrawLine(vertex.position, neighbour.Transform.position);
//            }
//        }
//    }

//    /// <summary>
//    /// ���������p�X��`�悷��
//    /// </summary>
//    void DrawPath()
//    {
//        if (_path == null) return;

//        foreach(Vector3 pos in _path)
//        {
//            Gizmos.color = Color.red;
//            Gizmos.DrawSphere(pos, 0.25f);
//        }
//    }
//}
