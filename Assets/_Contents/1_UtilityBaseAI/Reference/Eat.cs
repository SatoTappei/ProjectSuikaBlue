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
            Debug.Log("�H����");
            //npc.stats.hunger -= 1;
            // TODO:�����������鏈���A�H�ׂ�����������Ă��������������

            npc.OnFinishedAction();
        }
    }
}
