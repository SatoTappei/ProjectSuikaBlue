using System.Linq;
using UnityEngine;

/// <summary>
/// 各パラメータを評価して次にする行動を決定するクラス
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
    /// 各値を評価して一番高い数値のものを返す
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
