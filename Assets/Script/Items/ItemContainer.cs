using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [SerializeField]
    public Entity character;

    void Awake()
    {
        /*
        if (SaveWithJSON.CheckKeyInBD("Player"))
        {
            SaveWithJSON.LoadClassFromPictionary("Player", ref character);
            Debug.Log("BD contains Player");
        }
        else
        {
            Debug.Log("BD doesnt contain Player");
        }

        LoadSystem.AddPostLoadCorutine(()=> GetComponent<ItemsGiver>()?.Activate(character));
        */

        LoadSystem.AddPostLoadCorutine(() => GetComponent<ItemsGiver>()?.Activate(character));
    }

}