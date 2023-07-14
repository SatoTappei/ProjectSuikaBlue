using UnityEngine;
using UniRx;
using System;

/// <summary>
/// 視界の機能、黒板に書き込むクラス
/// </summary>
[RequireComponent(typeof(CommonLayerBlackBoard))]
public class SightSensor : MonoBehaviour
{
    /// <summary>
    /// 視界を更新する間隔
    /// </summary>
    const float UpdateInterval = 0.05f;
    /// <summary>
    /// 視界で一度に発見できる数
    /// </summary>
    const int Capacity = 9;

    [Header("プレイヤーに関する値")]
    [SerializeField] string _playerTag;
    [SerializeField] LayerMask _playerLayer;
    [Header("障害物のレイヤー")]
    [SerializeField] LayerMask _obstacleLayer;
    [Header("視界の半径")]
    [SerializeField] float _radius = 10.0f;
    [Header("ギズモ上に表示するか")]
    [SerializeField] bool _isDrawGizmos = true;

    Transform _transform;

    void Awake()
    {
        _transform = transform;
        CommonLayerBlackBoard commonLayerBlackBoard = GetComponent<CommonLayerBlackBoard>();

        // 一定間隔で球形の判定でプレイヤーを検知する
        Collider[] results = new Collider[Capacity];
        Observable.Interval(System.TimeSpan.FromSeconds(UpdateInterval)).Subscribe(_ => 
        {
            Array.Clear(results, 0, results.Length);
            Physics.OverlapSphereNonAlloc(_transform.position, _radius, results, _playerLayer);
        });
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying && _isDrawGizmos)
        {
            Gizmos.DrawWireSphere(_transform.position, _radius);
        }
    }
}

// 視界にとらえているものをGameObjectとして保存する
// プレイヤーは別途保持する。辞書型やリストを使用して引き出せるようにする？
