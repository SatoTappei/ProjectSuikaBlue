using System.Collections;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace UtilityBaseAI
{
    public enum ActionType
    {
        Rest, // 休憩
        Eat,  // 食事
        Work, // 仕事
        Play, // 遊ぶ
        Move, // 移動中(お気に入りの動作に設定不可能)
    }

    [RequireComponent(typeof(UtilityParamView))]
    [RequireComponent(typeof(AgentPlacementHolder))]
    public class AgentController : MonoBehaviour
    {
        [SerializeField] AgentDesire _foodDesire;
        [SerializeField] AgentDesire _funDesire;
        [SerializeField] AgentDesire _fatigueDesire;
        [Header("全ての欲求が満たされている場合に実行し続ける動作")]
        [SerializeField] ActionType _favoriteActionType;

        UtilityParamView _uIController;
        AgentPlacementHolder _placementHolder;
        AgentTaskHolder _taskHolder = new();
        ActionType _currentBehavior = ActionType.Move;

        void Awake()
        {
            if (_favoriteActionType == ActionType.Move)
            {
                Debug.LogWarning("お気に入りの動作が 移動中 だったので 休憩 に強制変更");
                _favoriteActionType = ActionType.Rest;
            }

            _uIController = GetComponent<UtilityParamView>();
            _placementHolder = GetComponent<AgentPlacementHolder>();

            // 欲求を更新
            this.UpdateAsObservable().Subscribe(_ => 
            {
                _foodDesire.Decrease();
                _funDesire.Decrease();
                _fatigueDesire.Decrease();

                if (_foodDesire.BelowThreshold && _taskHolder.IsContain(ActionType.Eat))
                {
                    _taskHolder.Add(ActionType.Eat);
                }
                if (_funDesire.BelowThreshold && _taskHolder.IsContain(ActionType.Play))
                {
                    _taskHolder.Add(ActionType.Play);
                }
                if (_fatigueDesire.BelowThreshold && _taskHolder.IsContain(ActionType.Rest))
                {
                    _taskHolder.Add(ActionType.Rest);
                }

                //_uIController.SetFoodValue(_foodDesire.Current, AgentDesire.MaxValue);
                //_uIController.SetFunValue(_funDesire.Current, AgentDesire.MaxValue);
                //_uIController.SetEnergyValue(_fatigueDesire.Current, AgentDesire.MaxValue);
            });

            // 一定間隔でタスクを評価して
            Observable.Interval(System.TimeSpan.FromSeconds(1.0f)).Subscribe(_ => 
            {

            }).AddTo(this);
        }

        //IEnumerator MoveAndAction()
        //{

        //}
    }

    // hungryだったら食べる
    // fatigueだったら休憩する
    // funだったら仕事をする

    // 各動作には必要な時間がある
    // 最低限続ける時間後に定期的に続けるかどうか判定する

    // 値が一定以下になったらタスクを発行する
    // タスクをキューに追加していく
    // 実行タイミングでタスクを評価して実行すべきなら実行、そうでないなら破棄する

    // 実行タイミングで必要な値が閾値以下か調べる
    // 各動作
}