using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���[�e�B���e�B�x�[�X�Ŏ��s���鐇���������Ԃ̃N���X
/// </summary>
public class UtilitySateSleep : UtilityStateBase
{
    public UtilitySateSleep(UtilityBlackBoard blackBoard) 
        : base(UtilityStateType.Sleep, blackBoard) { }

    protected override void Enter()
    {
    }

    protected override void Exit()
    {
        // ���X�ɃG�l���M�[���񕜂��Ă���
    }

    protected override void Stay()
    {

    }
}
