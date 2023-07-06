using PathTableGraph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �o�H�e�[�u����p�����o�H�T�����s���N���X
/// ���O�ɃV�[����ɒ��_�ƂȂ�I�u�W�F�N�g��z�u���Ă���K�v������
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

    AStarTask _aStarTask;
    Vertex[] _graph;
    /// <summary>
    /// �M�Y���ɕ\�����邽�߂Ƀ����o�Ƃ��ĕێ����Ă���
    /// </summary>
    Stack<Vector3> _path;
    /// <summary>
    /// �o�H�e�[�u��
    /// </summary>
    Stack<Vector3>[,] _table;

    void Awake()
    {
        // 1�n�܂�̒��_�ԍ��Ŏ擾�������̂� 0�Ԗڂ��_�~�[�ɂ��Ă���
        _graph = new Vertex[_vertexObjects.Length + 1];
        _aStarTask = new AStarTask(_graph);

        CreateVertex();
        CreateGraph();
        WritePathToTextAsset();
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
    /// �o�H�T�����s���A�e�L�X�g�t�@�C���ɏ�������
    /// </summary>
    public void WritePathToTextAsset()
    {
        //TextAssetGenerator generator = new();


        int i = Random.Range(1, _vertexObjects.Length + 1);
        int k = Random.Range(1, _vertexObjects.Length + 1);

#if UNITY_EDITOR
        Stopwatch stopwatch = new(i, k);
        stopwatch.Start();
#endif
        _path = _aStarTask.Pathfinding(i, k);
#if UNITY_EDITOR
        stopwatch.Stop();
#endif
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying && _drawGizmos)
        {
            DrawPathOnGizmos();
        }
    }

    void DrawPathOnGizmos()
    {
        if (_path == null || _path.Count == 0) return;

        foreach (Vector3 pos in _path)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pos, 0.25f);
        }
    }
}