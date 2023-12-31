using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGame
{
    [RequireComponent(typeof(Button))]
    public class ClickOnceButton : MonoBehaviour
    {
        [Header("消えるまでの時間")]
        [SerializeField] float delay = 0.5f;

        Button _button;

        void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void Valid() => _button.gameObject.SetActive(true);
        public void Invalid() => _button.gameObject.SetActive(false);

        /// <summary>
        /// クリックされてしばらくしたら消える
        /// </summary>
        public async UniTask ClickedAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await _button.OnClickAsync(token);
            await UniTask.Delay(System.TimeSpan.FromSeconds(delay), cancellationToken: token);
            _button.gameObject.SetActive(false);
        }
    }
}
