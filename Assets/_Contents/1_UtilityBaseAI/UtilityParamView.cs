using UnityEngine;

/// <summary>
/// �e�p�����[�^��UI�ɕ\������
/// �ǂݎ�肾���s���̂ō�������ΒP�̂œ��삷��
/// </summary>
[RequireComponent(typeof(UtilityBlackBoard))]
public class UtilityParamView : MonoBehaviour
{
    [Header("�p�����[�^��\������UI")]
    [SerializeField] Transform _foodBar;
    [SerializeField] Transform _energyBar;

    UtilityBlackBoard _blackBoard;

    void Start()
    {
        _blackBoard = GetComponent<UtilityBlackBoard>();
        _foodBar.localScale = Vector3.one;
        _energyBar.localScale = Vector3.one;
    }

    void Update()
    {
        View(_blackBoard.FoodParam.Value, _foodBar);
        View(_blackBoard.EnergyParam.Value, _energyBar);
    }

    void View(float current, Transform bar)
    {
        Vector3 scale = bar.localScale;
        scale.x = Mathf.Clamp01(current);
        bar.localScale = scale;
    }
}