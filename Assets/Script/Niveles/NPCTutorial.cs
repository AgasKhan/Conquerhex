using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTutorial : MonoBehaviour, Interactuable
{
    [SerializeField]
    Sprite myImage;

    public Sprite Image => myImage;

    
    [SerializeField]
    bool myEnable;

    bool Interactuable.interactuable { get => myEnable; set => myEnable = value; }
    
    public void Interact(Character character)
    {
        TutorialScenaryManager.instance.NextDialog();
    }
    
}
