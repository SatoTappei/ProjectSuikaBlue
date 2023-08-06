using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

namespace DataBinding
{
    [System.Serializable]
    public class ActorData
    {
        public string Name;
        public int Rare;
        public int Power;
    }

    public class Logic : MonoBehaviour
    {
        void Start()
        {
            Setup().Forget();
        }

        async UniTask<int> Setup(UnityAction callback = null)
        {   
            // ★:今までは.Instance.~で処理を呼んでいたが、Instance自体のメンバを指定して
            //    返すstaticなプロパティを用意することでインスタンス自体にはアクセスをしないでも呼んでいる。
            string sheetURI = GameSetting.MasterDataURI;
            string sheetName = GameSetting.MasterDataSheetName;

            Debug.Log("マスターデータのロード開始");
            
            string uri = string.Format("{0}?sheet={1}", sheetURI, sheetName);
            string result = await WebRequest.GetRequest(uri);

            Debug.Log("Webリクエスト完了");
            Debug.Log("取得したデータ\n" + result);

            //ActorData actorData = JsonUtility.FromJson<ActorData>(result);

            return 0;
        }
    }
}
