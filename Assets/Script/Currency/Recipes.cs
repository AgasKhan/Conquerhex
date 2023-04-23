using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Currency/Recipe", fileName = "Recipe")]
public class Recipes : ItemBase
{
    public List<Ingredient> materials;

    public List<Ingredient> results;

    public bool CanCraft(IItemContainer container)
    {
        foreach (var ingredient in materials)
        {
            if (container.ItemCount(ingredient.Item.nameDisplay) < ingredient.Amount)
                return false;
        }
        return true;
    }

    public void Craft(IItemContainer container)
    {
        if(CanCraft(container))
        {
            foreach (var ingredient in materials)
            {
                container.AddOrSubstractItems(ingredient.Item.nameDisplay, - ingredient.Amount);
            }

            foreach (var ingredient in results)
            {
                container.AddOrSubstractItems(ingredient.Item.nameDisplay, ingredient.Amount);
            }

            foreach (var ingredient in materials)
            {
                Debug.Log("Despues del crafteo el jugador tiene: " + container.ItemCount(ingredient.Item.nameDisplay).ToString() + " "  + ingredient.Item.nameDisplay);
            }

        }
    }

    protected override void SetCreateItemType()
    {
        
    }

    protected override void MyEnable()
    {
        base.MyEnable();
        Manager<Recipes>.pic.Add(nameDisplay, this);
    }
}

[Serializable]
public struct Ingredient
{
    [SerializeField]
    public ItemBase Item;

    [Range(1, 50)]
    public int Amount;
}