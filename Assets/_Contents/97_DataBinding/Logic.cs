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
            // ��:���܂ł�.Instance.~�ŏ������Ă�ł������AInstance���̂̃����o���w�肵��
            //    �Ԃ�static�ȃv���p�e�B��p�ӂ��邱�ƂŃC���X�^���X���̂ɂ̓A�N�Z�X�����Ȃ��ł��Ă�ł���B
            string sheetURI = GameSetting.MasterDataURI;
            string sheetName = GameSetting.MasterDataSheetName;

            Debug.Log("�}�X�^�[�f�[�^�̃��[�h�J�n");
            
            string uri = string.Format("{0}?sheet={1}", sheetURI, sheetName);
            string result = await WebRequest.GetRequest(uri);

            Debug.Log("Web���N�G�X�g����");
            Debug.Log("�擾�����f�[�^\n" + result);

            //ActorData actorData = JsonUtility.FromJson<ActorData>(result);

            return 0;
        }
    }
}
