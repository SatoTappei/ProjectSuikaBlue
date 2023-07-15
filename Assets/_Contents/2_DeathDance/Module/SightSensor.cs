using UnityEngine;
using UniRx;
using System;

/// <summary>
/// 全ての敵に共通して使用する、視界の機能
/// 黒板に書き込むクラス
/// </summary>
[RequireComponent(typeof(CommonLayerBlackBoard))]
public class SightSensor : MonoBehaviour
{
    /// <summary>
    /// 視界を更新する間隔
    /// </summary>
    const float UpdateInterval = 0.1f;
    /// <summary>
    /// 視界で一度に発見できる数
    /// </summary>
    const int Capacity = 9;

    [Header("プレイヤーに関する値")]
    [SerializeField] string _playerTag = "Player";
    [SerializeField] LayerMask _playerLayer;
    [Header("障害物のレイヤー")]
    [SerializeField] LayerMask _obstacleLayer;
    [Header("視界の半径")]
    [SerializeField] float _radius = 10.0f;
    [Header("ギズモ上に表示するか")]
    [SerializeField] bool _isDrawGizmos = true;

    CommonLayerBlackBoard _commonLayerBlackBoard;
    Transform _transform;
    Collider[] _results = new Collider[Capacity];

    void Awake()
    {
        _transform = transform;
        _commonLayerBlackBoard = GetComponent<CommonLayerBlackBoard>();

        // 一定間隔で球形の判定でプレイヤーを検知する
        Observable.Interval(System.TimeSpan.FromSeconds(UpdateInterval)).Subscribe(_ => DetectPlayer()).AddTo(this);
    }

    /// <summary>
    /// このメソッドを一定間隔で呼ぶことでプレイヤーを検知する
    /// 球形の判定でプレイヤーを検知したら黒板の視界に捉えたものリストを更新する
    /// </summary>
    void DetectPlayer()
    {
        // 更新するために結果と黒板の保持するリストを削除
        Array.Clear(_results, 0, _results.Length);
        _commonLayerBlackBoard.VisibleTargetDict[VisibleTargetType.Player].Clear();

        // 検知
        int count = Physics.OverlapSphereNonAlloc(_transform.position, _radius, _results, _playerLayer);
        if (count == 0) return;

        foreach (Collider result in _results)
        {
            if (result == null) continue;

            // 黒板の辞書に新しくデータを更新して追加
            _commonLayerBlackBoard.VisibleTargetDict[VisibleTargetType.Player].Clear();
            VisibleTargetData data = new VisibleTargetData(VisibleTargetType.Player, result.transform.position, Time.time);
            _commonLayerBlackBoard.VisibleTargetDict[VisibleTargetType.Player].Add(data);
        }
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying && _isDrawGizmos)
        {
            // 視界の範囲
            Gizmos.DrawWireSphere(_transform.position, _radius);
        }
    }
}
