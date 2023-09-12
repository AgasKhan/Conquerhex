using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [SerializeField]
    public Character character;

    void Awake()
    { 
        
        if (SaveWithJSON.CheckKeyInBD("Player"))
        {
            SaveWithJSON.LoadClassFromPictionary("Player", ref character);
            Debug.Log("BD contains Player");
        }
        else
        {
            Debug.Log("BD doesnt contain Player");
        }
        

        /*
        SaveWithJSON.OnSave += () => SaveWithJSON.SaveParams(character.GetType(), character.inventory, character.currentWeight);
        LoadParams();
        */
    }
    void LoadParams()
    {
        if (!SaveWithJSON.CheckKeyInBD(character.GetType().ToString()))
            return;
        
        character.inventory = (List<Item>)SaveWithJSON.LoadParams(character.GetType(), character.inventory);
        character.currentWeight = (float)SaveWithJSON.LoadParams(character.GetType(), character.currentWeight);
    }
}