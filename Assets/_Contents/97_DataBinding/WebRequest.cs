using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace DataBinding
{
    public class WebRequest
    {
        const int InitWorkerCapacity = 5;

        static WebRequest _instance = new();
        List<RequestWorker> _workerList = new(InitWorkerCapacity);

        static void AddWorkerInstance()
        {
            // 最初に初期容量の分だけインスタンスを追加しておく？
            if (_instance._workerList.Count == 0)
            {
                for (int i = 0; i < InitWorkerCapacity; i++)
                {
                    _instance._workerList.Add(new());
                }
            }

            // 全部使用済みなら1つ追加する？
            IEnumerable<RequestWorker> processing = _instance._workerList.Where(v => !v.IsProcessing);
            if (processing.Count() == 0)
            {
                _instance._workerList.Add(new());
            }
        }

        static RequestWorker GetWorker()
        {
            RequestWorker worker = null;
            IEnumerable<RequestWorker> processing = _instance._workerList.Where(v => !v.IsProcessing);
            // 全部使用済みなら1つ追加する？
            if (processing.Count() == 0)
            {
                worker = new();
                _instance._workerList.Add(worker);
            }
            else
            {
                worker = processing.First();
            }
            return worker;
        }

        static public async UniTask<string> GetRequest(string uri)
        {
            AddWorkerInstance();
            RequestWorker worker = GetWorker();
            return await worker.GetRequest(uri);
        }
    }
}
