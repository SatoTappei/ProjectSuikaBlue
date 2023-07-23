using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using MilitaryHierarchy;

public class MilitaryGeneral : MonoBehaviour
{
    // ある程度の部隊が敗北すると全部隊撤退する
    // ドラゴンを撃破すると全部隊撤退する
    // 部隊の半分以上がやられるとその部隊は撤退する

    [SerializeField] GameObject _tankLeaderPrefab;
    [SerializeField] GameObject _tankPrefab;
    [SerializeField] Transform[] _spawnPoints;
    [SerializeField] Transform[] _escapePoints;
    [Header("部隊に命令するために使用する")]
    [SerializeField] Transform _destination;

    /// <summary>
    /// 部隊の沸きポイントが利用可能かチェックするための配列
    /// 初期値はtrueで利用可能になっている
    /// </summary>
    bool[] _spawnPointAvailable;
    /// <summary>
    /// 0始まりの部隊番号で、生成を行う度に1ずつ増えていく
    /// </summary>
    int _troopNum;

    void Awake()
    {
        // 0始まりで指定する想定で初期化
        _spawnPointAvailable = new bool[_spawnPoints.Length];
        System.Array.Fill(_spawnPointAvailable, true);
    }

    void Start()
    {
        // 部隊を生成して部隊長に移動先の命令をする
        SpawnTroop(_troopNum);
        MessageBroker.Default.Publish(new DestinationMessage()
        {
            Destination = _destination.position,
            TroopNum = _troopNum,
        });

        _troopNum++;
    }

    /// <summary>
    /// 部隊長1台 兵士3もしくは4体、ランダムな箇所に部隊を生成
    /// 生成した部隊のユニットには部隊番号を付与する
    /// </summary>
    void SpawnTroop(int troopNum)
    {
        // 部隊長をランダムな沸きポイントに生成
        Vector3 spawnPos = _spawnPoints[GetEmptySpawnPointIndex()].position;
        GameObject tankLeader = Instantiate(_tankLeaderPrefab, spawnPos, Quaternion.identity);
        tankLeader.GetComponent<TankLeader>().TroopNum = troopNum;

        // 周囲八近傍の何処かに配置
        float mag = 3.0f;
        List<Vector3> neighbours = new()
        {
            spawnPos + Vector3.forward * mag,
            spawnPos + Vector3.forward * mag + Vector3.right * mag,
            spawnPos + Vector3.right * mag,
            spawnPos + Vector3.back * mag + Vector3.right * mag,
            spawnPos + Vector3.back * mag,
            spawnPos + Vector3.back * mag + Vector3.left * mag,
            spawnPos + Vector3.left * mag,
            spawnPos + Vector3.forward * mag + Vector3.left * mag,
        };

        // 3もしくは4体生成
        int spawnQuantity = Random.Range(3, 5);
        for(int i= 0; i < spawnQuantity; i++)
        {
            int neighbourIndex = Random.Range(0, neighbours.Count);
            GameObject tank = Instantiate(_tankPrefab, neighbours[neighbourIndex], Quaternion.identity);
            tank.GetComponent<Tank>().TroopNum = troopNum;
            neighbours.RemoveAt(neighbourIndex);
        }
    }

    /// <summary>
    /// ランダムな空いた沸きポイントを返す
    /// </summary>
    int GetEmptySpawnPointIndex()
    {
        for(int i = 0; i < 100; i++)
        {
            int index = Random.Range(0, _spawnPointAvailable.Length);
            if (_spawnPointAvailable[index])
            {
                return index;
            }
        }

        throw new System.InvalidOperationException("ランダムな数が偏りすぎている");
    }
}
