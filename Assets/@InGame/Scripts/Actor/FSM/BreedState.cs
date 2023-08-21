using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace PSB.InGame
{
    public enum Sex
    {
        None,
        Male,
        Female,
    }

    public class BreedState : BaseState
    {
        IBlackBoardForState _blackBoard;
        Transform _actor;
        Transform _partner;
        Sex _sex;
        Stack<Vector3> _path = new();
        // マッチングした際にメッセージを受信してtrueになる
        bool IsMatching => _partner != null && _sex != Sex.None;

        public BreedState(IBlackBoardForState blackBoard) : base(StateType.Breed)
        {
            _blackBoard = blackBoard;
            _actor = blackBoard.Transform;
        }

        protected override void Enter()
        {
            SubscribeMatchingMessage();
            SendMessage();
        }

        protected override void Exit()
        {
            SendCancelMessage();
        }

        protected override void Stay()
        {
            // 番は2体の繁殖がいないと成立しない。
            // Enterのタイミングでメッセージングするが、そのタイミングで番がいない場合、パートナーと性別のデータが来ない。
            // ので、Stayのタイミングでパートナーと性別のデータが来ているか調べる必要がる。

            if (!IsMatching) return;

            MatchingLog();

            if (_blackBoard.Sex == Sex.Male)
            {
                //// メスの所まで移動する
                //Vector3 femalePos = _blackBoard.Partner.GetComponent<IBlackBoardForState>().Transform.position;
                //// メスまでの経路
                //FieldManager.Instance.TryGetPath(_actor.position, femalePos, out _path);

            }
            else if (_blackBoard.Sex == Sex.Female)
            {
                // その場で待機
                // 
            }

            //var partner = _blackBoard.Partner;
            //var sex = _blackBoard.Sex;

            // 問題:作り直し、やはり自分自身(Actorクラス)を送信してマッチングするクラスを作る方が良い気がする。
            // 問題:オスはメスの箇所に移動しないといけないのだが、経路が取得できなかった場合はどうする？
            // 問題:オスもしくはメスが繁殖中に死んだ場合のキャンセル処理
            //      →繁殖中は死なないようにすれば良い？
            //      →死亡の判定は評価ステートに依存しているので繁殖が終わったら評価ステートに遷移すればよい

            // 繁殖したい個体がメッセージング
            // 繁殖したい個体がメッセージング
            // ↑この2体を交配させる
            // 繁殖したい個体は繁殖ステートに入ってその場で待機している
            // Matchingクラスはキューを持っている。2体入る毎に先頭から2体抜き出してMatchingさせる。
            // 片方がオス、もう片方がメスとして設定する
            // オスがパートナーの隣まで移動  メスはパートナーが来るまで待機
            // 経路が無い場合はどうなる？
            // 時間経過                      時間経過
            // 評価ステートに遷移            子供を生成 -> 評価ステートに遷移

            // とりあえずすぐ評価ステートに遷移させる。
            //ToEvaluateState();

            // 近場の他の個体を探す
            //  どこかが個体のリストを保持している必要がある
            // 隣のセルまで移動する
            //  繁殖相手も繁殖ステートに遷移する必要がある。
            //  どうやって相手に伝えるか
            //  メッセージング
            //   攻めがメッセージを送信する ステート内で送信する
            //   受けがメッセージを受信する
            //   受けがメッセージを送信する
            //   攻めがメッセージを受信する
            //  メッセージの受信 -> リーダーの評価 -> 個体の評価 の順
            //  繁殖状態(受け)と繁殖状態(攻め)がある？
            //   受け:待つ
            //   攻め:受けに向けて移動
            //   受け:繁殖率0に
            //   攻め:繁殖率0に
            //   受け:スポナーから生成

            //  案:疑似的な雄雌を付ける?

            // 繁殖
        }

        void SubscribeMatchingMessage()
        {
            MessageBroker.Default.Receive<MatchingMessage>().Subscribe(ReceiveMessage).AddTo(_actor);
        }

        void SendMessage()
        {
            MessageBroker.Default.Publish(new BreedingMessage() { Actor = _actor });
        }

        void SendCancelMessage()
        {
            MessageBroker.Default.Publish(new CancelBreedingMessage() { Actor = _actor });
        }

        void ReceiveMessage(MatchingMessage msg)
        {
            if (msg.ID == _actor.GetInstanceID())
            {
                _sex = msg.Sex;
                _partner = msg.Partner;
            }
        }

        void ToEvaluateState() => TryChangeState(_blackBoard.EvaluateState);

        void MatchingLog()
        {
            Debug.Log($"マッチング {_actor.name} と {_partner.name} 自身の性別:{_sex}");
        }
    }
}
