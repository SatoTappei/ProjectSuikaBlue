using UnityEngine;

public class UtilityParamView : MonoBehaviour
{
    [SerializeField] Transform _foodBar;
    [SerializeField] Transform _funBar;
    [SerializeField] Transform _energyBar;

    void Start()
    {
        _foodBar.localScale = Vector3.one;
        //_funBar.localScale = Vector3.one;
        _energyBar.localScale = Vector3.one;
    }

    public void SetFoodValue(float current) => SetBar(current, _foodBar);
    //public void SetFunValue(float current, float max) => SetBar(current, max, _funBar);
    public void SetEnergyValue(float current) => SetBar(current, _energyBar);

    void SetBar(float current, Transform bar)
    {
        Vector3 scale = bar.localScale;
        scale.x = Mathf.Clamp01(current);
        bar.localScale = scale;
    }
}