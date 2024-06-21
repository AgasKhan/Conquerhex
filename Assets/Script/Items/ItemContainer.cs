using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [SerializeField]
    public Entity character;

    ItemsGiver myItemsGiver;

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

        myItemsGiver = GetComponent<ItemsGiver>();
        //myItemsGiver?.Activate(character);
        LoadSystem.AddPostLoadCorutine(() => myItemsGiver?.Activate(character), 10);

        /*
        LoadSystem.AddPostLoadCorutine(() =>
        {
            myItemsGiver = GetComponent<ItemsGiver>();
            myItemsGiver?.Activate(character);

            Debug.Log("POST LOAD");
        });*/
    }
}