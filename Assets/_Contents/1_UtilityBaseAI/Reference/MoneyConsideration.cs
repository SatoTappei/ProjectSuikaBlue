using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAIReference
{
    [CreateAssetMenu(fileName = "MoneyConsideration", menuName = "UtilityAI/Considerations/Money Consideration")]
    public class MoneyConsideration : Consideration
    {
        [SerializeField] AnimationCurve _responseCurve;

        public override float ScoreConsideration(NPCController npc)
        {
            //Score = _responseCurve.Evaluate(Mathf.Clamp01(npc.stats.money / 1000.0f));
            return Score;
        }
    }
}