using UnityEngine;

namespace PathTableGraph
{
    /// <summary>
    /// デバッグ用の経路探索用のストップウォッチのクラス
    /// </summary>
    public class Stopwatch
    {
        System.Diagnostics.Stopwatch _stopwatch = new();
        string _prefixMessage;

        public Stopwatch(string prefixMessage = "")
        {
            _prefixMessage = prefixMessage;
        }

        public void Start()
        {
            _stopwatch.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
            Debug.Log($"{_prefixMessage} 処理時間: {_stopwatch.Elapsed} ms");
        }
    }
}
