using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailsWindowsManager : MonoBehaviour
{
    public static DetailsWindowsManager instance;
    
    public Pictionarys<string, DetailsWindow> detailsWindows = new Pictionarys<string, DetailsWindow>();

    private void Awake()
    {
        instance = this;
    }
   
}
