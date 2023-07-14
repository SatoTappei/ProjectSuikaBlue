using System.Collections.Generic;
using UnityEngine;
using FSM;

/// <summary>
/// ‹ŠE‚É‘¨‚¦‚½‘ÎÛ‚ğ•”Â‚Å«‘Œ^‚Å•Û‚·‚é‚½‚ß‚ÌKey‚Æ‚È‚é—ñ‹“Œ^
/// </summary>
public enum VisibleTargetType
{
    Player,
}

/// <summary>
/// ‹ŠE‚É‘¨‚¦‚½‘ÎÛ‚ğ•”Â‚Å«‘Œ^‚Å•Û‚·‚é‚½‚ß‚Ìƒf[ƒ^
/// </summary>
public class VisibleTargetData
{
    VisibleTargetType _type;
    Vector3 _pos;
    float _time;
    // TODO:‹ºˆĞ“x/—Dæ“x ‚ª‚ ‚é‚Æ—Ç‚¢H

    public VisibleTargetData(VisibleTargetType type, in Vector3 pos, float time)
    {
        _type = type;
        _pos = pos;
        _time = time;
    }

    //public Type
}

/// <summary>
/// ƒ{ƒX‚Æ“G‚ª‹¤’Ê‚Å‚Âƒpƒ‰ƒ[ƒ^‚ğ“Ç‚İ‘‚«‚·‚é•”Â
/// Init/PlayerDetect/PlayerUndetect/Defeated ‚Ì4‚Â‚Ìó‘Ô‚ªå‚ÉQÆ‚·‚é
/// </summary>
[DefaultExecutionOrder(-1)]
public class CommonLayerBlackBoard : MonoBehaviour
{
    [Header("Å‘å‘Ì—Í")]
    [SerializeField] int _maxHp = 10;
    [Header("‹…ó‚Ì‹ŠE‚Ì”¼Œa")]
    [SerializeField] float _sightRadius = 10;

    Dictionary<EnemyStateType, EnemyStateBase> _stateDict;
    int _currentHp;
    bool _isDetectedPlayer;
    
    /// <summary>
    /// Šeó‘Ô‚ª‘JˆÚğŒ‚ğ–‚½‚µ‚½ê‡‚É‘JˆÚæ‚ğæ“¾‚·‚é
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
                throw new KeyNotFoundException("‘Î‰‚·‚éó‘Ô‚ª–³‚¢: " + key);
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
