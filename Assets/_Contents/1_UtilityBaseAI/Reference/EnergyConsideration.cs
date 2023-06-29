using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAIReference
{
    [CreateAssetMenu(fileName = "EnergyConsideration", menuName = "UtilityAI/Considerations/Energy Consideration")]
    public class EnergyConsideration : Consideration
    {
        [SerializeField] AnimationCurve _responseCurve;

        public override float ScoreConsideration(NPCController npc)
        {
            //Score = _responseCurve.Evaluate(Mathf.Clamp01(npc.stats.energy / 100.0f));
            return Score;
        }
    }
}
