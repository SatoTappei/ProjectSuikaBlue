using System;
using UnityEngine;

namespace PSB.InGame
{
    public class LeaderEvaluator
    {
        static float _timer;

        DataContext _context;
        float[] _leaderEvaluate = new float[Utility.GetEnumLength<ActionType>() - 1];

        public LeaderEvaluator(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 黒板に書き込まれた値を用いて評価する。
        /// 毎フレーム呼び出される事を想定している。
        /// </summary>
        /// <returns>共通の黒板を用いた評価</returns>
        public float[] Evaluate()
        {
            Array.Clear(_leaderEvaluate, 0, _leaderEvaluate.Length);
            // 黒板に自身の位置を書き込む
            PublicBlackBoard.GatherPos = _context.Transform.position;

            // 一定間隔で群れを集合させる
            _timer += Time.deltaTime;
            if (_timer > _context.Base.GatherInterval)
            {
                _timer = 0;
                _leaderEvaluate[(int)ActionType.Gather] = 11; // TODO:調整
            }
            else
            {
                // 群れの個体が死んだ数に応じて集合の評価値を決定する
                _leaderEvaluate[(int)ActionType.Gather] = PublicBlackBoard.DeathRate;
                PublicBlackBoard.DeathRate -= Time.deltaTime;
            }

            return _leaderEvaluate;
        }
    }
}