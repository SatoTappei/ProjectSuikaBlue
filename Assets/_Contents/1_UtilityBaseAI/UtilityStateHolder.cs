using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �e��Ԃ̃C���X�^���X��ێ����Ă���N���X
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
    /// �e��Ԃ̃C���X�^���X���쐬���ď�����Ԃ�Ԃ�
    /// </summary>
    public UtilityStateBase CreateStateAll()
    {
        UtilitySateSleep stateSleep = new(_blackBoard);
        UtilityStateEat stateEat = new(_blackBoard);
        UtilityStateWork stateWork = new(_blackBoard);

        return stateSleep;
    }
}
