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

        Vector3 _scorePanelDefaultPos;

        void Start()
        {
            _scorePanelDefaultPos = _scorePanel.position;
            ResetScorePos();
            _retryButton.Invalid();
        }

        /// <summary>
        /// ���g���C�{�^�����N���b�N�����܂ő҂�
        /// </summary>
        public async UniTask WaitForRetryAsync(CancellationToken token)
        {
            // ���g���C�{�^���\��
            _retryButton.Valid();
            // �X�R�AUI�̃A�j���[�V������await����K�v�Ȃ�
            Sequence sequence = DOTween.Sequence();
            sequence.Join(_scorePanel.DOLocalMove(Vector3.zero, _scorePanelTweenDuration).SetEase(_ease));
            sequence.Join(_scorePanel.DOScale(new Vector3(1.5f, 1.5f, 1), _scorePanelTweenDuration).SetEase(_ease));
            sequence.SetLink(gameObject);
            
            if (await _retryButton.ClickedAsync(token).SuppressCancellationThrow()) return;
            sequence.Kill();
            ResetScorePos();
        }

        void ResetScorePos()
        {
            _scorePanel.position = _scorePanelDefaultPos;
            _scorePanel.localScale = Vector3.one;
        }
    }
}
