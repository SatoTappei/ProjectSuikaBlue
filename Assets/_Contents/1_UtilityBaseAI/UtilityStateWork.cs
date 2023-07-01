using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ユーティリティベースで実行する仕事をする状態のクラス
/// </summary>
public class UtilityStateWork : UtilityStateBase
{
    public UtilityStateWork(UtilityBlackBoard blackBoard) 
        : base(UtilityStateType.Work, blackBoard) { }

    protected override void Enter()
    {
    }

    protected override void Exit()
    {
    }

    protected override void Stay()
    {
    }
}
