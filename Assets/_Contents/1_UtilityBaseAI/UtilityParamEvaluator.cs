using System.Linq;
using UnityEngine;

/// <summary>
/// �e�p�����[�^��]�����Ď��ɂ���s�������肷��N���X
/// </summary>
[RequireComponent(typeof(UtilityBlackBoard))]
[DefaultExecutionOrder(-1)]
public class UtilityParamEvaluator : MonoBehaviour
{
    UtilityBlackBoard _blackBoard;

    void Awake()
    {
        _blackBoard = GetComponent<UtilityBlackBoard>();
    }

    /// <summary>
    /// �e�l��]�����Ĉ�ԍ������l�̂��̂�Ԃ�
    /// </summary>
    public UtilityParamType SelectHighestParamType()
    {
        UtilityParam[] temp =
        {
            _blackBoard.FoodParam,
            _blackBoard.FunParam,
            _blackBoard.TiredParam,
        };

        return temp.OrderByDescending(param => param.Evaluate()).FirstOrDefault().Type;
    }
}
