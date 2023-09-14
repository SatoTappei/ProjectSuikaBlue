using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public class LeaderSelector : MonoBehaviour
    {
        [SerializeField] float Rate = 1.0f;

        float _timer;

        /// <summary>
        /// 一定間隔でリーダーを選出する
        /// スコアが一番高い個体のリーダーフラグを立てる
        /// </summary>
        /// <returns>次のリーダーが選出:true 選出タイミング以外もしくは個体がいない:false</returns>
        public bool Tick(IReadOnlyList<Actor> candidate, out Actor leader)
        {
            _timer += Time.deltaTime;
            if (_timer > Rate)
            {
                _timer = 0;
                return TrySelect(candidate, out leader);
            }

            leader = null;
            return false;
        }

        bool TrySelect(IReadOnlyList<Actor> candidate, out Actor leader)
        {
            int max = int.MinValue;
            leader = null;
            foreach (Actor actor in candidate)
            {
                // 全個体のリーダーフラグを折る
                actor.IsLeader = false;

                if (actor.Score > max)
                {
                    max = actor.Score;
                    leader = actor;
                }
            }

            // 次のリーダーのフラグを立てる
            if (leader != null) leader.IsLeader = true;

            return leader != null;
        }
    }
}
