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
            Debug.Log("GMのStart開始");
            _dungeonBuilder.gameObject.SetActive(true);
            await UniTask.DelayFrame(10);
            _vectorFieldManager.gameObject.SetActive(true);
            Debug.Log("GMのStart終わり");
            GameObject player = SpawnPlayer();
            //_vectorFieldManager.CreateGrid();
            //await UniTask.DelayFrame(10);
            //_vectorFieldManager.CreateVectorField(player.transform.position, FlowMode.Toward);
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

// VFのRayを飛ばしてもダンジョンの壁に障害物のレイヤーが割り当てられていないので市場に動作しない
// 解決、呼び出し順の問題だった