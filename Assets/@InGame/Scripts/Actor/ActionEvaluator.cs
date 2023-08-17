using UnityEngine;
using System;

namespace PSB.InGame
{
    public enum ActionType
    {
        Killed,      // �E���ꂽ
        Senility,    // ����
        Attack,      // �U��
        Escape,      // ������
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

            // �G�ɑ΂��čU���B�̗͂Ǝ��g�̃T�C�Y�����Ɍ��߂�
            // ������

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
    }
}