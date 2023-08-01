using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    // �Q�[���̏�Ԃ��J�ڂ����ۂɂ��ꂼ�ꑗ�M����郁�b�Z�[�W
    public struct InGameStartMessage { }
    public struct GameOverMessage { }

    public struct PlayerDefeatedMessage { }
    public struct AddScoreMessage
    {
        public int Score { get; set; }
    }
}