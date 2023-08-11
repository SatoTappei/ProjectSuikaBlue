using System;
using UnityEngine;

namespace PSB.InGame
{
    public class Utility
    {
        /// <summary>
        /// 周囲八近傍を指定する用の方向の配列
        /// </summary>
        public static readonly Vector2Int[] EightDirections =
        {
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(1, 0),
            new Vector2Int(1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, -1),
        };

        /// <summary>
        /// 列挙型の要素数を取得
        /// </summary>
        /// <returns>要素数</returns>
        public static int GetEnumLength<T>() where T : Enum => Enum.GetValues(typeof(T)).Length;
    }
}