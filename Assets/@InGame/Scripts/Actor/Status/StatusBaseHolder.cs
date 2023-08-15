using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace PSB.InGame
{
    public class StatusBaseHolder
    {
        // シングルトン
        static StatusBaseHolder _instance = new();

        const string KinpatsuAddress       = "Assets/@InGame/Scripts/Actor/Status/Status_Kinpatsu.asset";
        const string KinpatsuLeaderAddress = "Assets/@InGame/Scripts/Actor/Status/Status_KinpatsuLeader.asset";
        const string KurokamiAddress       = "Assets/@InGame/Scripts/Actor/Status/Status_Kurokami.asset";

        // 3つだけなのでベタ書き
        StatusBase _kinpatsuBase;
        StatusBase _kinpatsuLeaderBase;
        StatusBase _kurokamiBase;

        List<AsyncOperationHandle<StatusBase>> _handleList = new();

        /// <summary>
        /// StatusBaseを取得する前に必ずこのメソッドを1度呼んでロードすること
        /// </summary>
        /// <returns>使用するStatusBaseをロードするUniTask</returns>
        public static async UniTask LoadAsync(CancellationToken token)
        {
            UniTask<StatusBase> load1 = LoadAsync(KinpatsuAddress, token);
            UniTask<StatusBase> load2 = LoadAsync(KinpatsuLeaderAddress, token);
            UniTask<StatusBase> load3 = LoadAsync(KurokamiAddress, token);

            var assets = await UniTask.WhenAll(load1, load2, load3);
            if (assets.Item1 == null || assets.Item2 == null || assets.Item3 == null)
            {
                throw new System.NullReferenceException("StatusBaseのロードに失敗");
            }

            _instance._kinpatsuBase = assets.Item1;
            _instance._kinpatsuLeaderBase = assets.Item2;
            _instance._kurokamiBase = assets.Item3;
        }

        static async UniTask<StatusBase> LoadAsync(string address, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            AsyncOperationHandle<StatusBase> handle = Addressables.LoadAssetAsync<StatusBase>(address);
            _instance._handleList.Add(handle);
            
            return await handle;
        }

        public static StatusBase Get(ActorType type)
        {
            if      (type == ActorType.Kinpatsu)       return _instance._kinpatsuBase;
            else if (type == ActorType.KinpatsuLeader) return _instance._kinpatsuLeaderBase;
            else if (type == ActorType.Kurokami)       return _instance._kurokamiBase;
            else
            {
                throw new System.ArgumentException("対応するStatusBaseが無い: " + type);
            }
        }

        /// <summary>
        /// シーンの最後にこのメソッドを呼んで開放すること
        /// </summary>
        public static void Release()
        {
            foreach (AsyncOperationHandle<StatusBase> handle in _instance._handleList)
            {
                Addressables.Release(handle);
            }

            _instance._handleList.Clear();
        }
    }
}