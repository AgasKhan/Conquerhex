using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [SerializeField]
    public Character character;

    void Awake()
    { 

        character.AddOrSubstractItems("PortalFuel", 100);
        if (SaveWithJSON.CheckKeyInBD("Player"))
        {
            SaveWithJSON.LoadClassFromPictionary("Player", ref character);
            Debug.Log("BD contains Player");
        }
        else
        {
            Debug.Log("BD doesnt contain Player");
        }
    }


    private void OnDestroy()
    {
        SaveWithJSON.SaveClassInPictionary("Player", character);
    }
}