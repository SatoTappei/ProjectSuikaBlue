using System.Collections.Generic;
using UnityEngine;
using FSM;

/// <summary>
/// ボスと敵が共通で持つパラメータを読み書きする黒板
/// Init/PlayerDetect/PlayerUndetect/Defeated の4つの状態が主に参照する
/// </summary>
[DefaultExecutionOrder(-1)]
public class CommonLayerBlackBoard : MonoBehaviour
{
    [Header("最大体力")]
    [SerializeField] int _maxHp = 10;
    [Header("球状の視界の半径")]
    [SerializeField] float _sightRadius = 10;

    Dictionary<EnemyStateType, EnemyStateBase> _stateDict;
    int _currentHp;
    bool _isDetectedPlayer;
    
    /// <summary>
    /// 各状態が遷移条件を満たした場合に遷移先を取得する
    /// </summary>
    public EnemyStateBase this [EnemyStateType key]
    {
        get
        {
            if (_stateDict.TryGetValue(key, out EnemyStateBase value))
            {
                return value;
            }
            else
            {
                throw new KeyNotFoundException("対応する状態が無い: " + key);
            }
        }
    }
    public float SightRadius => _sightRadius;
    public int CurrentHp { get => _currentHp; set => _currentHp = value; }
    public bool IsDetectedPlayer { get => _isDetectedPlayer; set => _isDetectedPlayer = value; }

    void Awake()
    {
        _currentHp = _maxHp;
        CreateState();
    }

    void CreateState()
    {
        _stateDict = new(4);
        _stateDict.Add(EnemyStateType.Init, new InitState(EnemyStateType.Init, this));
        _stateDict.Add(EnemyStateType.PlayerDetected, new PlayerDetectState(EnemyStateType.PlayerDetected, this));
        _stateDict.Add(EnemyStateType.PlayerUndetected, new PlayerUndetectState(EnemyStateType.PlayerUndetected, this));
        _stateDict.Add(EnemyStateType.Defeated, new DefeatedState(EnemyStateType.Init, this));
    }
}
