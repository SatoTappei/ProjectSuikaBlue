using UnityEngine;

namespace MyUtility
{
    /// <summary>
    /// �f�o�b�O�p�̃X�g�b�v�E�H�b�`�̃N���X
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
            Debug.Log($"{_prefixMessage} ��������: {_stopwatch.Elapsed} ms");
        }
    }
}
