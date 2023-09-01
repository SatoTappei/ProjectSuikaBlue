using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace PSB.InGame
{
    public interface IReadOnlyBreedingParam
    {
        uint Gene { get; }
    }

    public enum Sex
    {
        None,
        Male,
        Female,
    }

    /// <summary>
    /// 繁殖は以下の流れで行う
    /// 1.ステートに遷移時、繁殖待機のメッセージを送信する
    /// 2.繁殖マネージャ側が繁殖待機の個体同士に経路があるか調べ、マッチングさせる。
    /// 3.マッチングした場合はこのステートが性別とパートナーのメッセージを受信する。
    /// 4.経路探索 <- 必要？
    /// </summary>
    public class BreedState : BaseState
    {
        // この時間をオーバーした場合は強制的に評価ステートに遷移する
        const float TimeOut = 30.0f;

        Transform _actor;
        Stack<Vector3> _path = new();
        Vector3 _currentCellPos;
        Vector3 _nextCellPos;
        float _lerpProgress;
        float _speedModify = 1;
        float _timer;
        // マッチング用
        Transform _partner;
        Sex _sex;

        bool HasPath => _path != null;
        bool OnNextCell => _actor.position == _nextCellPos;
        // マッチングした際にメッセージを受信してtrueになる
        bool IsMatching => _partner != null && _sex != Sex.None;
        bool IsDeath => Context.NextState.Type == StateType.Killed ||
                        Context.NextState.Type == StateType.Senility;

        public BreedState(DataContext context) : base(context, StateType.Breed)
        {
            _actor = context.Transform;
        }

        protected override void Enter()
        {
            _timer = 0;
            SubscribeReceiveMessage();
            SendMessage();
        }

        protected override void Exit()
        {
            SendCancelMessage();
            _partner = null;
            _sex = Sex.None;
        }

        protected override void Stay()
        {
            //// タイマーを進める。時間切れの場合は評価ステートに遷移
            //if (!StepTimer()) { ToEvaluateState(); return; }
            //// マッチングしていない状態で死んだ場合はセル上にいるので死亡ステートに遷移して大丈夫
            //if (!IsMatching && IsDeath) { ToEvaluateState(); return; }
            //// マッチングしているかチェック
            //if (!IsMatching) return;
            //// 経路があるかチェック
            //if (!HasPath) return;

            //if (_sex == Sex.Male)
            //{
            //    if (OnNextCell)
            //    {
            //        if (IsDeath) { ToEvaluateState(); return; }

            //        if (!TryStepNextCell())
            //        {
            //            // 番の雌にサインを送信し、受信した雌が出産の処理を実行する
            //            OnArrivalNeighbourPertner();
            //            ToEvaluateState();
            //        }
            //    }
            //    else
            //    {
            //        Move();
            //    }
            //}
            //else if (_sex == Sex.Female)
            //{
            //    // 雌はその場で待機、メッセージを受信したら出産
            //}
        }

        /// <summary>
        /// タイマーを進める
        /// </summary>
        /// <returns>時間内:true 時間切れ:false</returns>
        bool StepTimer()
        {
            _timer += Time.deltaTime;
            return _timer < TimeOut;
        }

        void SubscribeReceiveMessage()
        {
            // マッチングした
            MessageBroker.Default.Receive<MatchingMessage>()
                .Where(msg => msg.ID == _actor.GetInstanceID())
                .Subscribe(MatchingComplete).AddTo(_actor);
            // パートナーからのサイン
            MessageBroker.Default.Receive<BreedingPartnerMessage>()
                .Where(msg => msg.ID == _actor.GetInstanceID())
                .Subscribe(PartnerSign).AddTo(_actor);
            // パートナーがマッチングをキャンセルした
            MessageBroker.Default.Receive<CancelBreedingMessage>()
                .Where(_ => _partner != null)
                .Where(msg => msg.Actor == _partner)
                .Subscribe(MatchingCancel).AddTo(_actor);
        }

        void SendMessage()
        {
            MessageBroker.Default.Publish(new BreedingMessage() { Actor = _actor });
        }

        void SendCancelMessage()
        {
            MessageBroker.Default.Publish(new CancelBreedingMessage() { Actor = _actor });
        }

        void OnArrivalNeighbourPertner()
        {
            //// 雌に産ませる
            //MessageBroker.Default.Publish(new BreedingPartnerMessage() { ID = _partner.GetInstanceID() });
            //// 登録された繁殖率を0にする処理を実行
            //_blackBoard.OnMaleBreedingInvoke();
        }

        void MatchingComplete(MatchingMessage msg)
        {
            // マッチング情報のセット
            _sex = msg.Sex;
            _partner = msg.Partner;
            // 経路探索
            //bool hasPath = TryPathfinding();
            //if (!hasPath) throw new System.NullReferenceException("繁殖ステートのパートナーへの経路がnull");
            
            TryStepNextCell();
        }

        void MatchingCancel(CancelBreedingMessage _)
        {
            //ToEvaluateState();
        }

        void PartnerSign(BreedingPartnerMessage msg)
        {
            // 産む処理の実行
            uint gene = _partner.GetComponent<IReadOnlyBreedingParam>().Gene;
            //_blackBoard.OnFemaleBreedingInvoke(gene);
            
            // 評価ステートに遷移
            //ToEvaluateState();
        }

        //void ToEvaluateState() => TryChangeState(_blackBoard.EvaluateState);

        //bool TryPathfinding()
        //{
        //    return FieldManager.Instance.TryGetPath(_actor.position, _partner.position, out _path);
        //}

        /// <summary>
        /// 現在のセルの位置を自身の位置で更新する。
        /// 次のセルの位置をあれば次のセルの位置、なければ自身の位置で更新する。
        /// </summary>
        /// <returns>次のセルがある:true 次のセルが無い(目的地に到着):false</returns>
        bool TryStepNextCell()
        {
            _currentCellPos = _actor.position;

            if (_path.TryPop(out _nextCellPos))
            {
                // 経路のセルとキャラクターの高さが違うので水平に移動させるために高さを合わせる
                _nextCellPos.y = _actor.position.y;
                Modify();
                Look();
                _lerpProgress = 0;

                return true;
            }

            _nextCellPos = _actor.position;

            return false;
        }

        void Look()
        {
            Vector3 dir = _nextCellPos - _currentCellPos;
            //_blackBoard.Model.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }

        /// <summary>
        /// 斜め移動の速度を補正する
        /// </summary>
        void Modify()
        {
            bool dx = Mathf.Approximately(_currentCellPos.x, _nextCellPos.x);
            bool dz = Mathf.Approximately(_currentCellPos.z, _nextCellPos.z);

            _speedModify = (dx || dz) ? 1 : 0.7f;
        }

        void Move()
        {
            //_lerpProgress += Time.deltaTime * _blackBoard.Speed * _speedModify;
            _actor.position = Vector3.Lerp(_currentCellPos, _nextCellPos, _lerpProgress);
        }

        // デバッグ用
        void MatchingLog()
        {
            Debug.Log($"マッチング 自身:{_actor.name} と 相手:{_partner.name} 自身の性別:{_sex}");
        }
    }
}