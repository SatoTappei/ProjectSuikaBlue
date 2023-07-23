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
    /// 1‚©‚ç3‚Ì•”‘à”Ô†
    /// •”‘à‚²‚Æ‚Ì–½—ß‚ğ”»•Ê‚·‚é‚Ì‚É—p‚¢‚é
    /// </summary>
    public int TroopNum { get; set; }

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        // Š‘®‚µ‚Ä‚¢‚é•”‘à”Ô†‚Ì‚İ‚Ì–½—ß‚Ì‚İó‚¯æ‚é
        IObservable<DestinationMessage> generalOrder = MessageBroker.Default
            .Receive<DestinationMessage>().Where(msg => msg.TroopNum == TroopNum);
        // ˆÚ“®æ‚ğİ’è
        generalOrder.Subscribe(msg => _destination = msg.Destination);
        // •ºm‚É’Ç]‚µ‚ÄˆÚ“®‚·‚é‚æ‚¤–½—ß
        generalOrder.Subscribe(_ => 
        { 
            MessageBroker.Default.Publish(new TankOrderMessage()
            {
                Order = TankOrder.Follow,
            });
        }).AddTo(this);

        // ˆÚ“®æ‚ª‚ ‚éê‡‚ÍAˆê’è‹——£‚Ü‚Å‚»‚Ì’n“_‚ÉŒü‚¯‚ÄˆÚ“®
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
                // ˆÚ“®
                Vector3 dir = (Vector3)_destination - transform.position;
                dir.y = 0;
                dir.Normalize();
                Vector3 velo = dir * 3.0f;
                velo.y = _rigidbody.velocity.y;
                _rigidbody.velocity = velo;
                // ‰ñ“]
                Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
                _model.rotation = Quaternion.Lerp(_model.rotation, rot, Time.deltaTime * 5.0f);
            }).AddTo(this);
    }
}