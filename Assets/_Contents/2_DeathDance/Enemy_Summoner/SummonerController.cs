using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// 召喚してくる敵を制御するクラス
/// 最初からステージに配置されるのではなく、ゲーム中に生成されることを考慮した作りになっている
/// </summary>
[RequireComponent(typeof(CommonLayerBlackBoard))]
public class SummonerController : MonoBehaviour
{
    [SerializeField] GameObject _spawnPrefab;

    void Awake()
    {
        CommonLayerBlackBoard commonLayerBlackBoard = GetComponent<CommonLayerBlackBoard>();
        // 初期状態を割り当てる
        EnemyStateBase currentState = commonLayerBlackBoard[EnemyStateType.Init];
        this.UpdateAsObservable().Subscribe(_ =>
        {
            currentState = currentState.Update();
        });
    }

    void Start()
    {

    }

    void Update()
    {
        
    }
}

// 待機:未発見
// 戦闘:発見、敵を召喚する
// 回避:プレイヤーが一定距離以下かつ攻撃のメッセージを受信したタイミング

// 敵の発見などはステートで行う