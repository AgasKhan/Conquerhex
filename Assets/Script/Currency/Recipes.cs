using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Currency/Recipe", fileName = "Recipe")]
public class Recipes : ItemBase
{
    public List<Ingredient> materials;

    public Ingredient result;

    public bool CanCraft(IItemContainer container)
    {
        foreach (var ingredient in materials)
        {
            if (container.ItemCount(ingredient.Item.nameDisplay) < ingredient.Amount)
            {
                Debug.Log("No posees los items necesarios para el crafteo");
                return false;
            }
        }
        return true;
    }

    public void Craft(IItemContainer container)
    {
        foreach (var ingredient in materials)
        {
            container.AddOrSubstractItems(ingredient.Item.nameDisplay, -ingredient.Amount);
        }

        container.AddOrSubstractItems(result.Item.nameDisplay, result.Amount);

        foreach (var ingredient in materials)
        {
            Debug.Log("Despues del crafteo el jugador tiene: " + container.ItemCount(ingredient.Item.nameDisplay).ToString() + " " + ingredient.Item.nameDisplay);
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