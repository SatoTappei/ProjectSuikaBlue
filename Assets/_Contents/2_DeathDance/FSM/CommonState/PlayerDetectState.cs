using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    /// <summary>
    /// �L�����N�^�[���v���C���[�𔭌��������
    /// �{�X�ƓG�ŋ��ʂ̏��
    /// </summary>
    public class PlayerDetectState : EnemyStateBase
    {
        CommonLayerBlackBoard _blackBoard;

        public PlayerDetectState(EnemyStateType type, CommonLayerBlackBoard blackBoard) : base(type)
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
