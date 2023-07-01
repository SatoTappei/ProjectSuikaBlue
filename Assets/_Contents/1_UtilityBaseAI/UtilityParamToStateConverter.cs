using UnityEngine;

/// <summary>
/// �]���l�����ɍs����Ԃւƕϊ�����N���X
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

        Debug.LogError("���̃��O���\�������̂͂�������");
        return UtilityStateType.Base;
    }
}
