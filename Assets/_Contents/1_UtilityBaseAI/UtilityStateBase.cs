/// <summary>
/// 各状態を表す列挙型
/// 状態が増えた場合は対応する値を追加する必要がある
/// </summary>
public enum UtilityStateType
{
    Base,
    Work,
    Eat,
    Sleep,
}

/// <summary>
/// ユーティリティベースで選択された各行動を行う状態のクラス
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
    /// 1度の呼び出しでステートの段階に応じてEnter() Stay() Exit()のうちどれか1つが実行される
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
