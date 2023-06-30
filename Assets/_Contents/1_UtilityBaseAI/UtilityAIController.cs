using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class UtilityAIController : MonoBehaviour
{
    /// <summary>
    /// 最適な評価値を選択する間隔
    /// </summary>
    const float UtilityUpdateInterval = 0.1f;

    [SerializeField] UtilityParams _paramControlModule;

    UtilityStateBase _currentState;
    //UtilityStateAndParamLinker _linker;

    void Awake()
    {
        UtilitySateSleep stateSleep = new();
        UtilityStateEat stateEat = new();
        UtilityStateWork stateWork = new();

        _currentState = stateSleep;

        // 最適な評価値の取得
        Observable.Interval(System.TimeSpan.FromSeconds(UtilityUpdateInterval)).Subscribe(_ => 
        {
            // TODO:選択だけしてそれをどうする処理を書いていない
            _paramControlModule.SelectNext();
        });

        // 評価値の自然減少＆現在の状態の更新
        this.UpdateAsObservable().Subscribe(_ => 
        {
            _paramControlModule.Update();
            _currentState.Update();
        });
    }

    void Start()
    {
        
    }
}

// 一番近い箇所まで行って作業をする
// 休憩する(食事)
// 家に戻る

// 評価値の更新
// 一定間隔で評価値を基にステートの遷移をさせる
// ステート自体も毎フレーム更新

// 時間経過で減少する(空腹度など)
// 何かしらのアクションで減少する(疲労など