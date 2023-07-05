using UnityEngine;

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

    /// <summary>
    /// 黒板に書き込まれた状態が現在の状態と違う場合はその状態に遷移する
    /// </summary>
    public void TransitionIfStateChanged()
    {
        if (BlackBoard.SelectedStateType != Type) TryChangeState();
    }

    /// <summary>
    /// Enter()が呼ばれてかつ、ステートの遷移処理を呼んでいない場合のみ遷移可能
    /// </summary>
    bool TryChangeState()
    {
        // TODO: ここで黒板にある状態のリストから、同じく黒板にある
        //       選択された次の状態を指定しないといけないのが気持ち悪い。
        UtilityStateBase next = BlackBoard[BlackBoard.SelectedStateType];
        
        if (_stage == Stage.Enter)
        {
            Debug.LogWarning("Enter()が呼ばれる前にステートを遷移することは不可能: 遷移先: " + next);
            return false;
        }
        else if (_stage == Stage.Exit)
        {
            Debug.LogWarning("既に別のステートに遷移する処理が呼ばれています: 遷移先: " + next);
            return false;
        }

        _stage = Stage.Exit;
        _nextState = next;

        return true;
    }
}
