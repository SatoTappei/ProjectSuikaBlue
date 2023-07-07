using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(PathfindingSystem))]
public class PathTableManager : MonoBehaviour
{
    [Header("Path�̃M�Y���ւ̕`��")]
    [SerializeField] bool _drawGizmos = true;

    PathfindingSystem _pathfindingSystem;

    /// <summary>
    /// �M�Y���ɕ\�����邽�߂Ƀ����o�Ƃ��ĕێ����Ă���
    /// </summary>
    Stack<Vector3> _path;

    void Awake()
    {
        _pathfindingSystem = GetComponent<PathfindingSystem>();
    }

    IEnumerator Start()
    {
        yield return StartCoroutine(_pathfindingSystem.CreatePathTableCoroutine());
        // �o�H�e�[�u���̍쐬��������A���b�Z�[�W�𑗐M����
        MessageBroker.Default.Publish(new PathTableCreatedMessage());
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

        Gizmos.color = Color.red;
        foreach(Vector3 vertexPos in _path)
        {
            Gizmos.DrawSphere(vertexPos, 0.25f);
        }
    }
}