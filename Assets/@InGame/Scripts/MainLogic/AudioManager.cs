using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;

namespace PSB.InGame
{
    public enum AudioKey
    {
        BreedingSE,
        BreedingMutationSE,
        ClickSE,
    }

    public class AudioManager : MonoBehaviour
    {
        // �����Đ��o����ő吔
        const int Max = 10;

        [System.Serializable]
        class AudioData
        {
            [SerializeField] AudioKey _key;
            [SerializeField] AudioClip _clip;
            [Range(0, 1)]
            [SerializeField] float _volume = 1;

            public AudioKey Key => _key;
            public AudioClip Clip => _clip;
            public float Volume => _volume;
        }

        [SerializeField] AudioData[] _audioData;

        Dictionary<AudioKey, AudioData> _dataDict;
        AudioSource[] _sources = new AudioSource[Max];

        void Awake()
        {
            Init();
            SubscribeMessage();
        }

        void Init()
        {
            // AudioSource����������ǉ�
            for(int i = 0; i < _sources.Length; i++)
            {
                _sources[i] = gameObject.AddComponent<AudioSource>();
            }

            // ���f�[�^�������ɒǉ�
            _dataDict = _audioData.ToDictionary(v => v.Key, v => v);
        }

        void SubscribeMessage()
        {
            // ���Đ��̃��b�Z�[�W����M�����特���Đ�����
            MessageBroker.Default.Receive<PlayAudioMessage>().Subscribe(OnReceiveMessage).AddTo(this);
        }

        void OnReceiveMessage(PlayAudioMessage msg)
        {
            if (_dataDict.ContainsKey(msg.Key))
            {
                Play(msg.Key);
            }
            else
            {
                Debug.LogWarning("�Đ����鉹�������ɓo�^����Ă��Ȃ�: " + msg.Key);
            }
        }

        AudioSource GetFree() => _sources.Where(v => !v.isPlaying).FirstOrDefault();
        AudioSource GetPlaying() => _sources.Where(v => v.isPlaying).FirstOrDefault();

        /// <summary>
        /// �w�肵��SE/BGM���Đ�
        /// </summary>
        void Play(AudioKey key)
        {
            AudioSource source = GetFree();
            if (source != null)
            {
                source.clip = _dataDict[key].Clip;
                source.volume = _dataDict[key].Volume;
                source.Play();
            }
        }

        /// <summary>
        /// �w�肵��SE/BGM���~
        /// </summary>
        void Stop(AudioKey key)
        {
            AudioSource source = GetPlaying();
            if (source != null)
            {
                source.Stop();
            }
        }

        /// <summary>
        /// �O�����炱�̃��\�b�h���ĂԂ��Ƃŉ����Đ�����
        /// </summary>
        public static void PlayAudio(AudioKey key)
        {
            MessageBroker.Default.Publish(new PlayAudioMessage() { Key = key });
        }
    }
}