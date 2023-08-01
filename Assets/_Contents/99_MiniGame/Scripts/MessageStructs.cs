using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    // ゲームの状態が遷移した際にそれぞれ送信されるメッセージ
    public struct InGameStartMessage { }
    public struct GameOverMessage { }

    public struct PlayerDefeatedMessage { }
    public struct AddScoreMessage
    {
        public int Score { get; set; }
    }
}