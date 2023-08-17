using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Buffers;
using System;

namespace PSB.DebugOnly
{
    /// <summary>
    /// 1つのfor文の中で3つのメソッドを呼ぶのと、3つのfor文の中で1つずつメソッドを呼ぶので
    /// 処理速度がどれくらい違うのかテスト
    /// 結果: 目に見えるほど変わるわけではない。多少前者の方が軽いが縛られるほどではない。
    /// </summary>
    public class Example : MonoBehaviour
    {
        void Start()
        {
            int length = 5;
            float[] buffer = ArrayPool<float>.Shared.Rent(length);
            float[] dummy = buffer.AsSpan(0, length).ToArray();

            Debug.Log(dummy.Length); // 5

            ArrayPool<float>.Shared.Return(buffer);
        }

        void Do()
        {
            MyUtility.Stopwatch sw2 = new("1*3");
            sw2.Start();
            G();
            sw2.Stop();

            MyUtility.Stopwatch sw = new("3*1");
            sw.Start();
            F();
            sw.Stop();
        }

        void F()
        {
            for (int i = 0; i < 10000; i++)
            {
                M();
                N();
                O();
            }
        }

        void G()
        {
            for (int i = 0; i < 10000; i++)
            {
                M();
            }
            for (int i = 0; i < 10000; i++)
            {
                N();
            }
            for (int i = 0; i < 10000; i++)
            {
                O();
            }
        }

        void M()
        {
            Debug.Log("M");
        }

        void N()
        {
            Debug.Log("N");
        }

        void O()
        {
            Debug.Log("O");
        }
    }
}
