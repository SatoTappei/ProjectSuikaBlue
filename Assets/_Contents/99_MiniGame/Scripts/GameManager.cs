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

            // 生成されたダンジョンに合わせるので最低でも1フレーム待たないといけない。
            // タイトルボタンクリックまで待つ。
            await _startButton.ClickedAsync(token);
            _vectorFieldManager.CreateGrid();
            _vectorFieldManager.CreateVectorField(player.transform.position, FlowMode.Toward);
            _enemySpawner.GenerateStart();
            MessageBroker.Default.Publish(new InGameStartMessage());

            // プレイヤーが撃破されたらがめおべら
            await UniTask.WaitUntil(() => player.GetComponent<Player>().IsDefeated, cancellationToken: token);
            _enemySpawner.GenerateStop();
            MessageBroker.Default.Publish(new GameOverMessage());

            // リトライボタンがクリックされたらリトライ
            await _gameOver.WaitForRetryAsync(token);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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