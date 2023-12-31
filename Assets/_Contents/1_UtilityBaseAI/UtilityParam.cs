using UnityEngine;

/// <summary>
/// ユーティリティベースの各評価を表す列挙型
/// 評価値が増えた場合は対応する値を追加する必要がある
/// </summary>
public enum UtilityParamType
{
    Base,
    Food,
    Fun,
    Tired,
}

/// <summary>
/// 評価に用いる値のクラス
/// </summary>
[System.Serializable]
public class UtilityParam
{
    [SerializeField] AnimationCurve _curve;
    [SerializeField] UtilityParamType _type;
    [Header("自然減少時の倍率")]
    [Range(0, 1.0f)]
    [SerializeField] float _decreaseMag = 0.01f;

    /// <summary>
    /// 0~1の間を取る必要がある
    /// </summary>
    float _value = 0.5f;

    public UtilityParamType Type => _type;
    public float Value { get => _value; set => _value = Mathf.Clamp01(value); }

    public float Decrease()
    {
        _value -= Time.deltaTime * _decreaseMag;
        Mathf.Clamp01(_value);

        return _value;
    }

    public float Increase()
    {
        _value += Time.deltaTime * _decreaseMag;
        Mathf.Clamp01(_value);

        return _value;
    }

    public float Evaluate() => _curve.Evaluate(_value);
}