using UnityEngine;

namespace PathTableGraph
{
    /// <summary>
    /// デバッグ用の経路探索用のストップウォッチのクラス
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
            Debug.Log($"{_start}から{_goal}への経路探索にかかった時間: {_stopwatch.Elapsed} ms");
        }
    }
}
