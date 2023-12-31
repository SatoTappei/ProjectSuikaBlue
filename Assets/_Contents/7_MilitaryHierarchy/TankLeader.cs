using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using MilitaryHierarchy;
using System;

public class TankLeader : MonoBehaviour
{
    [SerializeField] Transform _model;

    Rigidbody _rigidbody;
    Vector3? _destination;

    /// <summary>
    /// 1から3の部隊番号
    /// 部隊ごとの命令を判別するのに用いる
    /// </summary>
    public int TroopNum { get; set; }

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        // 所属している部隊番号のみの命令のみ受け取る
        IObservable<DestinationMessage> generalOrder = MessageBroker.Default
            .Receive<DestinationMessage>().Where(msg => msg.TroopNum == TroopNum);
        // 移動先を設定
        generalOrder.Subscribe(msg => _destination = msg.Destination);
        // 兵士に追従して移動するよう命令
        generalOrder.Subscribe(_ => 
        { 
            MessageBroker.Default.Publish(new TankOrderMessage()
            {
                Order = TankOrder.Follow,
            });
        }).AddTo(this);

        // 移動先がある場合は、一定距離までその地点に向けて移動
        this.FixedUpdateAsObservable()
            .Where(_ => _destination != null)
            .TakeWhile(_ => (transform.position - (Vector3)_destination).sqrMagnitude > 30.0f)
            .DoOnCompleted(() => 
            { 
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            })
            .Subscribe(_ =>
            {
                // 移動
                Vector3 dir = (Vector3)_destination - transform.position;
                dir.y = 0;
                dir.Normalize();
                Vector3 velo = dir * 3.0f;
                velo.y = _rigidbody.velocity.y;
                _rigidbody.velocity = velo;
                // 回転
                Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
                _model.rotation = Quaternion.Lerp(_model.rotation, rot, Time.deltaTime * 5.0f);
            }).AddTo(this);
    }
}