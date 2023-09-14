using System;
using UnityEngine;

namespace PSB.InGame
{
    public static class Utility
    {
        const string CharTable = "abcdefghijklmnopqrstuvwxyz1234567890";

        public const string ColorCodeGreen = "#21ff37";
        public const string ColorCodeRed = "#ff6759";

        /// <summary>
        /// セルのScaleが 1 の場合に、隣接するセルをレイキャストで取得できる半径
        /// </summary>
        public const float NeighbourCellRadius = 1.45f;

        /// <summary>
        /// 右始まりで反時計回りの方向
        /// </summary>
        public static readonly Vector2Int[] Counterclockwise =
        {
            Vector2Int.right, 
            Vector2Int.up, 
            Vector2Int.left, 
            Vector2Int.down,
        };

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
        /// 自身 + 周囲八近傍 の位置を指定する用の方向の配列
        /// </summary>
        public static readonly Vector2Int[] SelfAndEightDirections =
        {
            new Vector2Int(0, 0), // 自身
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

        /// <summary>
        /// ランダムな文字列を取得
        /// </summary>
        /// <returns>length文字のa~zもしくは1~9 で構成された文字列</returns>
        public static string GetRandomString(int length = 8)
        {
            string s = string.Empty;
            for(int i = 0; i < length; i++)
            {
                int r = UnityEngine.Random.Range(0, CharTable.Length);
                s += CharTable[r];
            }

            return s;
        }
    }
}