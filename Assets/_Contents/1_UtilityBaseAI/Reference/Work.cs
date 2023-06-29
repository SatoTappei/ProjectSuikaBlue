using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAIReference
{
    [CreateAssetMenu(fileName = "Work", menuName = "UtilityAI/Actions/Work")]
    public class Work : Action
    {
        public override void Execute(NPCController npc)
        {
            npc.DoWork(3);
        }
    }
}
