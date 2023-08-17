using UnityEngine;
using System;
using System.Linq;

namespace PSB.InGame
{
    public enum ActionType
    {
        Killed,      // 殺された
        Senility,    // 寿命
        Attack,      // 攻撃
        Escape,      // 逃げる
        Gather,      // 集合
        Breed,       // 繁殖
        SearchFood,  // 食料を探す
        SearchWater, // 水を探す
        Wander,      // うろうろ

        // 値で配列の添え字の指定をするので末尾に追加する
        None,
    }

    public static class EvaluateUtility
    {
        public const float Dead = 100;
    }

    public class ActionEvaluator : MonoBehaviour
    {
        // 行動の数だけ評価値を格納するための配列
        float[] _evaluate = new float[Utility.GetEnumLength<ActionType>() - 1];

        [SerializeField] AnimationCurve _breedCurve;
        [SerializeField] AnimationCurve _foodCurve;
        [SerializeField] AnimationCurve _waterCurve;
        [SerializeField] AnimationCurve _wanderCurve;

        /// <summary>
        /// 評価値は0~1の値だが、死亡のみ例外で最優先にするため、特別な値をとる。
        /// </summary>
        /// <returns>各行動の評価値の配列</returns>
        public float[] Evaluate(Status status)
        {
            Array.Fill(_evaluate, 0);

            // 体力が0で死ぬ
            if (status.Hp.IsBelowZero)
            {
                _evaluate[(int)ActionType.Killed] = EvaluateUtility.Dead;
            }

            // 寿命が0で死ぬ
            if (status.LifeSpan.IsBelowZero)
            {
                _evaluate[(int)ActionType.Senility] = EvaluateUtility.Dead;
            }

            // TODO:敵に対して攻撃。体力と自身のサイズを元に決める
            // TODO:逃げる
            // TODO:集合(実際にはリーダーが群れを見て選択する項目なので0で大丈夫？)

            // 繁殖
            if (status.BreedingReady)
            {
                float breed = status.BreedingRate.Percentage * _breedCurve.Evaluate(status.BreedingRate.Percentage);
                _evaluate[(int)ActionType.Breed] = Mathf.Clamp01(breed);
            }

            // 食べ物を探す評価
            float food = status.Food.Percentage * _foodCurve.Evaluate(status.Food.Percentage);
            _evaluate[(int)ActionType.SearchFood] = Mathf.Clamp01(food);

            // 水を探す評価
            float water = status.Water.Percentage * _waterCurve.Evaluate(status.Water.Percentage);
            _evaluate[(int)ActionType.SearchWater] = Mathf.Clamp01(water);

            // うろうろする評価。食べ物と水のうち少ない方を評価する。
            float wander = Mathf.Min(status.Food.Percentage, status.Water.Percentage);
            wander *= _wanderCurve.Evaluate(wander);
            _evaluate[(int)ActionType.Wander] = Mathf.Clamp01(wander);

            return _evaluate;
        }

        public static ActionType SelectMax(float[] myEvaluate, float[] leaderEvaluate)
        {
            if (myEvaluate.Length != leaderEvaluate.Length)
            {
                string msg = "評価値の配列の長さが違う: " + myEvaluate.Length + " " + leaderEvaluate.Length;
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
                throw new Exception("評価値を計算した結果、Noneが一番高いのはおかしい");
            }

            return type;
        }
    }
}