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
    /// �����̑����ɒǏ]������
    /// </summary>
    public Transform TankLeader { get; set; }
    /// <summary>
    /// 1����3�̕����ԍ�
    /// �������Ƃ̖��߂𔻕ʂ���̂ɗp����
    /// </summary>
    public int TroopNum { get; set; }

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        // �������Ă��镔���ԍ��݂̖̂��߂̂ݎ󂯎��
        IObservable<TankOrderMessage> tankOrder = MessageBroker.Default
            .Receive<TankOrderMessage>().Where(msg => msg.TroopNum == TroopNum);

        // �Ǐ]�̖��߂�����Ă���Ƃ��͈ړ�����
        this.FixedUpdateAsObservable().Where(_ => _order == TankOrder.Follow).Subscribe(_ =>
        {
            _rigidbody.velocity = TankLeader.forward * 3.0f;
            Debug.Log("�Ƃ��Ƃ�");
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
