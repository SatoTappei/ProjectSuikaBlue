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

        void Awake()
        {

        }

        async void Start()
        {
            Debug.Log("GM��Start�J�n");
            _dungeonBuilder.gameObject.SetActive(true);
            await UniTask.DelayFrame(10);
            _vectorFieldManager.gameObject.SetActive(true);
            Debug.Log("GM��Start�I���");
            GameObject player = SpawnPlayer();
            //_vectorFieldManager.CreateGrid();
            //await UniTask.DelayFrame(10);
            //_vectorFieldManager.CreateVectorField(player.transform.position, FlowMode.Toward);
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

// VF��Ray���΂��Ă��_���W�����̕ǂɏ�Q���̃��C���[�����蓖�Ă��Ă��Ȃ��̂Ŏs��ɓ��삵�Ȃ�
// �����A�Ăяo�����̖�肾����