using UnityEngine;
using System;

namespace PSB.InGame
{
    public enum ActionType
    {
        Killed,      // E‚³‚ê‚½
        Senility,    // õ–½
        Attack,      // UŒ‚
        Escape,      // “¦‚°‚é
        Breed,       // ”ÉB
        SearchFood,  // H—¿‚ğ’T‚·
        SearchWater, // …‚ğ’T‚·
        Wander,      // ‚¤‚ë‚¤‚ë

        // ’l‚Å”z—ñ‚Ì“Y‚¦š‚Ìw’è‚ğ‚·‚é‚Ì‚Å––”ö‚É’Ç‰Á‚·‚é
        None,
    }

    public static class EvaluateUtility
    {
        public const float Dead = 100;
    }

    public class ActionEvaluator : MonoBehaviour
    {
        // s“®‚Ì”‚¾‚¯•]‰¿’l‚ğŠi”[‚·‚é‚½‚ß‚Ì”z—ñ
        float[] _evaluate = new float[Utility.GetEnumLength<ActionType>() - 1];

        [SerializeField] AnimationCurve _breedCurve;
        [SerializeField] AnimationCurve _foodCurve;
        [SerializeField] AnimationCurve _waterCurve;
        [SerializeField] AnimationCurve _wanderCurve;

        /// <summary>
        /// •]‰¿’l‚Í0~1‚Ì’l‚¾‚ªA€–S‚Ì‚İ—áŠO‚ÅÅ—Dæ‚É‚·‚é‚½‚ßA“Á•Ê‚È’l‚ğ‚Æ‚éB
        /// </summary>
        /// <returns>Šes“®‚Ì•]‰¿’l‚Ì”z—ñ</returns>
        public float[] Evaluate(Status status)
        {
            Array.Fill(_evaluate, 0);

            // ‘Ì—Í‚ª0‚Å€‚Ê
            if (status.Hp.IsBelowZero)
            {
                _evaluate[(int)ActionType.Killed] = EvaluateUtility.Dead;
            }

            // õ–½‚ª0‚Å€‚Ê
            if (status.LifeSpan.IsBelowZero)
            {
                _evaluate[(int)ActionType.Senility] = EvaluateUtility.Dead;
            }

            // “G‚É‘Î‚µ‚ÄUŒ‚B‘Ì—Í‚Æ©g‚ÌƒTƒCƒY‚ğŒ³‚ÉŒˆ‚ß‚é
            // “¦‚°‚é

            // ”ÉB
            if (status.BreedingReady)
            {
                float breed = status.BreedingRate.Percentage * _breedCurve.Evaluate(status.BreedingRate.Percentage);
                _evaluate[(int)ActionType.Breed] = Mathf.Clamp01(breed);
            }

            // H‚×•¨‚ğ’T‚·•]‰¿
            float food = status.Food.Percentage * _foodCurve.Evaluate(status.Food.Percentage);
            _evaluate[(int)ActionType.SearchFood] = Mathf.Clamp01(food);

            // …‚ğ’T‚·•]‰¿
            float water = status.Water.Percentage * _waterCurve.Evaluate(status.Water.Percentage);
            _evaluate[(int)ActionType.SearchWater] = Mathf.Clamp01(water);

            // ‚¤‚ë‚¤‚ë‚·‚é•]‰¿BH‚×•¨‚Æ…‚Ì‚¤‚¿­‚È‚¢•û‚ğ•]‰¿‚·‚éB
            float wander = Mathf.Min(status.Food.Percentage, status.Water.Percentage);
            wander *= _wanderCurve.Evaluate(wander);
            _evaluate[(int)ActionType.Wander] = Mathf.Clamp01(wander);

            return _evaluate;
        }
    }
}