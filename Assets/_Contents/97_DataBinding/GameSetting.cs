using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataBinding
{
    /// <summary>
    /// ゲームの設定クラスで、読み込んでくるデータの設定を保持。
    /// 開発/ローカル/リリースの3つのモードがある。
    /// </summary>
    public class GameSetting
    {
        public enum Mode
        {
            Develop,
            // Local <- ローカル環境用
            // Release <- リリース用
        }

        static GameSetting _instance = new();
        DataLoadSetting _dataLoadSetting;
        Mode _mode = Mode.Develop;

        public static string MasterDataURI => GetConfig().MasterDataURI;
        public static string MasterDataSheetName => GetConfig().MasterDataSheetName;

        static DataLoadSetting GetConfig()
        {
            if (_instance._dataLoadSetting != null) return _instance._dataLoadSetting;

            // 最初の1回のみ、生成して返す
            // ものびを継承していないシングルトンなのでResourcesファイルからロードしてくる
            string name = string.Format("{0}", _instance._mode.ToString());
            _instance._dataLoadSetting = Resources.Load<DataLoadSetting>(name);
            return _instance._dataLoadSetting;
        }
    }
}