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
        // 同時再生出来る最大数
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
            // AudioSourceをたくさん追加
            for(int i = 0; i < _sources.Length; i++)
            {
                _sources[i] = gameObject.AddComponent<AudioSource>();
            }

            // 音データを辞書に追加
            _dataDict = _audioData.ToDictionary(v => v.Key, v => v);
        }

        void SubscribeMessage()
        {
            // 音再生のメッセージを受信したら音を再生する
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
                Debug.LogWarning("再生する音が辞書に登録されていない: " + msg.Key);
            }
        }

        AudioSource GetFree() => _sources.Where(v => !v.isPlaying).FirstOrDefault();
        AudioSource GetPlaying() => _sources.Where(v => v.isPlaying).FirstOrDefault();

        /// <summary>
        /// 指定したSE/BGMを再生
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
        /// 指定したSE/BGMを停止
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
        /// 外部からこのメソッドを呼ぶことで音を再生する
        /// </summary>
        public static void PlayAudio(AudioKey key)
        {
            MessageBroker.Default.Publish(new PlayAudioMessage() { Key = key });
        }
    }
}