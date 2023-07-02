using UnityEngine;

/// <summary>
/// ユーティリティベースで実行する睡眠をする状態のクラス
/// </summary>
public class UtilitySateSleep : UtilityStateBase
{
    /// <summary>
    /// 回復する間隔
    /// </summary>
    const float Interval = 1.0f;
    /// <summary>
    /// 一回の回復量
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
            // 徐々にエネルギーを回復する
            _timer += Time.deltaTime;
            if(_timer > Interval)
            {
                _timer = 0;
                BlackBoard.EnergyParam.Value += HealingValue;
            }
        }
        else
        {
            // 各種パラメータを自然減少
            BlackBoard.EnergyParam.Decrease();
            BlackBoard.FoodParam.Decrease();

            // ベッドに向けて移動
            Vector3 velo = toBed.normalized * Time.deltaTime * BlackBoard.MoveSpeed;
            BlackBoard.Transform.position += velo;
        }
    }
}
