using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���[�e�B���e�B�x�[�X�Ŏ��s����H���������Ԃ̃N���X
/// </summary>
public class UtilityStateEat : UtilityStateBase
{
    public UtilityStateEat(UtilityBlackBoard blackBoard) 
        : base(UtilityStateType.Eat, blackBoard) { }

    protected override void Enter()
    {
    }

    protected override void Exit()
    {
    }

    protected override void Stay()
    {
        //
    }
}