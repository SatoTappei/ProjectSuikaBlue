using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    /// <summary>
    /// �L�����N�^�[���v���C���[�𔭌����Ă��Ȃ����
    /// �{�X�ƓG�ŋ��ʂ̏��
    /// </summary>
    public class PlayerUndetectState : EnemyStateBase
    {
        CommonLayerBlackBoard _blackBoard;

        public PlayerUndetectState(EnemyStateType type, CommonLayerBlackBoard blackBoard) : base(type)
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
