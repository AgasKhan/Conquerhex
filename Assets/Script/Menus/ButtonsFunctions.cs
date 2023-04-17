using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsFunctions : MonoBehaviour
{

    private void Start()
    {
        MenuManager.instance.eventListVoid.AddRange(new Pictionarys<string, System.Action<GameObject>>()
        {

            // Static Buttons
            {"Button", Example},
            {"Start", Start},
            {"Options", Options},
            {"Credits", Credits},
            {"Exit", Exit},

            {"MBody", MBody},
            {"MArms", MArms},
            {"MLegs", MLegs},
            {"MHeads", MHeads},
            {"MTails", MTails},
            {"QUIT", QUIT},

            {"ShowMod", ShowMod},
            
            //Dragable Buttons
            {"ShowWindow", ShowWindow}

        });
    }

    string ReadDetails(List<string> list)
    {
        var str = "";

        foreach (var item in list)
        {
            str += item + "\n";
        }

        return str;
    }


    void ShowMod(GameObject g)
    {
        var data = g.GetComponent<ButtonInformation>();

        var aux = data.myDetails;

        //DoubleString text = new DoubleString(aux.nameDisplay, ReadDetails(aux.details));

        DoubleString text = new DoubleString(aux.nameDisplay, aux.details.ToString());

        data.myDetailWindow.SetWindow(aux.image, text);
    }

    #region Static Buttons
    void Example(GameObject g)
    {
        Debug.Log("Apretaste el boton");
    }

    void Start(GameObject g)
    {

    }

    void Options(GameObject g)
    {

    }

    void Credits(GameObject g)
    {
        //var data = g.GetComponent<ButtonInformation>().buttonData;

        //data.detailWindow.SetWindow(data.sprite, data.text);
    }

    void Exit(GameObject g)
    {
        Application.Quit();
    }



    void MBody(GameObject g)
    {
        
    }
    void MArms(GameObject g)
    {

    }
    void MLegs(GameObject g)
    {

    }
    void MHeads(GameObject g)
    {

    }
    void MTails(GameObject g)
    {

    }
    void QUIT(GameObject g)
    {

    }


    #endregion

    #region Drageable Buttons
    void ShowWindow(GameObject g)
    {
        Debug.Log("Apretaste un boton Draggeable");
        //DetailsWindow.instance.SetWindow(_mySprite, _information);
    }

    #endregion

    #region FuncionesAntiguas
    /*
    // Llamada cada que cambia el slider
    void CameraSpeed(GameObject g, float f)
    {
        // Almacenar 
        //CSVReader.SaveInPictionary("MouseSensibility", f);
        
        print("Todo funciono: " + f);
        if (player == null)
            return;


        player.cameraScript.Speed(f);
    }

    // On
    
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
    */

    #endregion
}
