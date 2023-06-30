using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ユーティリティベースで実行する食事をする状態のクラス
/// </summary>
public class UtilityStateEat : UtilityStateBase
{
    public UtilityStateEat() : base(UtilityStateType.Eat) { }

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
