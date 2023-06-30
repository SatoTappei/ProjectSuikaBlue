using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStateType
{
    Idle,
    Battle,
    Avoid,
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

    public EnemyStateBase(UtilityStateType type)
    {
        Type = type;
    }

    public UtilityStateType Type { get; }

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
}
