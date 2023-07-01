using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ユーティリティベースで実行する食事をする状態のクラス
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
