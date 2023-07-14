using System.Collections.Generic;
using UnityEngine;
using FSM;

/// <summary>
/// ���E�ɑ������Ώۂ����Ŏ����^�ŕێ����邽�߂�Key�ƂȂ�񋓌^
/// </summary>
public enum VisibleTargetType
{
    Player,
}

/// <summary>
/// ���E�ɑ������Ώۂ����Ŏ����^�ŕێ����邽�߂̃f�[�^
/// </summary>
public class VisibleTargetData
{
    VisibleTargetType _type;
    Vector3 _pos;
    float _time;
    // TODO:���Гx/�D��x ������Ɨǂ��H

    public VisibleTargetData(VisibleTargetType type, in Vector3 pos, float time)
    {
        _type = type;
        _pos = pos;
        _time = time;
    }

    //public Type
}

/// <summary>
/// �{�X�ƓG�����ʂŎ��p�����[�^��ǂݏ������鍕��
/// Init/PlayerDetect/PlayerUndetect/Defeated ��4�̏�Ԃ���ɎQ�Ƃ���
/// </summary>
[DefaultExecutionOrder(-1)]
public class CommonLayerBlackBoard : MonoBehaviour
{
    [Header("�ő�̗�")]
    [SerializeField] int _maxHp = 10;
    [Header("����̎��E�̔��a")]
    [SerializeField] float _sightRadius = 10;

    Dictionary<EnemyStateType, EnemyStateBase> _stateDict;
    int _currentHp;
    bool _isDetectedPlayer;
    
    /// <summary>
    /// �e��Ԃ��J�ڏ����𖞂������ꍇ�ɑJ�ڐ���擾����
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
                throw new KeyNotFoundException("�Ή������Ԃ�����: " + key);
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
