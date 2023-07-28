using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace MiniGame
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] DungeonBuilder _dungeonBuilder;
        [SerializeField] VectorFieldManager _vectorFieldManager;
        [SerializeField] GameObject _playerPrefab;

        async void Start()
        {
            _dungeonBuilder.Build();
            GameObject player = SpawnPlayer();

            // �������ꂽ�_���W�����ɍ��킹��̂ōŒ�ł�1�t���[���҂��Ȃ��Ƃ����Ȃ��B
            // �^�C�g���̃^�C�~���O�͂���
            await UniTask.Yield();
            _vectorFieldManager.CreateGrid();
            _vectorFieldManager.CreateVectorField(player.transform.position, FlowMode.Toward);
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