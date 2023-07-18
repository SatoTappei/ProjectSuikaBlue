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
    /// •”‘à‚Ì‘à’·‚É’Ç]‚³‚¹‚é
    /// </summary>
    public Transform TankLeader { get; set; }
    /// <summary>
    /// 1‚©‚ç3‚Ì•”‘à”Ô†
    /// •”‘à‚²‚Æ‚Ì–½—ß‚ğ”»•Ê‚·‚é‚Ì‚É—p‚¢‚é
    /// </summary>
    public int TroopNum { get; set; }

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        // Š‘®‚µ‚Ä‚¢‚é•”‘à”Ô†‚Ì‚İ‚Ì–½—ß‚Ì‚İó‚¯æ‚é
        IObservable<TankOrderMessage> tankOrder = MessageBroker.Default
            .Receive<TankOrderMessage>().Where(msg => msg.TroopNum == TroopNum);

        // ’Ç]‚Ì–½—ß‚ª‚³‚ê‚Ä‚¢‚é‚Æ‚«‚ÍˆÚ“®ˆ—
        this.FixedUpdateAsObservable().Where(_ => _order == TankOrder.Follow).Subscribe(_ =>
        {
            _rigidbody.velocity = TankLeader.forward * 3.0f;
            Debug.Log("‚Æ‚±‚Æ‚±");
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
