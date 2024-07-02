using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{   
    public Pictionarys<string, AudioLink> audios = new Pictionarys<string, AudioLink>();

    public void AddAudio(string key, AudioLink audioLink)
    {
        Internal.Pictionary<string, AudioLink> pic;

        if (!audios.ContainsKey(key, out int index))
        {
            pic = audios.Add(key, audioLink);
            pic.value.Init(gameObject);
        }
        else
        {
            pic = audios.GetPic(index);
            var aux = pic.value.source;
            pic.value = audioLink;
            pic.value.source = aux;
        }
    }

    public void Play(string name)
    {
        audios[name].source.Play();
    }

    public void Stop(string name)
    {
        audios[name].source.Stop();
    }

    public void Pause(string name)
    {
        audios[name].source.Pause();
    }

    // Start is called before the first frame update
    void Awake()
    {
        foreach (var item in audios)
        {
            item.value.Init(gameObject);
        }
    }   
}


[System.Serializable]
public struct AudioLink : ISerializationCallbackReceiver
{
    [HideInInspector]
    public AudioSource source;
    public AudioMixerGroup mixer;
    public AudioClip clip;

    [Header("multiplicara el volumen")]
    [Range(0, 1)]
    public float volume;

    [Header("Velocidad de reproduccion del clip")]
    [Range(-1, 3)]
    public float pitch;

    [Header("parte 3d")]
    [Range(0, 1)]
    public float spatialBlend;
    [Range(1, 100)]
    public float maxDistance;
    [Range(1, 100)]
    public float minDistance;

    [Header("configuracion")]
    public bool loop;
    public bool onAwake;

    public void OnAfterDeserialize()
    {
    }

    public void OnBeforeSerialize()
    {
        if (minDistance == 0)
            minDistance = 1;

        if (volume == 0)
            volume = 1;

        if (pitch == 0)
            pitch = 1;

        if (source == null)
            return;

        source.outputAudioMixerGroup = mixer;
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = loop;
        source.playOnAwake = onAwake;
        source.maxDistance = maxDistance;
        source.minDistance = minDistance;
        source.spatialBlend = spatialBlend;
    }

    /// <summary>
    /// Initialezer of audio clip
    /// </summary>
    /// <param name="param">necesita el gameobecjt de quien tendria el audio</param>
    public void Init(GameObject go)
    {
        source = go.AddComponent<AudioSource>();
        OnBeforeSerialize();
    }
}
