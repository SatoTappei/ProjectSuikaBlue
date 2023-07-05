using UnityEngine;

/// <summary>
/// 各パラメータをUIに表示する
/// 読み取りだけ行うので黒板があれば単体で動作する
/// </summary>
[RequireComponent(typeof(UtilityBlackBoard))]
public class UtilityParamView : MonoBehaviour
{
    [Header("パラメータを表示するUI")]
    [SerializeField] Transform _foodBar;
    [SerializeField] Transform _funBar;
    [SerializeField] Transform _tired;

    UtilityBlackBoard _blackBoard;

    void Start()
    {
        _blackBoard = GetComponent<UtilityBlackBoard>();
        _foodBar.localScale = Vector3.one;
        _funBar.localScale = Vector3.one;
        _tired.localScale = Vector3.one;
    }

    void Update()
    {
        View(_blackBoard.FoodParam.Value, _foodBar);
        View(_blackBoard.FunParam.Value, _funBar);
        View(_blackBoard.TiredParam.Value, _tired);
    }

    void View(float current, Transform bar)
    {
        Vector3 scale = bar.localScale;
        scale.x = Mathf.Clamp01(current);
        bar.localScale = scale;
    }
}