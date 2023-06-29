using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAIReference
{
    [CreateAssetMenu(fileName = "HungerConsideration", menuName = "UtilityAI/Considerations/Hunger Consideration")]
    public class HungerConsideration : Consideration
    {
        [SerializeField] AnimationCurve _responseCurve;

        public override float ScoreConsideration(NPCController npc)
        {
            //Score = _responseCurve.Evaluate(Mathf.Clamp01(npc.stats.hunger / 100.0f));
            return Score;
        }
    }
}
