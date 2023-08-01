using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

namespace MiniGame
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] DungeonBuilder _dungeonBuilder;
        [SerializeField] VectorFieldManager _vectorFieldManager;
        [SerializeField] EnemySpawner _enemySpawner;
        [SerializeField] GameObject _playerPrefab;
        [SerializeField] ClickOnceButton _startButton;

        void Awake()
        {

        }

        async void Start()
        {
            _dungeonBuilder.Build();
            GameObject player = SpawnPlayer();

            // �������ꂽ�_���W�����ɍ��킹��̂ōŒ�ł�1�t���[���҂��Ȃ��Ƃ����Ȃ��B
            // �^�C�g���{�^���N���b�N�܂ő҂B
            CancellationToken token = this.GetCancellationTokenOnDestroy();
            if (await _startButton.ClickedAsync(token).SuppressCancellationThrow()) return;

            _vectorFieldManager.CreateGrid();
            _vectorFieldManager.CreateVectorField(player.transform.position, FlowMode.Toward);
            _enemySpawner.GenerateStart();

            // �C���Q�[���J�n�̃��b�Z�[�W���O
            MessageBroker.Default.Publish(new InGameStartMessage());
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