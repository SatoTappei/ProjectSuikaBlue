using UnityEngine;

namespace PathTableGraph
{
    /// <summary>
    /// �f�o�b�O�p�̌o�H�T���p�̃X�g�b�v�E�H�b�`�̃N���X
    /// </summary>
    public class Stopwatch
    {
        System.Diagnostics.Stopwatch _stopwatch = new();
        int _start;
        int _goal;

        public Stopwatch(int start, int goal)
        {
            _start = start;
            _goal = goal;
        }

        public void Start()
        {
            _stopwatch.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
            Debug.Log($"{_start}����{_goal}�ւ̌o�H�T���ɂ�����������: {_stopwatch.Elapsed} ms");
        }
    }
}
