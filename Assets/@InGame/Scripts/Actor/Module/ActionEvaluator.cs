using System;
using UnityEngine;

namespace PSB.InGame
{
    public enum ActionType
    {
        Killed,      // �E���ꂽ
        Senility,    // ����
        Attack,      // �U��
        Escape,      // ������
        Gather,      // �W��
        Breed,       // �ɐB
        SearchFood,  // �H����T��
        SearchWater, // ����T��
        Wander,      // ���낤��

        // �l�Ŕz��̓Y�����̎w�������̂Ŗ����ɒǉ�����
        None,
    }

    public class ActionEvaluator
    {
        readonly DataContext _context;
        float[] _evaluate = new float[Utility.GetEnumLength<ActionType>() - 1];

        public ActionEvaluator(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// ���g�̏�Ԃ����ɕ]����A�ǉ��̕]���l�Ƒg�ݍ��킹�Ď��̍s����I������
        /// </summary>
        /// <returns>���̍s���ɑΉ�����ActorType</returns>
        public ActionType SelectAction(float[] addEvaluate)
        {
            Evaluate();
            return SelectMax(addEvaluate);
        }

        /// <summary>
        /// �]���l�� 0~1 �̒l�B�������A�̗͂�����ꍇ�Ɏ��S��A�G�����Ȃ��̂ɍU���̂悤��
        /// ���̏�Ԃł͐�΂Ɏ��Ȃ��U���Ɋւ��Ă� -1 �ɂȂ��Ă���B
        /// </summary>
        /// <returns>�e�s���̕]���l�̔z��</returns>
        float[] Evaluate()
        {
            // -1�Ŗ��߂邱�Ƃŕ]���l��0�̏ꍇ�ł�����ȉ��Ƃ������ɂȂ�
            // �ˑR�̎��E��A�G�����Ȃ��ꍇ�ɍU��/�������h�����Ƃ��o����B
            Array.Fill(_evaluate, -1);

            // �̗͂�0�Ȃ�E�Q�����
            if (_context.HP.IsBelowZero)
            {
                _evaluate[(int)ActionType.Killed] = 1;
            }
            // ������0�Ȃ�剝��
            else if (_context.LifeSpan.IsBelowZero)
            {
                _evaluate[(int)ActionType.Senility] = 1;
            }
            // �G������ꍇ�͍U��/������̂ǂ��炩��I������
            else if (_context.IsEnemyDetected)
            {
                // �Ō��10�Ŋ���A0~1�̒l�ɒ����̂Ŕ��X��5�ŌŒ�
                float eval = 5;

                // �T�C�Y�̕]���A�������傫���ꍇ��- �������ꍇ��+
                if (_context.Size < _context.Enemy.Size) eval -= _context.Base.SizeEvalFactor;
                else if (_context.Size >= _context.Enemy.Size) eval += _context.Base.SizeEvalFactor;

                // �F�̕]���A�������Z���ꍇ��+ ������蔖���ꍇ��-
                int r = _context.ColorR - _context.Enemy.ColorR;
                int g = _context.ColorG - _context.Enemy.ColorG;
                int b = _context.ColorB - _context.Enemy.ColorB;
                if (r + g + b >= 0) eval -= _context.Base.ColorEvalFactor;
                else if(r + g + b < 0) eval += _context.Base.ColorEvalFactor;

                // �̗͂̕]���An�ȏ�Ȃ�+�An�ȉ��Ȃ�-�A�ǂ���ł��Ȃ���Εω��Ȃ�
                if (_context.HP.Percentage > _context.Base.AttackHpThreshold) eval += _context.Base.HpEvalFactor;
                else if (_context.HP.Percentage < _context.Base.EscapeHpThreshold) eval -= _context.Base.HpEvalFactor;

                eval /= 10;
                eval = Mathf.Clamp01(eval);

                // �U���Ɠ�����̕]���l�͑����Ē��x1�ɂȂ�
                _evaluate[(int)ActionType.Attack] = eval;
                _evaluate[(int)ActionType.Escape] = 1 - eval;
            }
            // �G�����Ȃ��ꍇ�͈��񂾂�H�ׂ��肤�낤�낵���肷��
            else
            {
                // �ɐB
                if (_context.BreedingReady)
                {
                    float breed = _context.Base.BreedEvaluate(_context.BreedingRate.Percentage);
                    _evaluate[(int)ActionType.Breed] = Mathf.Clamp01(breed);
                }

                // �H�ו���T���]��
                float food = _context.Base.FoodEvaluate(_context.Food.Percentage);
                _evaluate[(int)ActionType.SearchFood] = Mathf.Clamp01(food);

                // ����T���]��
                float water = _context.Base.WaterEvaluate(_context.Water.Percentage);
                _evaluate[(int)ActionType.SearchWater] = Mathf.Clamp01(water);

                // ���낤�낷��]���B�H�ו��Ɛ��̂����傫������]������B
                float wander = Mathf.Max(_context.Food.Percentage, _context.Water.Percentage);
                wander = _context.Base.WanderEvaluate(wander);
                _evaluate[(int)ActionType.Wander] = Mathf.Clamp01(wander);
            }

            return _evaluate;
        }

        /// <summary>
        /// ���g�̕]���̊e�l�ƒǉ��̕]���̊e�l��P���ɉ��Z���A�]���l���ő�̍s����I��
        /// </summary>
        /// <returns>�]���l����ԍ����s��</returns>
        ActionType SelectMax(float[] addEvaluate)
        {
            if (_evaluate.Length != addEvaluate.Length)
            {
                string msg = "�]���l�̔z��̒������Ⴄ: " + _evaluate.Length + " " + addEvaluate.Length;
                throw new System.ArgumentException(msg);
            }

            float max = -1;
            int index = -1;
            for (int i = 0; i < _evaluate.Length; i++)
            {
                _evaluate[i] += addEvaluate[i];
                if (_evaluate[i] > max)
                {
                    max = _evaluate[i];
                    index = i;
                }
            }

            ActionType type = (ActionType)index;

            if (type == ActionType.None)
            {
                throw new Exception("�]���l���v�Z�������ʁANone����ԍ����̂͂�������");
            }

            return type;
        }
    }
}