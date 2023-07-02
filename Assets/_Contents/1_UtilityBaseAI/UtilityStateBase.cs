/// <summary>
/// �e��Ԃ�\���񋓌^
/// ��Ԃ��������ꍇ�͑Ή�����l��ǉ�����K�v������
/// </summary>
public enum UtilityStateType
{
    Base,
    Work,
    Eat,
    Sleep,
}

/// <summary>
/// ���[�e�B���e�B�x�[�X�őI�����ꂽ�e�s�����s����Ԃ̃N���X
/// </summary>
public abstract class UtilityStateBase
{
    enum Stage
    {
        Enter,
        Stay,
        Exit,
    }

    Stage _stage;
    UtilityStateBase _nextState;

    public UtilityStateBase(UtilityStateType type, UtilityBlackBoard blackBoard)
    {
        BlackBoard = blackBoard;
        Type = type;
    }

    public UtilityBlackBoard BlackBoard { get; }
    public UtilityStateType Type { get; }

    /// <summary>
    /// 1�x�̌Ăяo���ŃX�e�[�g�̒i�K�ɉ�����Enter() Stay() Exit()�̂����ǂꂩ1�����s�����
    /// </summary>
    public UtilityStateBase Update()
    {
        if (_stage == Stage.Enter)
        {
            Enter();
            _stage = Stage.Stay;
        }
        else if (_stage == Stage.Stay)
        {
            Stay();
        }
        else if (_stage == Stage.Exit)
        {
            Exit();
            _stage = Stage.Enter;

            return _nextState;
        }

        return this;
    }

    protected abstract void Enter();
    protected abstract void Stay();
    protected abstract void Exit();
    protected virtual void OnInvalid() { }
}
