using UnityEngine;

/// <summary>
/// ユーティリティベースで実行する睡眠をする状態のクラス
/// </summary>
public class UtilitySateSleep : UtilityStateBase
{
    /// <summary>
    /// 疲労が減少する間隔
    /// </summary>
    const float Interval = 1.0f;
    /// <summary>
    /// 一回の疲労の減少量
    /// </summary>
    const float HealingValue = 0.05f;

    float _timer;

    public UtilitySateSleep(UtilityBlackBoard blackBoard) 
        : base(UtilityStateType.Sleep, blackBoard) { }

    protected override void Enter()
    {
        _timer = 0;
    }

    protected override void Exit()
    {
    }

    protected override void Stay()
    {
        Vector3 toBed = BlackBoard[EnvironmentType.Bed].transform.position - BlackBoard.Transform.position;
        float distance = Vector3.SqrMagnitude(toBed);
        if(distance < .1f)
        {
            // 徐々に疲労が減少する
            _timer += Time.deltaTime;
            if(_timer > Interval)
            {
                _timer = 0;
                BlackBoard.TiredParam.Value -= HealingValue;

                // 遷移
                TransitionIfStateChanged();
            }
        }
        else
        {
            // ベッドに向けて移動
            Vector3 velo = toBed.normalized * Time.deltaTime * BlackBoard.MoveSpeed;
            BlackBoard.Transform.position += velo;
        }

        // 食べたい欲が増える
        BlackBoard.FoodParam.Increase();
    }
}