using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum AudioKey
{
    SeBlood,
    SeExplode,
    SeFire,
}

public class AudioModule : MonoBehaviour
{
    [System.Serializable]
    class AudioData
    {
        [SerializeField] AudioKey _key;
        [SerializeField] AudioSource[] _sources;

        public AudioKey Key => _key;

        public AudioSource GetFree() => _sources.Where(v => !v.isPlaying).FirstOrDefault();
        public AudioSource GetPlaying() => _sources.Where(v => v.isPlaying).FirstOrDefault();
    }

    [SerializeField] AudioData[] _audioData;
    Dictionary<AudioKey, AudioData> _dataDict;

    void Awake()
    {
        _dataDict = _audioData.ToDictionary(v => v.Key, v => v);
    }

    /// <summary>
    /// éwíËÇµÇΩSE/BGMÇçƒê∂
    /// </summary>
    public void Play(AudioKey key)
    {
        AudioSource source = _dataDict[key].GetFree();
        if (source != null) source.Play();
    }

    /// <summary>
    /// éwíËÇµÇΩSE/BGMÇí‚é~
    /// </summary>
    public void Stop(AudioKey key)
    {
        AudioSource source = _dataDict[key].GetPlaying();
        if (source != null) source.Stop();
    }
}
