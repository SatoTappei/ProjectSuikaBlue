using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// 自作便利クラス
using CommonUtility;

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
[DefaultExecutionOrder(-1)]
public class UtilityBlackBoard : MonoBehaviour
{
    /// <summary>
    /// インスペクターから割り当てる用
    /// </summary>
    [System.Serializable]
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
    [Header("楽しさ")]
    [SerializeField] UtilityParam _fun;
    [Header("疲労")]
    [SerializeField] UtilityParam _tired;
    [Header("移動速度")]
    [SerializeField] float _moveSpeed = 3.0f;

    Transform _transform;
    Dictionary<EnvironmentType, GameObject> _environmentDict;
    Dictionary<UtilityStateType, UtilityStateBase> _stateDict;
    UtilityStateType _selectedStateType;

    public Transform Transform => _transform;
    public UtilityParam FoodParam => _food;
    public UtilityParam FunParam => _fun;
    public UtilityParam TiredParam => _tired;
    public float MoveSpeed => _moveSpeed;
    /// <summary>
    /// キャラクターが干渉する環境の辞書
    /// 各状態が参照する
    /// </summary>
    public GameObject this[EnvironmentType key]
    {
        get => DictUtility.TryGetValue(_environmentDict, key);
    }
    /// <summary>
    /// キャラクターが遷移可能な状態の辞書
    /// 各状態が遷移可能
    /// </summary>
    public UtilityStateBase this[UtilityStateType key]
    {
        get => DictUtility.TryGetValue(_stateDict, key);
    }
    /// <summary>
    /// 一定間隔で評価値を用いて選択された状態がControllerから書き込まれて
    /// 実行中のStateが読み取る
    /// 実行中の状態と違う状態ならばこの状態に遷移する
    /// </summary>
    public UtilityStateType SelectedStateType 
    { 
        get => _selectedStateType; 
        set => _selectedStateType = value; 
    }

    void Awake()
    {
        CreateStateAll();
        _environmentDict = _environments.ToDictionary(e => e.Type, e => e.Object);
        _transform = transform;
    }

    void CreateStateAll()
    {
        UtilitySateSleep stateSleep = new(this);
        UtilityStateEat stateEat = new(this);
        UtilityStateWork stateWork = new(this);
        _stateDict = new(3);
        _stateDict.Add(stateSleep.Type, stateSleep);
        _stateDict.Add(stateEat.Type, stateEat);
        _stateDict.Add(stateWork.Type, stateWork);
    }
}