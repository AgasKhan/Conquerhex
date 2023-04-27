using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializerScriptObject : MonoBehaviour
{
    // Start is called before the first frame update

    public TMPro.TextMeshProUGUI textMesh;
    
    void Awake()
    {
        string path = "ScriptableObject";
        var aux = LoadSystem.LoadAsset(path);

        textMesh.text += "Se cargaron los assets: \n";

        foreach (var item in aux)
        {
            textMesh.text +="\t"  +item.name +" ";
        }
        textMesh.text += "\n";
    }

    private void Start()
    {
        textMesh.text += "el itembase de items contiene: " + Manager<ItemBase>.pic.Count;
        textMesh.text += "\nel showmanager de items contiene: " + Manager<ShowDetails>.pic.Count;
    }
}
