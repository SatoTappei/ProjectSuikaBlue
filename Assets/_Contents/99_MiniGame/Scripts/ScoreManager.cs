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
            AddScore(0);

            MessageBroker.Default.Receive<AddScoreMessage>()
                .Subscribe(msg => AddScore(msg.Score)).AddTo(this);
        }

        void AddScore(int score)
        {
            _currentScore += score;
            _text.text = _currentScore.ToString();
        }
    }
}
