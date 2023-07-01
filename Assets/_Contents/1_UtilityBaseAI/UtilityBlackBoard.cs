using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
public class UtilityBlackBoard : MonoBehaviour
{
    /// <summary>
    /// �C���X�y�N�^�[���犄�蓖�Ă�p
    /// </summary>
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
    [Header("���C")]
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
                Debug.LogError("�Ή������������: " + key);
                return null;
            }
        }
    }

    /// <summary>
    /// Controller���珑�����܂�āAState���ǂݎ��
    /// </summary>
    public UtilityStateType SelectedStateType { get; set; }

    void Awake()
    {
        _environmentDict = _environments.ToDictionary(e => e.Type, e => e.Object);
    }
}