using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Store : StaticEntity
{
    [SerializeField]
    ItemContainer customer;

    [SerializeField]
    TextMeshProUGUI coinCounter;

    public static Store instance;

    protected override void Config()
    {
        MyAwakes += MyAwake;
    }

    void MyAwake()
    {
        instance = this;
    }

    public void RefreshPlayerCoins()
    {
        coinCounter.text = customer.character.ItemCount("Coin").ToString();
    }


    public bool BuyAnItem(string recipeName) //(ItemContainer customer, string recipeName)
    {
        if (customer != null && Manager<Recipes>.pic.ContainsKey(recipeName))
        {
            if (Manager<Recipes>.pic[recipeName].CanCraft(customer.character))
            {
                Manager<Recipes>.pic[recipeName].Craft(customer.character);
                Manager<Recipes>.pic.Remove(recipeName);
                return true;
            }
            else
                return false;
        }
        else
        {
            Debug.Log("No se encontro la receta: " + recipeName);
            return false;
        }
            
    }

    //----------------------------------------------------
    public void ClearCustomerInventory()
    {
        customer.character.inventory.Clear();
    }
    //----------------------------------------------------
}
