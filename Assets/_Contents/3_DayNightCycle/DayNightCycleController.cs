using UnityEngine;

/// <summary>
/// 昼夜サイクルを制御するクラス
/// </summary>
public class DayNightCycleController : MonoBehaviour
{
    /// <summary>
    /// 夜の一番暗い角度
    /// </summary>
    const float NightAngle = -12.0f;
    /// <summary>
    /// 昼の一番明るい角度
    /// </summary>
    const float DayAngle = 50.0f;

    [SerializeField] Light _light;
    [Header("昼夜サイクルの設定")]
    [SerializeField] string _speedParamName = "Speed";
    [SerializeField] float _cycleSpeed = 0.1f;

    Animator _lightAnimController;

    void Awake()
    {
        _lightAnimController = _light.GetComponent<Animator>();
        _lightAnimController.SetFloat(_speedParamName, _cycleSpeed);
    }
}
