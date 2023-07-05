using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// ����֗��N���X
using CommonUtility;

/// <summary>
/// ���[�e�B���e�B�x�[�X�Ō��肳�ꂽ��ԂŎg�p����I�u�W�F�N�g�̗񋓌^
/// ��ԂƂ͕R�Â��Ă��Ȃ�
/// </summary>
public enum EnvironmentType
{
    WorkSpace,
    Chair,
    Bed,
}

/// <summary>
/// ���[�e�B���e�B�x�[�X�Ŏg������
/// </summary>
[DefaultExecutionOrder(-1)]
public class UtilityBlackBoard : MonoBehaviour
{
    /// <summary>
    /// �C���X�y�N�^�[���犄�蓖�Ă�p
    /// </summary>
    [System.Serializable]
    class Environment
    {
        [SerializeField] EnvironmentType _type;
        [SerializeField] GameObject _object;

        public EnvironmentType Type => _type;
        public GameObject Object => _object;
    }

    [Header("�e��ԂŎg�p�����")]
    [SerializeField] Environment[] _environments;
    [Header("�H�~")]
    [SerializeField] UtilityParam _food;
    [Header("�y����")]
    [SerializeField] UtilityParam _fun;
    [Header("��J")]
    [SerializeField] UtilityParam _tired;
    [Header("�ړ����x")]
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
    /// �L�����N�^�[����������̎���
    /// �e��Ԃ��Q�Ƃ���
    /// </summary>
    public GameObject this[EnvironmentType key]
    {
        get => DictUtility.TryGetValue(_environmentDict, key);
    }
    /// <summary>
    /// �L�����N�^�[���J�ډ\�ȏ�Ԃ̎���
    /// �e��Ԃ��J�ډ\
    /// </summary>
    public UtilityStateBase this[UtilityStateType key]
    {
        get => DictUtility.TryGetValue(_stateDict, key);
    }
    /// <summary>
    /// ���Ԋu�ŕ]���l��p���đI�����ꂽ��Ԃ�Controller���珑�����܂��
    /// ���s����State���ǂݎ��
    /// ���s���̏�ԂƈႤ��ԂȂ�΂��̏�ԂɑJ�ڂ���
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