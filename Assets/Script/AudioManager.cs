using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

    public static AudioManager instance;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (PlayerPrefs.GetFloat("Ambiente")==0)
        {

            PlayerPrefs.SetFloat("Ambiente", 0.7f);
            PlayerPrefs.SetFloat("Efecto", 0.5f);
            PlayerPrefs.SetFloat("Menu", 0.01f);
        }

        foreach (var item in sounds)
        {
            CreateAuSc(item, gameObject);
        }
    }
    public Sound Srch(string name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == name)
            {
                
                return sounds[i];
            }
        }

        DebugPrint.Warning("Sonido no encontrado: " + name);
        return null;
    }

    public Sound Vol(string name, float v)
    {
        Sound s = Srch(name);

        s.source.volume = v;

        return s;
    }

    public Sound Play(string name)
    {
        Sound s = Srch(name);

        s.source.Play();

        return s;        
    }

    public void UpdateSound()
    {
        foreach (var item in sounds)
        {
            if(item.source!=null)
            switch (item.type)
            {
                case Type.ambiental:
                    item.source.volume = PlayerPrefs.GetFloat("Ambiente");
                    
                    break;

                case Type.efecto:
                    item.source.volume = PlayerPrefs.GetFloat("Efecto");
                    
                    break;

                case Type.menu:
                    item.source.volume = PlayerPrefs.GetFloat("Menu") / 10;
                    
                    break;
            }
        }
    }

    public AudioSource CreateAuSc(Sound item, GameObject Objective )
    {

        item.source = Objective.AddComponent<AudioSource>();

        item.source.clip = item.clip;

        item.source.pitch = item.pitch;

        item.source.playOnAwake = false;

        switch (item.type)
        {

            case Type.ambiental:
                item.source.volume = PlayerPrefs.GetFloat("Ambiente");

                DebugPrint.Log("Correcto Ambiental " + item.source.volume);
                break;

            case Type.efecto:
                item.source.volume = PlayerPrefs.GetFloat("Efecto");

                DebugPrint.Log("Correcto Efecto " + item.source.volume);
                break;

            case Type.menu:
                item.source.volume = PlayerPrefs.GetFloat("Menu") / 10;

                DebugPrint.Log("Correcto Menu " + item.source.volume);
                break;

        }
        return item.source;
    }
}

public enum Type
{
    ambiental = 0,
    efecto = 1,
    menu = 2    
}

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    //[Range(0f, 1f)]
    public float volume;

    [Range(0.1f, 3f)]
    public float pitch;

    public Type type = Type.ambiental;

    public AudioSource source;

    
}
