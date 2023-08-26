using System;
using UnityEngine;

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

    public class ActionEvaluator : MonoBehaviour
    {
        // 繁殖、食べ物、水分、うろうろの場合は最低値に++
        // 殺害、寿命の場合は1に++
        // 0より大きくすることで自殺や虚空に向かって攻撃するのを防ぐ
        const float Offset = 0.01f;

        // 行動の数だけ評価値を格納するための配列
        float[] _evaluate = new float[Utility.GetEnumLength<ActionType>() - 1];

        [SerializeField] AnimationCurve _breedCurve;
        [SerializeField] AnimationCurve _foodCurve;
        [SerializeField] AnimationCurve _waterCurve;
        [SerializeField] AnimationCurve _wanderCurve;

        /// <summary>
        /// 評価値は0~1の値とオフセットを組み合わせた値
        /// 死亡は他より最優先なので 0.01 足される
        /// 他は最低値に 0.01 足される
        /// </summary>
        /// <returns>各行動の評価値の配列</returns>
        public float[] Evaluate(Status status, IReadOnlyGeneParams enemy = null)
        {
            Array.Fill(_evaluate, 0);

            // 体力が0で死ぬ、それ以外の要因で死ぬを選択しないように特別な値を取る
            _evaluate[(int)ActionType.Killed] = status.Hp.IsBelowZero ? (1 + Offset) : 0;

            // 寿命が0で死ぬ、それ以外の要因で死ぬを選択しないように特別な値を取る
            _evaluate[(int)ActionType.Senility] = status.LifeSpan.IsBelowZero ? (1 + Offset) : 0;

            // 敵がいる場合は攻撃/逃げるのどちらかを選択する
            //  基本は0、攻撃対象がいる状態の場合は体力とサイズに基づく
            if (enemy != null)
            {
                float eval = 5;
                // 自身より大きい
                if (status.Size < enemy.Size) eval -= 1.5f;
                // 自身より小さい
                if (status.Size >= enemy.Size) eval += 1.5f;
                // 色が自分より濃い
                int r = status.ColorR - enemy.ColorR;
                int g = status.ColorG - enemy.ColorG;
                int b = status.ColorB - enemy.ColorB;
                // +だと相手の方が濃い、-だと相手の方が薄い
                if (r + g + b >= 0)
                {
                    eval -= 1.5f;
                }
                // 色が自分より薄い
                else
                {
                    eval += 1.5f;
                }
                // 体力がn以上
                if(status.Hp.Percentage > 0.75f)
                {
                    eval += 2.0f;
                }
                // 体力がn以下
                if (status.Hp.Percentage < 0.33f)
                {
                    eval -= 2.0f;
                }

                eval = Mathf.Clamp01(eval);

                // 攻撃と逃げるの評価値は足して丁度1になる
                _evaluate[(int)ActionType.Attack] = eval;
                _evaluate[(int)ActionType.Escape] = 1 - eval;

                return _evaluate;
            }
            // 敵がいない場合は飲んだり食べたりうろうろしたりする
            else
            {
                // 集合
                //  0で固定、リーダーの評価のみ

                // 繁殖
                if (status.BreedingReady)
                {
                    float breed = _breedCurve.Evaluate(status.BreedingRate.Percentage);
                    _evaluate[(int)ActionType.Breed] = Mathf.Clamp01(breed);
                }

                // 食べ物を探す評価
                float food = _foodCurve.Evaluate(status.Food.Percentage) + Offset;
                _evaluate[(int)ActionType.SearchFood] = Mathf.Clamp01(food);

                // 水を探す評価
                float water = _waterCurve.Evaluate(status.Water.Percentage) + Offset;
                _evaluate[(int)ActionType.SearchWater] = Mathf.Clamp01(water);

                // うろうろする評価。食べ物と水のうち大きい方を評価する。
                float wander = Mathf.Max(status.Food.Percentage, status.Water.Percentage);
                wander = _wanderCurve.Evaluate(wander) + Offset;
                _evaluate[(int)ActionType.Wander] = Mathf.Clamp01(wander);

                return _evaluate;
            }
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