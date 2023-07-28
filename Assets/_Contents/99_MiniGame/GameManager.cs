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

            // 生成されたダンジョンに合わせるので最低でも1フレーム待たないといけない。
            // タイトルのタイミングはここ
            await UniTask.Yield();
            _vectorFieldManager.CreateGrid();
            _vectorFieldManager.CreateVectorField(player.transform.position, FlowMode.Toward);
        }
        
        /// <summary>
        /// 生成したダンジョンから、指定した文字'@'に対応したタイルを辞書型から取得
        /// その位置にプレイヤーを生成する
        /// </summary>
        /// <returns>生成したプレイヤー</returns>
        GameObject SpawnPlayer()
        {
            if (!_dungeonBuilder.TileDataDict.TryGetValue('@', out List<GameObject> value))
            {
                throw new KeyNotFoundException("プレイヤーの沸くタイルが登録されていない: @");
            }

            Vector3 spawnPos = value[0].transform.position;
            return Instantiate(_playerPrefab, spawnPos, Quaternion.identity);
        }
    }
}