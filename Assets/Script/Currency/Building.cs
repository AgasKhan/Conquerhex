using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Building : StaticEntity
{
    /*
    [SerializeField]
    ItemContainer customer;

    [SerializeField]
    TextMeshProUGUI coinCounter;
    */

    [SerializeField]
    StructureBase structureBase;
    protected override Damage[] vulnerabilities => structureBase.vulnerabilities;

    protected override void Config()
    {
        base.Config();

        MyOnEnables += MyOnEnable;
    }

    private void MyOnEnable()
    {
        
    }


    /*
    protected override void Config()
    {
        MyAwakes += MyAwake;
    }

    public void RefreshPlayerCoins()
    {
        coinCounter.text = customer.character.ItemCount("Coin").ToString();
    }

        //----------------------------------------------------
    public void ClearCustomerInventory()
    {
        character.inventory.Clear();
    }
    //----------------------------------------------------
    */




}
