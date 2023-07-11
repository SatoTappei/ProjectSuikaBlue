using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    /// <summary>
    /// �L�����N�^�[�����S�����ۂ̏��
    /// �{�X�ƓG�ŋ��ʂ̏��
    /// </summary>
    public class DefeatedState : EnemyStateBase
    {
        CommonLayerBlackBoard _blackBoard;

        public DefeatedState(EnemyStateType type, CommonLayerBlackBoard blackBoard) : base(type)
        {
            _blackBoard = blackBoard;
        }

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
        }
    }
}
