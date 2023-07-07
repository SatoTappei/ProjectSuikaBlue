using PathTableGraph;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �O���t��̌o�H�T�����s���A�e�L�X�g�A�Z�b�g�Ƃ��ď����o���N���X
/// ���O�ɃV�[����ɒ��_�ƂȂ�I�u�W�F�N�g��z�u���Ă���K�v������
/// Vertex�^�O/���C���[�̒��_�I�u�W�F�N�g�AObstacle���C���[�̏�Q�����K�v
/// </summary>
public class PathfindingSystem : MonoBehaviour
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

    AStarTask _aStarTask;
    Vertex[] _graph;
    /// <summary>
    /// �o�H�e�[�u��
    /// </summary>
    Stack<Vector3>[,] _table;

    /// <summary>
    /// �o�H�e�[�u���̍쐬�������������̃t���O
    /// ���̃t���O��true�ɂȂ�����Ɍo�H�̎擾���\
    /// </summary>
    bool _pathTableCreated;

    void Awake()
    {
        // 1�n�܂�̒��_�ԍ��Ŏ擾�������̂� 0�Ԗڂ��_�~�[�ɂ��Ă���
        _graph = new Vertex[_vertexObjects.Length + 1];
        _aStarTask = new(_graph);
        _table = new Stack<Vector3>[_vertexObjects.Length + 1, _vertexObjects.Length + 1];
    }

    /// <summary>
    /// �O�����炱�̃��\�b�h���Ă�Ōo�H�e�[�u�����쐬����
    /// ���̃��\�b�h������������Ɍo�H�T�����\�ɂȂ�
    /// </summary>
    public IEnumerator CreatePathTableCoroutine()
    {
        CreateVertex();
        CreateGraph();
        yield return CreateTableCoroutine();
    }

    /// <summary>
    /// 1�n�܂�̒��_�ԍ���t�^���Ē��_�̍쐬
    /// �O���t���璸�_�ԍ��Ŏ擾�ł���悤�ɂ���
    /// </summary>
    void CreateVertex()
    {
        for(int i = 0; i < _vertexObjects.Length; i++)
        {
            Vertex vertex = _vertexObjects[i].gameObject.AddComponent<Vertex>();
            vertex.Number = i + 1;
            _graph[i + 1] = vertex;
        }
    }

    /// <summary>
    /// ���C�L���X�g��p���Ċe���_�̗אڂ��Ă��钸�_���擾���ǉ�����
    /// </summary>
    void CreateGraph()
    {
        for (int i = 0; i < _vertexObjects.Length; i++)
        {
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
                    if (hitInfo.collider.TryGetComponent(out Vertex neighbourVertex))
                    {
                        _graph[i + 1].AddNeighbour(neighbourVertex);
                    }
                }
            }
        }
    }

    /// <summary>
    /// �O�����炱�̃��\�b�h���ĂԂ��ƂŁA�w�肵��2�n�_�����ԋ߂����_���m�̌o�H�����܂�
    /// </summary>
    public Stack<Vector3> GetPath(Vector3 startPos, Vector3 goalPos)
    {
        if (!_pathTableCreated)
        {
            Debug.LogWarning("�o�H�e�[�u�����쐬: ���̏�Ɉړ�����o�H���쐬���ĕԂ�");
            Stack<Vector3> path = new();
            path.Push(startPos);
            return path;
        }

        // TOOD:���_���\�[�g���Ĉ�ԋ߂��_�����߂Ă���̂Ŗ���\�[�g��2������Ă���
        int startNumber = PosToNeighbourVertexNumber(startPos);
        int goalNumber = PosToNeighbourVertexNumber(goalPos);
        return _table[startNumber, goalNumber];
    }

    int PosToNeighbourVertexNumber(Vector3 pos)
    {
        Vertex neighbour = _graph.OrderBy(v => (v.transform.position - pos).sqrMagnitude).FirstOrDefault();
        return neighbour.Number;
    }

    /// <summary>
    /// �O���t��̑S�Ă̒��_�̌o�H�T�����s���A�o�H�e�[�u�����쐬����
    /// Editor��ŃX�e�[�W�ɔz�u�����I�u�W�F�N�g��ύX�����ۂɎ��s�����z��
    /// </summary>
    IEnumerator CreateTableCoroutine()
    {
#if UNITY_EDITOR
        Stopwatch stopwatch = new("�o�H�e�[�u���̍쐬");
        stopwatch.Start();
#endif

        for (int i = 1; i <= _vertexObjects.Length; i++)
        {
            for (int k = 1; k <= _vertexObjects.Length; k++)
            {
                _table[i, k] = _aStarTask.Pathfinding(i, k);
            }
            yield return null;
        }

        // �o�H�e�[�u���̍쐬�����������t���O�𗧂Ă�
        _pathTableCreated = true;

#if UNITY_EDITOR
        stopwatch.Stop();
#endif
    }
}