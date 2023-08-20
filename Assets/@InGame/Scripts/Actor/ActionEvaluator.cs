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

    public class ActionEvaluator : MonoBehaviour
    {
        // �ɐB�A�H�ו��A�����A���낤��̍Œ�l
        // 0���傫�����邱�ƂŎ��E�⋕��Ɍ������čU������̂�h��
        const float OffsetMin = 0.01f;

        // �s���̐������]���l���i�[���邽�߂̔z��
        float[] _evaluate = new float[Utility.GetEnumLength<ActionType>() - 1];

        [SerializeField] AnimationCurve _breedCurve;
        [SerializeField] AnimationCurve _foodCurve;
        [SerializeField] AnimationCurve _waterCurve;
        [SerializeField] AnimationCurve _wanderCurve;

        /// <summary>
        /// �]���l��0~1�̒l
        /// </summary>
        /// <returns>�e�s���̕]���l�̔z��</returns>
        public float[] Evaluate(Status status)
        {
            Array.Fill(_evaluate, 0);

            // �̗͂�0�Ŏ��ʁA����ȊO�̗v���Ŏ��ʂ�I�����Ȃ��悤�ɓ��ʂȒl�����
            _evaluate[(int)ActionType.Killed] = status.Hp.IsBelowZero ? 1 : 0;

            // ������0�Ŏ��ʁA����ȊO�̗v���Ŏ��ʂ�I�����Ȃ��悤�ɓ��ʂȒl�����
            _evaluate[(int)ActionType.Senility] = status.LifeSpan.IsBelowZero ? 1 : 0;

            // �U��
            //  ��{��0�A�U���Ώۂ������Ԃ̏ꍇ�̗͑͂ƃT�C�Y�Ɋ�Â�
            // ������
            //  ��{��0�A�U���Ώۂ������Ԃ̏ꍇ�̗͑͂ƃT�C�Y�Ɋ�Â�
            // �W��
            //  0�ŌŒ�A���[�_�[�̕]���̂�

            // �ɐB
            if (status.BreedingReady)
            {
                float breed = status.BreedingRate.Percentage * _breedCurve.Evaluate(status.BreedingRate.Percentage);
                _evaluate[(int)ActionType.Breed] = Mathf.Clamp01(breed);
            }

            // �H�ו���T���]��
            float food = _foodCurve.Evaluate(status.Food.Percentage) + OffsetMin;
            _evaluate[(int)ActionType.SearchFood] = Mathf.Clamp01(food);

            // ����T���]��
            float water = _waterCurve.Evaluate(status.Water.Percentage) + OffsetMin;
            _evaluate[(int)ActionType.SearchWater] = Mathf.Clamp01(water);

            // ���낤�낷��]���B�H�ו��Ɛ��̂����傫������]������B
            float wander = Mathf.Max(status.Food.Percentage, status.Water.Percentage);
            wander = _wanderCurve.Evaluate(wander) + OffsetMin;
            _evaluate[(int)ActionType.Wander] = Mathf.Clamp01(wander);

            return _evaluate;
        }

        public static ActionType SelectMax(float[] myEvaluate, float[] leaderEvaluate)
        {
            if (myEvaluate.Length != leaderEvaluate.Length)
            {
                string msg = "�]���l�̔z��̒������Ⴄ: " + myEvaluate.Length + " " + leaderEvaluate.Length;
                throw new System.ArgumentException(msg);
            }

            float max = -1;
            int index = -1;
            for (int i = 0; i < myEvaluate.Length; i++)
            {
                myEvaluate[i] += leaderEvaluate[i];
                if (myEvaluate[i] > max)
                {
                    max = myEvaluate[i];
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