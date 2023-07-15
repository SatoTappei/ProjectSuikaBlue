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

    public VisibleTargetType Type => _type;
    public Vector3 Pos => _pos;
    public float Time => _time;
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
    /// <summary>
    /// ���E�ɑ������^�[�Q�b�g����ނ��ƂɃ��X�g�ŕێ����Ă���
    /// ��ނ��Ƃɕ����̃^�[�Q�b�g��ێ��\
    /// </summary>
    Dictionary<VisibleTargetType, List<VisibleTargetData>> _visibleTargetDict;
    int _currentHp;
    
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
    /// <summary>
    /// ���E�ɑ����Ă��郊�X�g�Ƀv���C���[���ǉ�����Ă���΃v���C���[�����m���Ă���Ƃ݂Ȃ�
    /// </summary>
    public bool IsDetectedPlayer => _visibleTargetDict[VisibleTargetType.Player].Count > 0;
    
    public int CurrentHp 
    { 
        get => _currentHp;
        set => _currentHp = value;
    }
    public Dictionary<VisibleTargetType, List<VisibleTargetData>> VisibleTargetDict 
    {
        get => _visibleTargetDict;
        set => _visibleTargetDict = value; 
    }

    void Awake()
    {
        _currentHp = _maxHp;
        CreateState();
        CreateVisibleTargetDict();
    }

    void CreateState()
    {
        _stateDict = new(4);
        _stateDict.Add(EnemyStateType.Init, new InitState(EnemyStateType.Init, this));
        _stateDict.Add(EnemyStateType.PlayerDetected, new PlayerDetectState(EnemyStateType.PlayerDetected, this));
        _stateDict.Add(EnemyStateType.PlayerUndetected, new PlayerUndetectState(EnemyStateType.PlayerUndetected, this));
        _stateDict.Add(EnemyStateType.Defeated, new DefeatedState(EnemyStateType.Init, this));
    }

    void CreateVisibleTargetDict()
    {
        _visibleTargetDict = new();
        _visibleTargetDict.Add(VisibleTargetType.Player, new());
    }
}
