using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializerScriptObject : MonoBehaviour
{
    // Start is called before the first frame update

    public TMPro.TextMeshProUGUI textMesh;

    public bool EnabledDebug;

    void Awake()
    {
        string path = "ScriptableObject";
        var aux = LoadSystem.LoadAssets(path);

        EnabledDebug = enabled ? EnabledDebug : false;

        if (!EnabledDebug)
            return;

        textMesh.text += "Se cargaron los assets: \n";

        foreach (var item in aux)
        {
            textMesh.text +="\t"  +item.name +" ";
        }
        textMesh.text += "\n";
    }

    private void Start()
    {
        if (!EnabledDebug)
            return;

        textMesh.text += "el itembase de items contiene: " + Manager<ItemBase>.pic.Count;
        textMesh.text += "\nel showmanager de items contiene: " + Manager<ShowDetails>.pic.Count;
    }
}
