using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAIReference
{
    public class AIBrain : MonoBehaviour
    {
        public bool FinishedDeciding { get; set; }
        public Action BestAction { get; set; }
        NPCController _npc;

        void Start()
        {
            _npc = GetComponent<NPCController>();
        }

        void Update()
        {
            if(BestAction is null)
            {
                DecideBestAction(_npc._actionsAvailable);
            }
        }

        public void DecideBestAction(Action[] actionsAvailable)
        {
            float score = 0;
            int nextBestActionIndex = 0;
            for(int i = 0; i < actionsAvailable.Length; i++)
            {
                if (ScoreAction(actionsAvailable[i]) > score)
                {
                    nextBestActionIndex = i;
                    score = actionsAvailable[i].Score;
                }
            }

            BestAction = actionsAvailable[nextBestActionIndex];
            FinishedDeciding = true;
        }

        public float ScoreAction(Action action)
        {
            float score = 1.0f;
            for (int i = 0; i < action._considerations.Length; i++)
            {
                float considerationScore = action._considerations[i].ScoreConsideration(_npc);
                score *= considerationScore;

                if(score == 0)
                {
                    action.Score = 0;
                    return action.Score;
                }
            }

            // 平均化スキーム というらしい
            float originalScore = score;
            // ↓参考動画曰く、何故機能するのかわからない。デイヴマーク氏の本
            //   "ゲームAIのための行動数学"を参照との事
            float modFactor = 1 - (1 / action._considerations.Length);
            float makeupValue = (1 - originalScore) * modFactor;
            action.Score = originalScore + (makeupValue * originalScore);

            return action.Score;
        }
    }
}
