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
        [SerializeField] KurokamiSpawnController _kurokamiSpawnModule;
        [SerializeField] LeaderSelector _leaderSelector;

        Actor _leader;
        List<Actor> _kinpatsuList = new();
        List<Actor> _kurokamiList = new();
        bool _initialized;
        // 任意のタイミングでキャラクターのリストを更新するための一時保存用のキュー
        Queue<Actor> _temp = new();
        // リーダーがいない場合のダミーの評価配列
        float[] _dummyEvaluate = new float[Utility.GetEnumLength<ActionType>() - 1];

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
            ReleaseDeadActor();
            // 全キャラクター共通
            ForEachAll(actor => actor.StepParams());
            ForEachAll(actor => actor.StepAction());
            ForEachEvaluate(actor => actor.ResetOnEvaluateState());
            // リーダーの評価を元に金髪の評価を行う
            EvaluateKinpatsuFromLeader();
            // 黒髪の評価
            EvaluateKurokami();
            // 一定間隔で黒髪を生成する
            TickSpawnKurokami();
            // 一定間隔でリーダーを選出する
            TickSelectLeader();
        }

        void OnDestroy()
        {
            StatusBaseHolder.Release();
            PublicBlackBoard.Release();
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
        /// キャラクター生成時に一時保存用のキューに追加する という処理をコールバックに追加する
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

                if      (actor.Type == ActorType.KinpatsuLeader) _leader = actor;
                else if (actor.Type == ActorType.Kinpatsu) _kinpatsuList.Add(actor);
                else if (actor.Type == ActorType.Kurokami) _kurokamiList.Add(actor);
                else
                {
                    string msg = "キャラクターの種類がNoneなのでControllerで制御不可能: " + actor.name;
                    throw new System.ArgumentException(msg);
                }
            }
        }

        /// <summary>
        /// 死亡したフラグが立っているキャラクターを操作するキャラクターのリストから削除する
        /// </summary>
        void ReleaseDeadActor()
        {
            if (_leader != null && _leader.IsDead) _leader = null;
            _kinpatsuList.RemoveAll(actor => actor.IsDead);
            _kurokamiList.RemoveAll(actor => actor.IsDead);
        }

        void ForEachAll(UnityAction<Actor> action)
        {
            foreach (Actor kinpatsu in _kinpatsuList)
            {
                action.Invoke(kinpatsu);
            }
            foreach (Actor kurokami in _kurokamiList)
            {
                action.Invoke(kurokami);
            }
        }

        void ForEachEvaluate(UnityAction<Actor> action)
        {
            foreach (Actor kinpatsu in _kinpatsuList.Where(a => a.State == StateType.Evaluate))
            {
                action.Invoke(kinpatsu);
            }
            foreach (Actor kurokami in _kurokamiList.Where(a => a.State == StateType.Evaluate))
            {
                action.Invoke(kurokami);
            }
        }

        void EvaluateKinpatsuFromLeader()
        {
            float[] leaderEval = _leader != null ? _leader.LeaderEvaluate() : _dummyEvaluate;
            foreach (Actor kinpatsu in _kinpatsuList.Where(a => a.State == StateType.Evaluate))
            {
                kinpatsu.Evaluate(leaderEval);
            }
        }

        void EvaluateKurokami()
        {
            foreach (Actor kurokami in _kurokamiList.Where(a => a.State == StateType.Evaluate))
            {
                // 黒髪はリーダーがいないのでダミーの配列を使用
                kurokami.Evaluate(_dummyEvaluate);
            }
        }

        void TickSpawnKurokami()
        {
            _kurokamiSpawnModule.Tick(_kinpatsuList);
        }

        void TickSelectLeader()
        {
            if (_leaderSelector.Tick(_kinpatsuList, out Actor nextLeader))
            {
                _leader = nextLeader;
            }
        }

        // デバッグ用
        void DebugLog()
        {
            Debug.Log($"リーダー:{_leader} 金髪:{_kinpatsuList.Count} 黒髪:{_kurokamiList.Count}");
        }
    }
}