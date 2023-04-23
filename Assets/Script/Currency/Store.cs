using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : StaticEntity
{
    [SerializeField]
    ItemContainer customer;

    public static Store instance;

    protected override void Config()
    {
        MyAwakes += MyAwake;
    }

    void MyAwake()
    {
        instance = this;
    }

    public void BuyAnItem(string recipeName) //(ItemContainer customer, string recipeName)
    {
        if (customer != null && Manager<Recipes>.pic.ContainsKey(recipeName))
        {
            if (Manager<Recipes>.pic[recipeName].CanCraft(customer.character))
            {
                Manager<Recipes>.pic[recipeName].Craft(customer.character);
            }
            else
            {
                Debug.Log("No tienes los recursos suficientes");
            }
        }
        else
            Debug.Log("No se encontro la receta: " + recipeName);
    }
}
