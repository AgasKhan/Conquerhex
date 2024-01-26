using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Currency/Recipe", fileName = "Recipe")]
public class Recipes : ItemBase
{
    public List<Ingredient> materials;

    public Ingredient result;

    public Color resultColor;

    public bool CanCraft(StaticEntity container)
    {
        foreach (var ingredient in materials)
        {
            if (container.ItemCount(ingredient.Item.nameDisplay) < ingredient.Amount)
            {
                Debug.Log("No posees los items necesarios para el crafteo");
                return false;
            }

            /*
            if (container.weightCapacity < result.Item.weight)
            {
                Debug.Log("Espacio insuficiente para el crafteo");
                return false;
            }
            */
        }
        return true;
    }

    public void Craft(StaticEntity container)
    {
        foreach (var ingredient in materials)
        {
            container.AddOrSubstractItems(ingredient.Item.nameDisplay, - ingredient.Amount);
        }

        if(result.Item != null)
        {
            //result.Item.image.color = resultColor;
            container.AddOrSubstractItems(result.Item.nameDisplay, result.Amount);

        }

        foreach (var ingredient in materials)
        {
            Debug.Log("Despues del crafteo el jugador tiene: " + container.ItemCount(ingredient.Item.nameDisplay).ToString() + " " + ingredient.Item.nameDisplay);
        }
    }

    public List<string> GetRequiresList()
    {
        List<string> aux = new List<string>();

        for (int i = 0; i < materials.Count; i++)
        {
            aux.Add(materials[i].Item.nameDisplay + " " + materials[i].Amount);
        }
        return aux;
    }

    public string GetRequiresString(StaticEntity container)
    {
        string aux = "";

        for (int i = 0; i < materials.Count; i++)
        {
            aux  += materials[i].Item.nameDisplay + " "+ container.ItemCount(materials[i].Item.nameDisplay) + " / "+ materials[i].Amount + "\n";
        }
        
        return aux.RichText("color", "#ffa500ff");
    }
    public Pictionarys<string, Sprite> GetRequireItems()
    {
        Pictionarys<string, Sprite> aux = new Pictionarys<string, Sprite>();

        for (int i = 0; i < materials.Count; i++)
        {
            aux.Add(materials[i].Item.nameDisplay + " " + materials[i].Amount, materials[i].Item.image);
        }

        return aux;
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

    public Ingredient(ItemBase item, int amount)
    {
        Item = item;
        Amount = amount;
    }
}