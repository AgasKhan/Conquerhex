using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [SerializeField]
    public Character character;

    private void Start()
    {
        //character = GetComponent<Character>();

        //LoadSystem.AddPostLoadCorutine(Charge);
    }

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

    }

    private void OnDisable()
    {
        SaveWithJSON.SaveInPictionary("PlayerInventory", character.inventory);
    }


}