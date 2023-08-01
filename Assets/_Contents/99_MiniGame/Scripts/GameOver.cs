using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

namespace MiniGame
{
    public class GameOver : MonoBehaviour
    {
        [SerializeField] Transform _scorePanel;
        [SerializeField] ClickOnceButton _retryButton;
        [Header("�X�R�AUI��Tween�Ɋւ��鍀��")]
        [SerializeField] float _scorePanelTweenDuration = 1.0f;
        [SerializeField] Ease _ease;

        void Awake()
        {
            _retryButton.transform.localScale = Vector3.zero;
        }

        /// <summary>
        /// ���g���C�{�^�����N���b�N�����܂ő҂�
        /// </summary>
        public async UniTask WaitForRetryAsync(CancellationToken token)
        {
            _retryButton.transform.localScale = Vector3.one;
            // �X�R�AUI�̃A�j���[�V������await����K�v�Ȃ�
            DOTween.Sequence()
                .Join(_scorePanel.DOLocalMove(Vector3.zero, _scorePanelTweenDuration).SetEase(_ease))
                .Join(_scorePanel.DOScale(new Vector3(1.5f, 1.5f, 1), _scorePanelTweenDuration).SetEase(_ease))
                .SetLink(gameObject);
            
            if (await _retryButton.ClickedAsync(token).SuppressCancellationThrow()) return;
        }
    }
}
