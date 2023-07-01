using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

/// <summary>
/// 各機能を用いてユーティリティベースAIを制御するクラス
/// </summary>
[RequireComponent(typeof(UtilityBlackBoard))]
[RequireComponent(typeof(UtilityParamEvaluator))]
[RequireComponent(typeof(UtilityStateHolder))]
public class UtilityAIController : MonoBehaviour
{
    /// <summary>
    /// 最適な評価値を選択する間隔
    /// </summary>
    const float UtilityUpdateInterval = 0.1f;

    void Awake()
    {
        UtilityParamEvaluator evaluator = GetComponent<UtilityParamEvaluator>();
        UtilityStateHolder stateHolder = GetComponent<UtilityStateHolder>();
        UtilityBodyLayer bodyLayer = GetComponent<UtilityBodyLayer>();
        UtilityBlackBoard blackBoard = GetComponent<UtilityBlackBoard>();
        UtilityParamToStateConverter converter = new();

        // 初期状態の状態を設定
        UtilityStateBase currentState = stateHolder.CreateStateAll();

        IObservable<Unit> update = this.UpdateAsObservable();
        // 評価値を取得 -> 身体の層で調整 -> 黒板に書き込む
        update.ThrottleFirst(TimeSpan.FromSeconds(UtilityUpdateInterval)).Subscribe(_ =>
        {
            UtilityParamType highestParam = evaluator.SelectHighestParamType();
            UtilityStateType nextState = converter.ConvertToState(highestParam);
            nextState = bodyLayer.Adjust(nextState);
            blackBoard.SelectedStateType = nextState;
        });
        // 評価値の自然減少＆現在の状態の更新
        update.Subscribe(_ => 
        {
            currentState.Update();
        });
    }
}

// ************************************************************************
// TOOD: 現在実行中の状態と黒板に書き込まれてる状態が違った場合は遷移させる
// ************************************************************************

// 一番近い箇所まで行って作業をする
// 休憩する(食事)
// 家に戻る

// 評価値の更新
// 一定間隔で評価値を基にステートの遷移をさせる
// ステート自体も毎フレーム更新

// 時間経過で減少する(空腹度など)
// 何かしらのアクションで減少する(疲労など

// 各パラメータの自然遷移はどこで行うか
// 
// 知能のレイヤー:遷移のメッセージを受け取る
// 身体のレイヤー:メッセージが実行可能か調べる

// 状態と評価値は紐づいていなくても大丈夫？
// 寝ている状態でスタート、食欲が高い状態 ならすぐ起きる

// どうやって遷移させる？
// メッセージの受信 もしくは 黒板の参照？

// 一定間隔で知能が一番高い評価値を返す
// 評価値を対応する動作に変更 <- ここまでが知能の層
// その動作が身体が出来るかどうかチェック <- ここから身体の層
// 