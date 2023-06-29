using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAIReference
{
    [CreateAssetMenu(fileName = "Eat", menuName = "UtilityAI/Actions/Eat")]
    public class Eat : Action
    {
        public override void Execute(NPCController npc)
        {
            Debug.Log("H–’†");
            //npc.stats.hunger -= 1;
            // TODO:Š‹à‚ªŒ¸‚éˆ—AH‚×‚½‚ç‹à‚ªŒ¸‚Á‚Ä‚¨• ‚ª–‚½‚³‚ê‚é

            npc.OnFinishedAction();
        }
    }
}
