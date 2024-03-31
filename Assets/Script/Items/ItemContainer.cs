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

        character.inventory.AddOrSubstractItems("Real Dash", 1);
        ((AbilityExtCast)character.inventory.inventory[character.inventory.inventory.Count - 1]).Init(character.inventory);
    }

}