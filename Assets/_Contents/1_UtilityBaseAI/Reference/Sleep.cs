using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAIReference
{
    [CreateAssetMenu(fileName = "Sleep", menuName = "UtilityAI/Actions/Sleep")]
    public class Sleep : Action
    {
        public override void Execute(NPCController npc)
        {
            npc.DoSleep(3);
        }
    }
}
