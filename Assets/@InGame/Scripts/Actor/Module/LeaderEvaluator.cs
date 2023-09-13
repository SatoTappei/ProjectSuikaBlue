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
        /// ���ɏ������܂ꂽ�l��p���ĕ]������B
        /// ���t���[���Ăяo����鎖��z�肵�Ă���B
        /// </summary>
        /// <returns>���ʂ̍���p�����]��</returns>
        public float[] Evaluate()
        {
            Array.Clear(_leaderEvaluate, 0, _leaderEvaluate.Length);
            // ���Ɏ��g�̈ʒu����������
            PublicBlackBoard.GatherPos = _context.Transform.position;

            // ���Ԋu�ŌQ����W��������
            _timer += Time.deltaTime;
            if (_timer > _context.Base.GatherInterval)
            {
                _timer = 0;
                _leaderEvaluate[(int)ActionType.Gather] = 11; // TODO:����
            }
            else
            {
                // �Q��̌̂����񂾐��ɉ����ďW���̕]���l�����肷��
                _leaderEvaluate[(int)ActionType.Gather] = PublicBlackBoard.DeathRate;
                PublicBlackBoard.DeathRate -= Time.deltaTime;
            }

            return _leaderEvaluate;
        }
    }
}