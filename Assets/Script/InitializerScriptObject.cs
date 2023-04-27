using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializerScriptObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        string path = "ScriptableObject";
        LoadSystem.LoadAsset(path);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
