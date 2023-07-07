using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(PathfindingSystem))]
public class PathTableManager : MonoBehaviour
{
    [Header("Pathのギズモへの描画")]
    [SerializeField] bool _drawGizmos = true;

    PathfindingSystem _pathfindingSystem;

    /// <summary>
    /// ギズモに表示するためにメンバとして保持しておく
    /// </summary>
    Stack<Vector3> _path;

    void Awake()
    {
        _pathfindingSystem = GetComponent<PathfindingSystem>();
    }

    IEnumerator Start()
    {
        yield return StartCoroutine(_pathfindingSystem.CreatePathTableCoroutine());
        // 経路テーブルの作成が完了後、メッセージを送信する
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