using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各状態のインスタンスを保持しているクラス
/// </summary>
[RequireComponent(typeof(UtilityBlackBoard))]
[DefaultExecutionOrder(-1)]
public class UtilityStateHolder : MonoBehaviour
{
    UtilityBlackBoard _blackBoard;

    void Awake()
    {
        _blackBoard = GetComponent<UtilityBlackBoard>();
    }

    /// <summary>
    /// 各状態のインスタンスを作成して初期状態を返す
    /// </summary>
    public UtilityStateBase CreateStateAll()
    {
        UtilitySateSleep stateSleep = new(_blackBoard);
        UtilityStateEat stateEat = new(_blackBoard);
        UtilityStateWork stateWork = new(_blackBoard);

        return stateSleep;
    }
}
