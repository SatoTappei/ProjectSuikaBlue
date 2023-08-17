using UnityEngine;
using System;
using System.Linq;

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

    public static class EvaluateUtility
    {
        public const float Dead = 100;
    }

    public class ActionEvaluator : MonoBehaviour
    {
        // �s���̐������]���l���i�[���邽�߂̔z��
        float[] _evaluate = new float[Utility.GetEnumLength<ActionType>() - 1];

        [SerializeField] AnimationCurve _breedCurve;
        [SerializeField] AnimationCurve _foodCurve;
        [SerializeField] AnimationCurve _waterCurve;
        [SerializeField] AnimationCurve _wanderCurve;

        /// <summary>
        /// �]���l��0~1�̒l�����A���S�̂ݗ�O�ōŗD��ɂ��邽�߁A���ʂȒl���Ƃ�B
        /// </summary>
        /// <returns>�e�s���̕]���l�̔z��</returns>
        public float[] Evaluate(Status status)
        {
            Array.Fill(_evaluate, 0);

            // �̗͂�0�Ŏ���
            if (status.Hp.IsBelowZero)
            {
                _evaluate[(int)ActionType.Killed] = EvaluateUtility.Dead;
            }

            // ������0�Ŏ���
            if (status.LifeSpan.IsBelowZero)
            {
                _evaluate[(int)ActionType.Senility] = EvaluateUtility.Dead;
            }

            // TODO:�G�ɑ΂��čU���B�̗͂Ǝ��g�̃T�C�Y�����Ɍ��߂�
            // TODO:������
            // TODO:�W��(���ۂɂ̓��[�_�[���Q������đI�����鍀�ڂȂ̂�0�ő��v�H)

            // �ɐB
            if (status.BreedingReady)
            {
                float breed = status.BreedingRate.Percentage * _breedCurve.Evaluate(status.BreedingRate.Percentage);
                _evaluate[(int)ActionType.Breed] = Mathf.Clamp01(breed);
            }

            // �H�ו���T���]��
            float food = status.Food.Percentage * _foodCurve.Evaluate(status.Food.Percentage);
            _evaluate[(int)ActionType.SearchFood] = Mathf.Clamp01(food);

            // ����T���]��
            float water = status.Water.Percentage * _waterCurve.Evaluate(status.Water.Percentage);
            _evaluate[(int)ActionType.SearchWater] = Mathf.Clamp01(water);

            // ���낤�낷��]���B�H�ו��Ɛ��̂������Ȃ�����]������B
            float wander = Mathf.Min(status.Food.Percentage, status.Water.Percentage);
            wander *= _wanderCurve.Evaluate(wander);
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