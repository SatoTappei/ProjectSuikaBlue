using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    // 撃破された際にそれぞれ呼ばれる
    public struct PlayerDefeatedMessage
    {
    }
    public struct AddScoreMessage
    {
        public int Score { get; set; }
    }
}