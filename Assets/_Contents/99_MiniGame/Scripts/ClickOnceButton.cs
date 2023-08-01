using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGame
{
    [RequireComponent(typeof(Button))]
    public class ClickOnceButton : MonoBehaviour
    {
        [Header("������܂ł̎���")]
        [SerializeField] float delay = 0.5f;

        Button _button;

        void Awake()
        {
            _button = GetComponent<Button>();
        }

        /// <summary>
        /// �N���b�N����Ă��΂炭�����������
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
