using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;

namespace MiniGame
{
    [RequireComponent(typeof(GameOver))]
    public class GameManager : MonoBehaviour
    {
        [SerializeField] WithEntityDungeonBuilder _dungeonBuilder;
        [SerializeField] VectorFieldManager _vectorFieldManager;
        [SerializeField] PlayerSpawner _playerSpawner;
        [SerializeField] EnemySpawner _enemySpawner;
        [SerializeField] ClickOnceButton _startButton;
        [SerializeField] GameOver _gameOver;

        void Start()
        {
            CancellationToken token = this.GetCancellationTokenOnDestroy();
            StreamAsync(token).Forget();
        }

        // �V�[���̃����[�h�������ECS�����Ή������Ă��Ȃ��̂ŃG���[���o��
        async UniTaskVoid StreamAsync(CancellationToken token)
        {
            // �X�e�[�W�ƃv���C���[����
            _dungeonBuilder.Build();
            Player player = _playerSpawner.Spawn(GetPlayerSpawnPos());

            // �������ꂽ�_���W�����ɍ��킹��̂ōŒ�ł�1�t���[���҂��Ȃ��Ƃ����Ȃ��B
            // �^�C�g���{�^���N���b�N�܂ő҂B
            await _startButton.ClickedAsync(token);
            _vectorFieldManager.CreateGrid();
            _vectorFieldManager.CreateVectorField(player.transform.position, FlowMode.Toward);

            while (true)
            {
                // �Q�[���X�^�[�g�A�G�̐����J�n
                MessageBroker.Default.Publish(new InGameStartMessage());
                _enemySpawner.GenerateStart();

                // �v���C���[�����j���ꂽ�炪�߂��ׂ�
                await UniTask.WaitUntil(() => player.IsDefeated, cancellationToken: token);
                
                // �G�̐������~�߂�
                MessageBroker.Default.Publish(new GameOverMessage());
                _enemySpawner.GenerateStop();

                // ���g���C�{�^�����N���b�N���ꂽ��1�t���[���҂��ă��g���C
                await _gameOver.WaitForRetryAsync(token);
                await UniTask.Yield();
            }
        }

        Vector3 GetPlayerSpawnPos()
        {
            if (!_dungeonBuilder.TileDataDict.TryGetValue('@', out List<GameObject> value))
            {
                throw new KeyNotFoundException("�v���C���[�̕����^�C�����o�^����Ă��Ȃ�: @");
            }

            return value[0].transform.position;
        }
    }
}