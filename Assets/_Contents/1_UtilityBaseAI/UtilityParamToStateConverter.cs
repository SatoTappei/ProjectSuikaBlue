using UnityEngine;

/// <summary>
/// 評価値を次に行う状態へと変換するクラス
/// </summary>
public class UtilityParamToStateConverter
{
    public UtilityStateType ConvertToState(UtilityParamType param)
    {
        switch (param)
        {
            case UtilityParamType.Food: return UtilityStateType.Eat;
            case UtilityParamType.Energy: return UtilityStateType.Sleep;
            case UtilityParamType.Fun: return UtilityStateType.Work;
        }

        Debug.LogError("このログが表示されるのはおかしい");
        return UtilityStateType.Base;
    }
}
