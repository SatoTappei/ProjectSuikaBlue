using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 召喚してくる敵を制御するクラス
/// </summary>
public class SummonerController : MonoBehaviour
{
    [SerializeField] GameObject _spawnPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

// 待機:未発見
// 戦闘:発見、敵を召喚する
// 回避:プレイヤーが一定距離以下かつ攻撃のメッセージを受信したタイミング

// 敵の発見などはステートで行う