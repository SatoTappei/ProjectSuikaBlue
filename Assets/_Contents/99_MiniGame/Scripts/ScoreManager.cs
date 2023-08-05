using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace MiniGame
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] Text _text;

        int _currentScore;

        void Awake()
        {
            ResetScore();

            MessageBroker.Default.Receive<InGameStartMessage>()
                .Subscribe(msg => ResetScore()).AddTo(this);
            MessageBroker.Default.Receive<AddScoreMessage>()
                .Subscribe(msg => AddScore(msg.Score)).AddTo(this);
        }

        void ResetScore()
        {
            _currentScore = 0;
            _text.text = _currentScore.ToString();
        }

        void AddScore(int score)
        {
            _currentScore += score;
            _text.text = _currentScore.ToString();
        }
    }
}
