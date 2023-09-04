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
        [SerializeField] KurokamiSpawnModule _kurokamiSpawnModule;

        Leader _leader;
        Actor _kinpatsuLeader;
        List<Actor> _kinpatsuList = new();
        List<Actor> _kurokamiList = new();

        bool _initialized;
        // 任意のタイミングでキャラクターのリストを更新するための一時保存用のキュー
        Queue<Actor> _temp = new();

        async void Start()
        {
            _initialized = await InitAsync(this.GetCancellationTokenOnDestroy());
        }

        void Update()
        {
            if (!_initialized) return;

            // 制御するキャラクターのリストの更新
            AddControledActorFromTemp();

            // 死んだキャラクターをリストから削除
            if (_kinpatsuLeader != null) _kinpatsuLeader = null;
            _kinpatsuList.RemoveAll(actor => actor.IsDead);
            _kurokamiList.RemoveAll(actor => actor.IsDead);

            DebugLog();

            ForEachAll(actor => actor.StepParams());
            ForEachAll(actor => actor.Evaluate(new float[Utility.GetEnumLength<ActionType>() - 1]));
            ForEachAll(actor => actor.StepAction());

            // 一定間隔で黒髪を生成する
            _kurokamiSpawnModule.Step(transform.position);
        }

        void OnDestroy()
        {
            StatusBaseHolder.Release();
        }

        async UniTask<bool> InitAsync(CancellationToken token)
        {
            RegisterActorCallback();
            // キャラクターのステータス読み込み
            await StatusBaseHolder.LoadAsync(token);
            // フィールドの生成
            Cell[,] field = FieldManager.Instance.Create();
            // 初期金髪を配置
            _initKinpatsuSpawner.Spawn(field);

            return true;
        }

        /// <summary>
        /// ループ中にキャラクターの数が増加してリストの要素数が変わる事を防ぐために
        /// キャラクター生成時に一時保存用のキューに追加する
        /// </summary>
        void RegisterActorCallback()
        {
            Actor.OnSpawned += actor => _temp.Enqueue(actor);
            this.OnDisableAsObservable().Subscribe(_ => Actor.OnSpawned -= actor => _temp.Enqueue(actor));
        }

        /// <summary>
        /// 一時保存用のキュー中身を全て制御するキャラクターのリストに追加する
        /// </summary>
        void AddControledActorFromTemp()
        {
            while (_temp.Count > 0)
            {
                Actor actor = _temp.Dequeue();

                if (actor.Type == ActorType.KinpatsuLeader) _kinpatsuLeader = actor;
                else if (actor.Type == ActorType.Kinpatsu) _kinpatsuList.Add(actor);
                else if (actor.Type == ActorType.Kurokami) _kurokamiList.Add(actor);
                else
                {
                    string msg = "キャラクターの種類がNoneなのでControllerで制御不可能: " + actor.name;
                    throw new System.ArgumentException(msg);
                }
            }
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

        // デバッグ用
        void DebugLog()
        {
            Debug.Log($"リーダー:{_kinpatsuLeader} 金髪:{_kinpatsuList.Count} 黒髪:{_kurokamiList.Count}");
        }
    }
}