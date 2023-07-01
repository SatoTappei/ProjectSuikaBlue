using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ユーティリティベースで決定された状態で使用するオブジェクトの列挙型
/// 状態とは紐づいていない
/// </summary>
public enum EnvironmentType
{
    WorkSpace,
    Chair,
    Bed,
}

/// <summary>
/// ユーティリティベースで使う黒板
/// </summary>
public class UtilityBlackBoard : MonoBehaviour
{
    /// <summary>
    /// インスペクターから割り当てる用
    /// </summary>
    class Environment
    {
        [SerializeField] EnvironmentType _type;
        [SerializeField] GameObject _object;

        public EnvironmentType Type => _type;
        public GameObject Object => _object;
    }

    [Header("各状態で使用する環境")]
    [SerializeField] Environment[] _environments;
    [Header("食欲")]
    [SerializeField] UtilityParam _food;
    [Header("やる気")]
    [SerializeField] UtilityParam _energy;

    Dictionary<EnvironmentType, GameObject> _environmentDict;
    UtilityStateType _selectedStateType;

    public UtilityParam FoodParam => _food;
    public UtilityParam EnergyParam => _energy;
    public GameObject this[EnvironmentType key]
    {
        get
        {
            if(_environmentDict.TryGetValue(key, out GameObject value))
            {
                return value;
            }
            else
            {
                Debug.LogError("対応する環境が無い: " + key);
                return null;
            }
        }
    }

    /// <summary>
    /// Controllerから書き込まれて、Stateが読み取る
    /// </summary>
    public UtilityStateType SelectedStateType { get; set; }

    void Awake()
    {
        _environmentDict = _environments.ToDictionary(e => e.Type, e => e.Object);
    }
}