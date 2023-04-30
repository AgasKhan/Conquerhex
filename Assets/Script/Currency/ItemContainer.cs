using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [SerializeField]
    public StaticEntity character;


    //Solo con fines de prueba
    [SerializeField]
    Resources_Item initialItems;

    private void Awake()
    {
        initialItems.Init();
    }


    private void Start()
    {
        /*
        //Solo con fines de prueba
        LoadSystem.AddPostLoadCorutine(() =>
            {
                character.AddOrSubstractItems(initialItems.nameDisplay, 20);
                character.AddOrSubstractItems(initialItems.nameDisplay, 30);
            }
        
        );
        */
        
        
    }
    private void OnEnable()
    {
        if (SaveWithJSON.CheckKeyInBD("PlayerInventory"))
        {
            Debug.Log("El inventario cargo del BD:  -------------------------------");
            Debug.Log(string.Join("", character.inventory));
            Debug.Log("-------------------------------------------------------------");

            character.inventory = SaveWithJSON.LoadFromPictionary<List<Item>>("PlayerInventory");
        }
    }

    private void OnDisable()
    {
        //Debug.Log("Inventario en Hexagonos: \n" + string.Join("", character.inventory));

        //var aux = new AuxClass<DropItem[]>(character.drops.ToArray());

        //aux.value = ;

        //character.drops

        //Debug.Log(aux.value[0]);

        //Debug.Log(JsonUtility.ToJson(aux));

        SaveWithJSON.SaveInPictionary("PlayerInventory", character.inventory);
    }

    private void OnDestroy()
    {
        
    }
}