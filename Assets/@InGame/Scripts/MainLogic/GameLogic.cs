using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using UniRx.Triggers;

namespace PSB.InGame
{
    /// <summary>
    /// ゲーム全体のロジックの制御を行う。
    /// フィールドおよびキャラクターの生成、各キャラクターの更新処理を行う。
    /// </summary>
    public class GameLogic : MonoBehaviour
    {
        [SerializeField] InitKinpatsuSpawner _initKinpatsuSpawner;
        [SerializeField] int _spawnRadius = 5;

        Leader _leader;
        Actor _kinpatsuLeader;
        LinkedList<Actor> _kinpatsuList = new();
        LinkedList<Actor> _kurokamiList = new();

        bool _initialized;
        // 任意のタイミングでキャラクターのリストを更新するための一時保存用のキュー
        Queue<Actor> _temp = new();

        async void Start()
        {
            // キャラクターを生成した際にこのクラスで制御できるように登録する
            RegisterActorCallback();

            _initialized = await InitAsync(this.GetCancellationTokenOnDestroy());
        }

        void Update()
        {
            if (!_initialized) return;

            // 制御するキャラクターのリストの更新
            AddControledActorFromTemp();

            //// 自動でターンが進むターンベースと考えればLogicが書きやすいかもしれない

            //if (Input.GetKeyDown(KeyCode.Space)) TrySpawnKurokami();
            //// テスト:キー入力で集合させる
            //if (Input.GetKey(KeyCode.LeftShift))
            //{
            //    ForEachAll(actor => actor.Leader = _kinpatsuLeader.transform);
            //    float[] eval = _kinpatsuLeader.LeaderEvaluate();
            //    ForEachAll(actor => actor.Evaluate(eval));
            //}
            //else
            //{
            //    ForEachAll(actor => actor.Evaluate()); // <- テストで毎フレーム評価する
            //}

            //ForEachAll(actor => actor.StepParams());
            //ForEachAll(actor => actor.StepAction());
        }

        void RegisterActorCallback()
        {
            Actor.OnSpawned += AddSpawnedActorTemp;
            this.OnDisableAsObservable().Subscribe(_ => Actor.OnSpawned -= AddSpawnedActorTemp);
        }

        async UniTask<bool> InitAsync(CancellationToken token)
        {
            // キャラクターのステータス読み込み
            await StatusBaseHolder.LoadAsync(token);
            // フィールドの生成
            Cell[,] field = FieldManager.Instance.Create();
            // 初期金髪を配置
            _initKinpatsuSpawner.Spawn(field);

            return true;
        }

        /// <summary>
        /// リーダーの位置を中心に一定間隔離れた位置に生成する
        /// </summary>
        /// <returns>生成した:true 生成できなかった:false</returns>
        bool TrySpawnKurokami()
        {
            if (_kinpatsuLeader == null) return false;
          
            Vector3 spawnPos = _kinpatsuLeader.transform.position;
            foreach (Vector2Int dir in Utility.EightDirections.OrderBy(_ => System.Guid.NewGuid()))
            {
                Vector3 pos = spawnPos + new Vector3(dir.x, 0, dir.y) * _spawnRadius;

                // セルが取得出来た。セルが海以外、資源なし、キャラがいない場合は生成可能
                if (!FieldManager.Instance.TryGetCell(pos, out Cell cell)) continue;
                if (!cell.IsEmpty) continue;

                MessageBroker.Default.Publish(new KurokamiSpawnMessage() { Pos = cell.Pos });
                return true;
            }

            return false;
        }

        void ForEachAll(UnityAction<Actor> action)
        {
            if (_kinpatsuLeader != null)
            {
                action.Invoke(_kinpatsuLeader);
            }

            foreach (Actor kinpatsu in _kinpatsuList)
            {
                action.Invoke(kinpatsu);
            }
            foreach (Actor kurokami in _kurokamiList)
            {
                action.Invoke(kurokami);
            }
        }

        void AddSpawnedActorTemp(Actor actor) => _temp.Enqueue(actor);

        void AddControledActorFromTemp()
        {
            while (_temp.Count > 0)
            {
                Actor actor = _temp.Dequeue();

                if (actor.Type == ActorType.KinpatsuLeader) _kinpatsuLeader = actor;
                else if (actor.Type == ActorType.Kinpatsu) _kinpatsuList.AddLast(actor);
                else if (actor.Type == ActorType.Kurokami) _kurokamiList.AddLast(actor);
                else
                {
                    string msg = "キャラクターの種類がNoneなのでControllerで制御不可能: " + actor.name;
                    throw new System.ArgumentException(msg);
                }
            }
        }
    }
}