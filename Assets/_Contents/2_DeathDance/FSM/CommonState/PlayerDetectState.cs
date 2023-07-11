using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    /// <summary>
    /// キャラクターがプレイヤーを発見した状態
    /// ボスと敵で共通の状態
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
