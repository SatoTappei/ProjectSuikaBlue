using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ユーティリティベースで実行する睡眠をする状態のクラス
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
        // 徐々にエネルギーが回復していく
    }

    protected override void Stay()
    {

    }
}
