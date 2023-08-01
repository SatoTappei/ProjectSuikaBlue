using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiniGame
{
    [RequireComponent(typeof(GameOver))]
    public class GameManager : MonoBehaviour
    {
        [SerializeField] DungeonBuilder _dungeonBuilder;
        [SerializeField] VectorFieldManager _vectorFieldManager;
        [SerializeField] EnemySpawner _enemySpawner;
        [SerializeField] GameObject _playerPrefab;
        [SerializeField] ClickOnceButton _startButton;
        [SerializeField] GameOver _gameOver;

        void Start()
        {
            CancellationToken token = this.GetCancellationTokenOnDestroy();
            StreamAsync(token).Forget();
        }

        async UniTaskVoid StreamAsync(CancellationToken token)
        {
            _dungeonBuilder.Build();
            GameObject player = SpawnPlayer();

            // �������ꂽ�_���W�����ɍ��킹��̂ōŒ�ł�1�t���[���҂��Ȃ��Ƃ����Ȃ��B
            // �^�C�g���{�^���N���b�N�܂ő҂B
            await _startButton.ClickedAsync(token);
            _vectorFieldManager.CreateGrid();
            _vectorFieldManager.CreateVectorField(player.transform.position, FlowMode.Toward);
            _enemySpawner.GenerateStart();
            MessageBroker.Default.Publish(new InGameStartMessage());

            // �v���C���[�����j���ꂽ�炪�߂��ׂ�
            await UniTask.WaitUntil(() => player.GetComponent<Player>().IsDefeated, cancellationToken: token);
            _enemySpawner.GenerateStop();
            MessageBroker.Default.Publish(new GameOverMessage());

            // ���g���C�{�^�����N���b�N���ꂽ�烊�g���C
            await _gameOver.WaitForRetryAsync(token);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        /// <summary>
        /// ���������_���W��������A�w�肵������'@'�ɑΉ������^�C���������^����擾
        /// ���̈ʒu�Ƀv���C���[�𐶐�����
        /// </summary>
        /// <returns>���������v���C���[</returns>
        GameObject SpawnPlayer()
        {
            if (!_dungeonBuilder.TileDataDict.TryGetValue('@', out List<GameObject> value))
            {
                throw new KeyNotFoundException("�v���C���[�̕����^�C�����o�^����Ă��Ȃ�: @");
            }

            Vector3 spawnPos = value[0].transform.position;
            return Instantiate(_playerPrefab, spawnPos, Quaternion.identity);
        }
    }
}