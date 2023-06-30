using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace AnimationAction
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] ActionTask _actionTask;
        [Header("使用するアニメーション")]
        [SerializeField] AnimationClip _clip;

        List<ActionData> _sequence = new();
        CancellationTokenSource _cts;

        void Awake()
        {
            _actionTask.Reset();

            ActionData data1 = new ActionData(_clip.name, _clip.length, 0, 0, 0.25f, 0.33f, transform.forward, 3.0f);
            ActionData data2 = new ActionData(_clip.name, _clip.length, 0, 0.25f, 0, 0, transform.right, 2.0f);
            ActionData data3 = new ActionData(_clip.name, _clip.length, 0, 0, 0, 0, -transform.right, 2.0f);

            _sequence.Add(data1);
            _sequence.Add(data2);
            _sequence.Add(data3);
        }

        public void Play()
        {
            Cancel();
            _cts = new CancellationTokenSource();
            ExecuteAsync(_cts.Token).Forget();
        }

        public void Cancel()
        {
            _cts?.Cancel();
            _actionTask.Reset();
        }

        async UniTaskVoid ExecuteAsync(CancellationToken token)
        {
            foreach (ActionData data in _sequence)
            {
                token.ThrowIfCancellationRequested();
                await _actionTask.ExecuteAsync(data, token);
            }
        }
    }
}