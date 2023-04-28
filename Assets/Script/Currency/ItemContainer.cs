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
        //Solo con fines de prueba
        LoadSystem.AddPostLoadCorutine(() =>
            {
                character.AddOrSubstractItems(initialItems.nameDisplay, 20);
                character.AddOrSubstractItems(initialItems.nameDisplay, 30);
            }
        
        );
    }
}