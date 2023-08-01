using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyUtility
{
    public static class Utility
    {
        /// <summary>
        /// 0からlengthまでの数値がランダムに並んだ配列を返す
        /// ダステンフェルドのアルゴリズム
        /// </summary>
        public static int[] DurstenfeldShuffle(int length)
        {
            int[] array = new int[length];
            for(int i = 0; i < length; i++)
            {
                array[i] = i;
            }

            for(int i = length - 1; i >= 1; i--)
            {
                int r = Random.Range(0, i);
                int temp = array[i];
                array[i] = array[r];
                array[r] = temp;
            }

            return array;
        }
    }
}
