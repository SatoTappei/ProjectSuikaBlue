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
            Debug.Log("食事中");
            //npc.stats.hunger -= 1;
            // 所持金が減る処理、食べたら金が減ってお腹が満たされる

            npc.OnFinishedAction();
        }
    }
}
