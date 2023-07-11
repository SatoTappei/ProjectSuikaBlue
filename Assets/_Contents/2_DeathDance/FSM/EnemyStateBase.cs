using UnityEngine;

/// <summary>
/// 雑魚とボスで共通している状態の層の列挙型
/// </summary>
public enum EnemyStateType
{
    Init,            
    PlayerUndetected,
    PlayerDetected,
    Defeated,
}

/// <summary>
/// 敵の取りうる状態の基底クラス
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
    /// 1度の呼び出しでステートの段階に応じてEnter() Stay() Exit()のうちどれか1つが実行される
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
    /// Enter()が呼ばれてかつ、ステートの遷移処理を呼んでいない場合のみ遷移可能
    /// </summary>
    public bool TryChangeState(EnemyStateBase nextState)
    {
        if (_stage == Stage.Enter)
        {
            Debug.LogWarning("Enter()が呼ばれる前にステートを遷移することは不可能: 遷移先: " + nextState);
            return false;
        }
        else if (_stage == Stage.Exit)
        {
            Debug.LogWarning("既に別のステートに遷移する処理が呼ばれています: 遷移先: " + nextState);
            return false;
        }

        _stage = Stage.Exit;
        _nextState = nextState;

        return true;
    }
}
