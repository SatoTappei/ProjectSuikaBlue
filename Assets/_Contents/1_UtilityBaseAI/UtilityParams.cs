using System.Linq;
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
    Energy,
}

/// <summary>
/// 評価に用いる各値を制御するクラス
/// </summary>
[System.Serializable]
public class UtilityParams
{
    /// <summary>
    /// 評価に用いる値のクラス
    /// </summary>
    [System.Serializable]
    public class Param
    {
        [SerializeField] AnimationCurve _curve;
        [SerializeField] UtilityParamType _type;
        [Header("自然減少時の倍率")]
        [Range(0, 1.0f)]
        [SerializeField] float _decreaseMag = 0.01f;

        /// <summary>
        /// 0~1の間を取るので初期値は1で固定
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

    [Header("デバッグ用にUIへ表示させる")]
    [SerializeField] UtilityParamView _paramView;
    [Header("食欲")]
    [SerializeField] Param _food;
    [Header("やる気")]
    [SerializeField] Param _energy;

    public void Update()
    {
        _paramView.SetFoodValue(_food.Decrease());
        _paramView.SetEnergyValue(_energy.Decrease());
    }

    /// <summary>
    /// 各値を評価して一番高い数値のものを返す
    /// </summary>
    public UtilityParamType SelectNext()
    {
        Param[] temp =
        {
            _food,
            _energy,
        };

        return temp.OrderByDescending(param => param.Evaluate()).FirstOrDefault().Type;
    }
}