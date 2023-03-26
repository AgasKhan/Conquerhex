using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class ButtonsManager : MonoBehaviour
{
    public AudioMixerGroup group;

    //Player_Character player;

    MenuManager menu;

    //
    // Start is called before the first frame update
    void Awake()
    {
        //player = GameManager.player;
        menu = MenuManager.instance;
        /*
        menu.eventListFloat.AddRange(new Pictionarys<string, System.Action<GameObject, float>>()
        {
            {"sens", CameraSpeed},
            {"AlphaReticula", ChangeAlphaReticula},
            {"Master", ChangeVolumeLevel},
            {"Ambiental", ChangeVolumeLevel},
            {"Effects", ChangeVolumeLevel}
        });

        menu.eventListSliderOn.AddRange(new Pictionarys<string, System.Action<Slider>>()
        {
            {"sens", CameraSpeed},
            {"AlphaReticula", OnAlphaReticula},
            {"Master", LoadVolumeLevel},
            {"Ambiental", LoadVolumeLevel},
            {"Effects", LoadVolumeLevel}
        });

        menu.eventListVoid.AddRange(new Pictionarys<string, System.Action<GameObject>>()
        {

            {"Mute", Mute}

        });*/
    }

    // Llamada cada que cambia el slider
    void CameraSpeed(GameObject g, float f)
    {
        // Almacenar 
        //CSVReader.SaveInPictionary("MouseSensibility", f);
        /*
        print("Todo funciono: " + f);
        if (player == null)
            return;


        player.cameraScript.Speed(f);*/
    }

    // On
    /*
    void CameraSpeed(Slider s)
    {
        s.value = CSVReader.LoadFromPictionary<float>("MouseSensibility", 25);
    }

    void ChangeAlphaReticula(GameObject g, float f)
    {
        CSVReader.SaveInPictionary("AlphaReticula", f);
        MainHud.ReticulaAlpha(f);
    }

    void OnAlphaReticula(Slider s)
    {
        s.value = CSVReader.LoadFromPictionary<float>("AlphaReticula", 0.15f);
    }
    */

    void ChangeVolumeLevel(GameObject g, float volume)
    {
        ChangeVolume(volume, g.name);
    }

    float LoadVolume(string name)
    {
        float volume;
        group.audioMixer.GetFloat(name, out volume);
        return Mathf.Pow(10, (volume / 20));
    }


    void LoadVolumeLevel(Slider s)
    {
        s.value = LoadVolume(s.name);
    }

    void ChangeVolume(float volume, string name)
    {
        if (volume == 0)
            volume = 0.0001f;
        var value = Mathf.Log10(volume) * 20;
        group.audioMixer.SetFloat(name, value);
    }

    void Mute(GameObject g)
    {
        //menu.eventListFloat["Master"](0.01f);
        if (ChangeText(g))
            ChangeVolume(1f, "Master");
        else
            ChangeVolume(0, "Master");

    }

    //////////////////////////////////
    ///hacer herencia con configuration
    ///
    bool ChangeText(GameObject g)
    {
        TMPro.TextMeshProUGUI text = g.GetComponentInChildren<TMPro.TextMeshProUGUI>();

        //text.text = (text.text == "Activate") ? "Deactivate" : "Activate";

        if ((text.text == "Activate"))
        {
            text.text = "Deactivate";
            return false;
        }
        else
        {
            text.text = "Activate";
            return true;
        }

    }

    void ChangeText(GameObject g, bool active)
    {
        TMPro.TextMeshProUGUI text = g.GetComponentInChildren<TMPro.TextMeshProUGUI>();

        text.text = active ? "Deactivate" : "Activate";
    }

}
