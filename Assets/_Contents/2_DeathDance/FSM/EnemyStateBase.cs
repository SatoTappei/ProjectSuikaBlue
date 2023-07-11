using UnityEngine;

/// <summary>
/// �G���ƃ{�X�ŋ��ʂ��Ă����Ԃ̑w�̗񋓌^
/// </summary>
public enum EnemyStateType
{
    Init,            
    PlayerUndetected,
    PlayerDetected,
    Defeated,
}

/// <summary>
/// �G�̎�肤���Ԃ̊��N���X
/// </summary>
public abstract class EnemyStateBase
{
    enum Stage
    {
        Enter,
        Stay,
        Exit,
    }

    Stage _stage;
    EnemyStateBase _nextState;

    public EnemyStateBase(EnemyStateType type)
    {
        Type = type;
    }

    public EnemyStateType Type { get; }

    /// <summary>
    /// 1�x�̌Ăяo���ŃX�e�[�g�̒i�K�ɉ�����Enter() Stay() Exit()�̂����ǂꂩ1�����s�����
    /// </summary>
    public EnemyStateBase Update()
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

    /// <summary>
    /// Enter()���Ă΂�Ă��A�X�e�[�g�̑J�ڏ������Ă�ł��Ȃ��ꍇ�̂ݑJ�ډ\
    /// </summary>
    public bool TryChangeState(EnemyStateBase nextState)
    {
        if (_stage == Stage.Enter)
        {
            Debug.LogWarning("Enter()���Ă΂��O�ɃX�e�[�g��J�ڂ��邱�Ƃ͕s�\: �J�ڐ�: " + nextState);
            return false;
        }
        else if (_stage == Stage.Exit)
        {
            Debug.LogWarning("���ɕʂ̃X�e�[�g�ɑJ�ڂ��鏈�����Ă΂�Ă��܂�: �J�ڐ�: " + nextState);
            return false;
        }

        _stage = Stage.Exit;
        _nextState = nextState;

        return true;
    }
}
