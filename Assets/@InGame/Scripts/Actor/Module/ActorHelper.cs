using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public static class ActorHelper
    {
        public static bool IsNeighbourOnGrid(Vector2Int a, Vector2Int b)
        {
            int dx = Mathf.Abs(a.x - b.x);
            int dy = Mathf.Abs(a.y - b.y);
            return dx <= 1 && dy <= 1;
        }

        /// <summary>
        /// �Ώۂ��ׂ̃Z���ɂ��邩�𒲂ׁA�ׂɂ���ꍇ�͌o�H���쐬����
        /// </summary>
        /// <returns>�o�H���쐬:true �ׂ̃Z���ł͂Ȃ�:false</returns>
        public static bool CreatePathIfNeighbourOnGrid(Vector2Int from, Vector2Int to, DataContext context)
        {
            if (IsNeighbourOnGrid(from, to))
            {
                // �ׂ̃Z���ɐH��������ꍇ�͈ړ����Ȃ��̂ŁA���ݒn���o�H�Ƃ��Ēǉ�����
                context.Path.Add(context.Transform.position);
                FieldManager.Instance.SetActorOnCell(context.Transform.position, context.Type);
                return true;
            }

            return false;
        }
    }
}