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
    /// 1����3�̕����ԍ�
    /// �������Ƃ̖��߂𔻕ʂ���̂ɗp����
    /// </summary>
    public int TroopNum { get; set; }

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        // �������Ă��镔���ԍ��݂̖̂��߂̂ݎ󂯎��
        IObservable<DestinationMessage> generalOrder = MessageBroker.Default
            .Receive<DestinationMessage>().Where(msg => msg.TroopNum == TroopNum);
        // �ړ����ݒ�
        generalOrder.Subscribe(msg => _destination = msg.Destination);
        // ���m�ɒǏ]���Ĉړ�����悤����
        generalOrder.Subscribe(_ => 
        { 
            MessageBroker.Default.Publish(new TankOrderMessage()
            {
                Order = TankOrder.Follow,
            });
        }).AddTo(this);

        // �ړ��悪����ꍇ�́A��苗���܂ł��̒n�_�Ɍ����Ĉړ�
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
                // �ړ�
                Vector3 dir = (Vector3)_destination - transform.position;
                dir.y = 0;
                dir.Normalize();
                Vector3 velo = dir * 3.0f;
                velo.y = _rigidbody.velocity.y;
                _rigidbody.velocity = velo;
                // ��]
                Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
                _model.rotation = Quaternion.Lerp(_model.rotation, rot, Time.deltaTime * 5.0f);
            }).AddTo(this);
    }
}