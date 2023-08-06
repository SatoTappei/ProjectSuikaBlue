using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataBinding
{
    /// <summary>
    /// �Q�[���̐ݒ�N���X�ŁA�ǂݍ���ł���f�[�^�̐ݒ��ێ��B
    /// �J��/���[�J��/�����[�X��3�̃��[�h������B
    /// </summary>
    public class GameSetting
    {
        public enum Mode
        {
            Develop,
            // Local <- ���[�J�����p
            // Release <- �����[�X�p
        }

        static GameSetting _instance = new();
        DataLoadSetting _dataLoadSetting;
        Mode _mode = Mode.Develop;

        public static string MasterDataURI => GetConfig().MasterDataURI;
        public static string MasterDataSheetName => GetConfig().MasterDataSheetName;

        static DataLoadSetting GetConfig()
        {
            if (_instance._dataLoadSetting != null) return _instance._dataLoadSetting;

            // �ŏ���1��̂݁A�������ĕԂ�
            // ���̂т��p�����Ă��Ȃ��V���O���g���Ȃ̂�Resources�t�@�C�����烍�[�h���Ă���
            string name = string.Format("{0}", _instance._mode.ToString());
            _instance._dataLoadSetting = Resources.Load<DataLoadSetting>(name);
            return _instance._dataLoadSetting;
        }
    }
}