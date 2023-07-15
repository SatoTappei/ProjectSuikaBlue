using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    /// <summary>
    /// キャラクターがプレイヤーを発見していない状態
    /// ボスと敵で共通の状態
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
            //Debug.Log("未検知");

            // プレイヤーを検知した場合は発見状態へ遷移
            if (_blackBoard.IsDetectedPlayer)
            {
                TryChangeState(_blackBoard[EnemyStateType.PlayerDetected]);
            }
        }
    }
}
