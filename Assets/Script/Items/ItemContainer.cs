using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [SerializeField]
    public Character character;

    void Awake()
    {
        if (SaveWithJSON.CheckKeyInBD("PlayerInventory"))
        {
            character.inventory = SaveWithJSON.LoadFromPictionary<List<Item>>("PlayerInventory");
            Debug.Log("BD contains PlayerInventory");
        }
        else
        {
            Debug.Log("BD doesnt contain PlayerInventory");
        }

        for (int i = 0; i < 3; i++)
        {
            character.weaponKataIndex = i;

            if (SaveWithJSON.CheckKeyInBD("PlayerKata" + i))
            {
                character.actualKata.character = character;

                character.actualKata.indexEquipedItem = SaveWithJSON.LoadFromPictionary<int>("PlayerKata" + i);

                if(character.actualKata.indexEquipedItem!=-1)
                    character.actualKata.equiped.Init(character);
            }
            else
            {
                Debug.Log("BD doesnt contain PlayerInventory");
            }
        }

        character.AddOrSubstractItems("PortalFuel", 100);
    }

    private void OnDisable()
    {
        SaveWithJSON.SaveInPictionary("PlayerInventory", character.inventory);

        for (int i = 0; i < 3; i++)
        {
            character.weaponKataIndex = i;

            SaveWithJSON.SaveInPictionary("PlayerKata" + i, character.actualKata.indexEquipedItem);
        }
    }
}