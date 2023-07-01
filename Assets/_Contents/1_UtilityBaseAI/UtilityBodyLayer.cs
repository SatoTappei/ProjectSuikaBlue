using UnityEngine;

/// <summary>
/// 身体の状態を保持しているクラス
/// </summary>
public class UtilityBodyLayer : MonoBehaviour
{
    public UtilityStateType Adjust(UtilityStateType type)
    {
        // TODO:フィルタリングの条件を書く、黒板の参照が必要なら持っても良し
        //      アニメーション中だったり、一定時間たっていなかったり等が考えられる
        return type;
    }
}
