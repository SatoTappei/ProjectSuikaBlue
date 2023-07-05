using UnityEngine;

/// <summary>
/// ����T�C�N���𐧌䂷��N���X
/// </summary>
public class DayNightCycleController : MonoBehaviour
{
    /// <summary>
    /// ��̈�ԈÂ��p�x
    /// </summary>
    const float NightAngle = -12.0f;
    /// <summary>
    /// ���̈�Ԗ��邢�p�x
    /// </summary>
    const float DayAngle = 50.0f;

    [SerializeField] Light _light;
    [Header("����T�C�N���̐ݒ�")]
    [SerializeField] string _speedParamName = "Speed";
    [SerializeField] float _cycleSpeed = 0.1f;

    Animator _lightAnimController;

    void Awake()
    {
        _lightAnimController = _light.GetComponent<Animator>();
        _lightAnimController.SetFloat(_speedParamName, _cycleSpeed);
    }
}
