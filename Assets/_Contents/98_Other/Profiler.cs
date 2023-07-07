using UnityEngine;

namespace InGameProfiler
{
    /// <summary>
    /// FPSとメモリ消費量をリアルタイムに表示する
    /// </summary>
    public class Profiler : MonoBehaviour
    {
        [SerializeField] float _interval = 0.5f;

        GUIStyle _style = new();
        GUIStyleState _styleState = new();
        float _timeCount;
        float _timer;
        float _fps;
        int _frameCount;
        float _used;
        float _unUsed;
        float _total;

        void Start()
        {
            _style.fontSize = 30;
            _styleState.textColor = Color.green;
            _style.normal = _styleState;
        }

        void Update()
        {
            _timer += Time.deltaTime;
            _timeCount += Time.timeScale / Time.deltaTime;
            _frameCount++;

            if (_timer > _interval)
            {
                _timer = 0;

                // FPS
                _fps = _timeCount / _frameCount;
                _timeCount = 0;
                _frameCount = 0;
                // メモリ
                _used = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024.0f * 1000);
                _unUsed = UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemoryLong() / (1024.0f * 1000);
                _total = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / (1024.0f * 1000);
            }
        }

        void OnGUI()
        {
            GUILayout.Label($"FPS: {_fps:f2}", _style);
            GUILayout.Label($"Used: {_used:f2}", _style);
            GUILayout.Label($"Unused: {_unUsed:f2}", _style);
            GUILayout.Label($"Total: {_total:f2}", _style);
        }
    }

}