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

        // シーンのリロードをするとECS側が対応させていないのでエラーが出る
        async UniTaskVoid StreamAsync(CancellationToken token)
        {
            // ステージとプレイヤー生成
            _dungeonBuilder.Build();
            Player player = _playerSpawner.Spawn(GetPlayerSpawnPos());

            // 生成されたダンジョンに合わせるので最低でも1フレーム待たないといけない。
            // タイトルボタンクリックまで待つ。
            await _startButton.ClickedAsync(token);
            _vectorFieldManager.CreateGrid();
            _vectorFieldManager.CreateVectorField(player.transform.position, FlowMode.Toward);

            while (true)
            {
                // ゲームスタート、敵の生成開始
                MessageBroker.Default.Publish(new InGameStartMessage());
                _enemySpawner.GenerateStart();

                // プレイヤーが撃破されたらがめおべら
                await UniTask.WaitUntil(() => player.IsDefeated, cancellationToken: token);
                
                // 敵の生成を止める
                MessageBroker.Default.Publish(new GameOverMessage());
                _enemySpawner.GenerateStop();

                // リトライボタンがクリックされたら1フレーム待ってリトライ
                await _gameOver.WaitForRetryAsync(token);
                await UniTask.Yield();
            }
        }

        Vector3 GetPlayerSpawnPos()
        {
            if (!_dungeonBuilder.TileDataDict.TryGetValue('@', out List<GameObject> value))
            {
                throw new KeyNotFoundException("プレイヤーの沸くタイルが登録されていない: @");
            }

            return value[0].transform.position;
        }
    }
}