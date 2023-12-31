using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using MilitaryHierarchy;
using System;

public class Tank : MonoBehaviour
{
    Rigidbody _rigidbody;
    TankOrder _order;

    /// <summary>
    /// 部隊の隊長に追従させる
    /// </summary>
    public Transform TankLeader { get; set; }
    /// <summary>
    /// 1から3の部隊番号
    /// 部隊ごとの命令を判別するのに用いる
    /// </summary>
    public int TroopNum { get; set; }

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        // 所属している部隊番号のみの命令のみ受け取る
        IObservable<TankOrderMessage> tankOrder = MessageBroker.Default
            .Receive<TankOrderMessage>().Where(msg => msg.TroopNum == TroopNum);

        // 追従の命令がされているときは移動処理
        this.FixedUpdateAsObservable().Where(_ => _order == TankOrder.Follow).Subscribe(_ =>
        {
            _rigidbody.velocity = TankLeader.forward * 3.0f;
            Debug.Log("とことこ");
        });
    }

    void Update()
    {
        switch (_order)
        {
            case TankOrder.Follow:

                break;
            case TankOrder.Fire:
                break;
        }
    }
}
