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

    public class ActionEvaluator
    {
        readonly DataContext _context;
        float[] _evaluate = new float[Utility.GetEnumLength<ActionType>() - 1];

        public ActionEvaluator(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 自身の状態を元に評価後、追加の評価値と組み合わせて次の行動を選択する
        /// </summary>
        /// <returns>次の行動に対応したActorType</returns>
        public ActionType SelectAction(float[] addEvaluate)
        {
            Evaluate();
            return SelectMax(addEvaluate);
        }

        /// <summary>
        /// 評価値は 0~1 の値。ただし、体力がある場合に死亡や、敵がいないのに攻撃のような
        /// その状態では絶対に取らない攻撃に関しては -1 になっている。
        /// </summary>
        /// <returns>各行動の評価値の配列</returns>
        float[] Evaluate()
        {
            // -1で埋めることで評価値が0の場合でもそれ以下という事になり
            // 突然の自殺や、敵がいない場合に攻撃/逃げるを防ぐことが出来る。
            Array.Fill(_evaluate, -1);

            // 体力が0なら殺害される
            if (_context.HP.IsBelowZero)
            {
                _evaluate[(int)ActionType.Killed] = 1;
            }
            // 寿命が0なら大往生
            else if (_context.LifeSpan.IsBelowZero)
            {
                _evaluate[(int)ActionType.Senility] = 1;
            }
            // 敵がいる場合は攻撃/逃げるのどちらかを選択する
            else if (_context.IsEnemyDetected)
            {
                // 最後に10で割り、0~1の値に直すので半々の5で固定
                float eval = 5;

                // サイズの評価、自分より大きい場合は- 小さい場合は+
                if (_context.Size < _context.Enemy.Size) eval -= _context.Base.SizeEvalFactor;
                else if (_context.Size >= _context.Enemy.Size) eval += _context.Base.SizeEvalFactor;

                // 色の評価、自分より濃い場合は+ 自分より薄い場合は-
                int r = _context.ColorR - _context.Enemy.ColorR;
                int g = _context.ColorG - _context.Enemy.ColorG;
                int b = _context.ColorB - _context.Enemy.ColorB;
                if (r + g + b >= 0) eval -= _context.Base.ColorEvalFactor;
                else if(r + g + b < 0) eval += _context.Base.ColorEvalFactor;

                // 体力の評価、n以上なら+、n以下なら-、どちらでもなければ変化なし
                if (_context.HP.Percentage > _context.Base.AttackHpThreshold) eval += _context.Base.HpEvalFactor;
                else if (_context.HP.Percentage < _context.Base.EscapeHpThreshold) eval -= _context.Base.HpEvalFactor;

                eval /= 10;
                eval = Mathf.Clamp01(eval);

                // 攻撃と逃げるの評価値は足して丁度1になる
                _evaluate[(int)ActionType.Attack] = eval;
                _evaluate[(int)ActionType.Escape] = 1 - eval;
            }
            // 敵がいない場合は飲んだり食べたりうろうろしたりする
            else
            {
                // 繁殖
                if (_context.BreedingReady)
                {
                    float breed = _context.Base.BreedEvaluate(_context.BreedingRate.Percentage);
                    _evaluate[(int)ActionType.Breed] = Mathf.Clamp01(breed);
                }

                // 食べ物を探す評価
                float food = _context.Base.FoodEvaluate(_context.Food.Percentage);
                _evaluate[(int)ActionType.SearchFood] = Mathf.Clamp01(food);

                // 水を探す評価
                float water = _context.Base.WaterEvaluate(_context.Water.Percentage);
                _evaluate[(int)ActionType.SearchWater] = Mathf.Clamp01(water);

                // うろうろする評価。食べ物と水のうち大きい方を評価する。
                float wander = Mathf.Max(_context.Food.Percentage, _context.Water.Percentage);
                wander = _context.Base.WanderEvaluate(wander);
                _evaluate[(int)ActionType.Wander] = Mathf.Clamp01(wander);
            }

            return _evaluate;
        }

        /// <summary>
        /// 自身の評価の各値と追加の評価の各値を単純に加算し、評価値が最大の行動を選ぶ
        /// </summary>
        /// <returns>評価値が一番高い行動</returns>
        ActionType SelectMax(float[] addEvaluate)
        {
            if (_evaluate.Length != addEvaluate.Length)
            {
                string msg = "評価値の配列の長さが違う: " + _evaluate.Length + " " + addEvaluate.Length;
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
                throw new Exception("評価値を計算した結果、Noneが一番高いのはおかしい");
            }

            return type;
        }
    }
}