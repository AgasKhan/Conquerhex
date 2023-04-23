using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [SerializeField]
    public StaticEntity character;


    //Solo con fines de prueba
    [SerializeField]
    Resources_Item characterItem;

    private void Awake()
    {
        characterItem.Init();
    }

    private void Start()
    {
        //Solo con fines de prueba
        LoadSystem.AddPostLoadCorutine((Action)(() =>
            {
                character.AddOrSubstractItems((string)characterItem.nameDisplay, (int)30);
                character.AddOrSubstractItems((string)characterItem.nameDisplay, (int)30);
            })
        
        );
    }
}