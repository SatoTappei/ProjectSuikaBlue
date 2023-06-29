using System.Linq;
using UnityEngine;

/// <summary>
/// ���[�e�B���e�B�x�[�X�̊e�]����\���񋓌^
/// �]���l���������ꍇ�͑Ή�����l��ǉ�����K�v������
/// </summary>
public enum UtilityParamType
{
    Base,
    Work,
    Eat,
    Sleep,
}

/// <summary>
/// �]���ɗp����l�̃N���X
/// </summary>
[System.Serializable]
public class UtilityParam
{
    [SerializeField] AnimationCurve _curve;
    [SerializeField] UtilityParamType _type;
    [Header("���R�������̔{��")]
    [Range(0, 1.0f)]
    [SerializeField] float _decreaseMag = 0.01f;

    /// <summary>
    /// 0~1�̊Ԃ����̂ŏ����l��1�ŌŒ�
    /// </summary>
    float _value = 1.0f;

    public float Value { get => _value; set => _value = value; }
    public UtilityParamType Type => _type;

    public float Decrease()
    {
        _value -= Time.deltaTime * _decreaseMag;
        Mathf.Clamp01(_value);

        return _value;
    }

    public float Evaluate() => _curve.Evaluate(_value);
}

/// <summary>
/// �]���ɗp����e�l�𐧌䂷��N���X
/// </summary>
[System.Serializable]
public class UtilityParamControlModule
{
    [Header("�f�o�b�O�p��UI�֕\��������")]
    [SerializeField] UtilityParamView _paramView;
    [Header("�H�~")]
    [SerializeField] UtilityParam _food;
    [Header("���C")]
    [SerializeField] UtilityParam _energy;

    public void Update()
    {
        _paramView.SetFoodValue(_food.Decrease());
        _paramView.SetEnergyValue(_energy.Decrease());
    }

    /// <summary>
    /// �e�l��]�����Ĉ�ԍ������l�̂��̂�Ԃ�
    /// </summary>
    public UtilityParamType SelectNext()
    {
        UtilityParam[] temp =
        {
            _food,
            _energy,
        };

        return temp.OrderByDescending(param => param.Evaluate()).FirstOrDefault().Type;
    }
}