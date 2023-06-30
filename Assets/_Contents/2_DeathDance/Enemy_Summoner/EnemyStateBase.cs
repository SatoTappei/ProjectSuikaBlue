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

    public EnemyStateBase(UtilityStateType type)
    {
        Type = type;
    }

    public UtilityStateType Type { get; }

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
}
