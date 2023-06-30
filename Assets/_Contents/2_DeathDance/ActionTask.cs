using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace AnimationAction
{
    [System.Serializable]
    public class ActionTask
    {
        [SerializeField] Transform _transform;
        [SerializeField] Animator _animator;
        [SerializeField] GameObject _hitBox;

        public void Reset()
        {
            _hitBox.SetActive(false);
            _animator.Play("Idle");
        }

        public async UniTask ExecuteAsync(ActionData data, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            await Delay(data.BeforeDelay, token);
            _animator.Play(data.AnimName);
            if (data.AttackDuration > 0)
            {
                ActiveHitBoxAsync(data, token).Forget();
            }
            MoveAsync(data, token).Forget();
            await Delay(data.AnimLength, token);
            await Delay(data.AfterDelay, token);

            Debug.Log("行動終了");
        }

        async UniTask ActiveHitBoxAsync(ActionData data, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            await Delay(data.AttackDelay, token);
            _hitBox.SetActive(true);
            await Delay(data.AttackDuration, token);
            _hitBox.SetActive(false);
        }

        async UniTask MoveAsync(ActionData data, CancellationToken token)
        {
            float count = 0;
            Vector3 velo = data.MoveDistance / data.AnimLength * data.MoveDir;
            while (count < data.AnimLength)
            {
                token.ThrowIfCancellationRequested();

                _transform.Translate(velo * Time.deltaTime);
                count += Time.deltaTime;
                await UniTask.Yield(cancellationToken: token);
            }
        }

        /// <summary>
        /// UniTask.Delayメソッドのラッパー
        /// </summary>
        async UniTask Delay(float delay, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await UniTask.Delay(System.TimeSpan.FromSeconds(delay), cancellationToken: token);
        }
    }
}
