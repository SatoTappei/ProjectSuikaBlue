using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UniRx;

namespace PSB.InGame
{
    public class ActorController : MonoBehaviour
    {
        [SerializeField] int _spawnRadius = 5;

        Leader _leader;
        Actor _kinpatsuLeader;
        LinkedList<Actor> _kinpatsuList = new();
        LinkedList<Actor> _kurokamiList = new();

        Queue<Actor> _tempQueue = new();

        void Awake()
        {
            Actor.OnSpawned += AddActor;
        }

        void OnDestroy()
        {
            Actor.OnSpawned -= AddActor;
        }

        void Update()
        {
            AddFromTemp();

            // 自動でターンが進むターンベースと考えればLogicが書きやすいかもしれない

            if (Input.GetKeyDown(KeyCode.Space)) TrySpawnKurokami();
            // テスト:キー入力で集合させる
            if (Input.GetKey(KeyCode.LeftShift))
            {
                ForEachAll(actor => actor.Leader = _kinpatsuLeader.transform);
                float[] eval = _kinpatsuLeader.LeaderEvaluate();
                ForEachAll(actor => actor.Evaluate(eval));
            }
            else
            {
                ForEachAll(actor => actor.Evaluate()); // <- テストで毎フレーム評価する
            }

            ForEachAll(actor => actor.StepParams());
            ForEachAll(actor => actor.StepAction());
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

        void AddActor(Actor actor)
        {
            _tempQueue.Enqueue(actor);
            //if      (actor.Type == ActorType.KinpatsuLeader) _kinpatsuLeader = actor;
            //else if (actor.Type == ActorType.Kinpatsu)       _kinpatsuList.AddLast(actor);
            //else if (actor.Type == ActorType.Kurokami)       _kurokamiList.AddLast(actor);
            //else
            //{
            //    string msg = "キャラクターの種類がNoneなのでControllerで制御不可能: " + actor.name;
            //    throw new System.ArgumentException(msg);
            //}
        }

        void AddFromTemp()
        {
            while (_tempQueue.Count > 0)
            {
                Actor actor = _tempQueue.Dequeue();

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