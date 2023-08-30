using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace PSB.InGame
{
    public class GameManager : MonoBehaviour
    {
        //[SerializeField] 

        //void Start()
        //{
        //    ExecuteAsync(this.GetCancellationTokenOnDestroy()).Forget();
        //}

        //async UniTaskVoid ExecuteAsync(CancellationToken token)
        //{
        //    // キャラクターのステータス読み込み
        //    await StatusBaseHolder.LoadAsync(token);
        //    // フィールドの生成
        //    FieldManager.Instance.Create();
        //    // 初期金髪を配置
        //    InitKinpatsuSpawner kinpatsu = GetComponent<InitKinpatsuSpawner>();
        //    //kinpatsu.Spawn(field);
        //}

        //void OnDestroy()
        //{
        //    StatusBaseHolder.Release();
        //}

        //void M()
        //{
        //    uint gene = 0b_0000_0000_0000_0000_0000_1000_0000_0000;
        //    // 0~255
        //    float f = gene & 0xFF;
        //    // fを0.75から1.5の範囲にリマップ
        //    float mappedSize = (f - 0) * (1.5f - 0.75f) / (255 - 0) + 0.75f;
        //    Debug.Log(mappedSize);
        //}

        ///// <summary>
        ///// ビットから特定の範囲を切り出して遺伝子として扱う
        ///// 論理ビットシフト(シフトした部分は0)を使用するのでuint型
        ///// </summary>
        //void Gene()
        //{
        //    // 1.任意の数(今回は8)の倍数で右にビットシフトする。32ビットなので4つに分けられる。
        //    // 2.ビットマスクを行う、今回は8ビットなので256である0xFFとのビット積をとる。

        //    //             カラーR   カラーG   カラーB   サイズ
        //    uint gene = 0b_0000_0000_0000_0000_0000_1000_1000_1010;
        //    Debug.Log("カラーR: " + ((gene >> 24) & 0xFF));
        //    Debug.Log("カラーG: " + ((gene >> 16) & 0xFF));
        //    Debug.Log("カラーB: " + ((gene >> 8)  & 0xFF));
        //    Debug.Log("サイズ: "  + (gene         & 0xFF));
        //}
    }
}
