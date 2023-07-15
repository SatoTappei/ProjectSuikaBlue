using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        // 課題:攻撃された場合はどうするのか
        //  状態の遷移条件&体力に関係するのでFSM側で検知する必要がある
        //  アニメーションの切り替えどうする？

        // 発見状態
        //  n秒間隔で召喚
        //      タイマー
        //      アクション <- アニメーションする
        //  m秒間隔で周囲を回復
        //      タイマー
        //      アクション <- アニメーションする
        //  プレイヤーが攻撃したら回避(後ろにジャンプのアニメーションをする)
        //      召喚/回復をしている場合は回避しない
        // 未発見状態
        //  別の木が担当
    }
}
