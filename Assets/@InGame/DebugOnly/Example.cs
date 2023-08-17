using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Buffers;
using System;

namespace PSB.DebugOnly
{
    /// <summary>
    /// 1��for���̒���3�̃��\�b�h���ĂԂ̂ƁA3��for���̒���1�����\�b�h���ĂԂ̂�
    /// �������x���ǂꂭ�炢�Ⴄ�̂��e�X�g
    /// ����: �ڂɌ�����قǕς��킯�ł͂Ȃ��B�����O�҂̕����y����������قǂł͂Ȃ��B
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
