using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Currency/Recipe", fileName = "Recipe")]
public class Recipes : ItemBase
{
    public List<Ingredient> materials;
    /*
    //public Ingredient result;

    [Range(1,20)]
    public int resultAmount = 1;

    public Color resultColor;

    public bool CanCraft(InventoryEntityComponent container)
    {
        foreach (var ingredient in materials)
        {
            if (container.ItemCount(ingredient.Item) < ingredient.Amount)
            {
                Debug.Log("No posees los items necesarios para el crafteo");
                return false;
            }
        }
        return true;
    }

    public void Craft(InventoryEntityComponent container, string resultName = "")
    {
        foreach (var ingredient in materials)
        {
            if(ingredient.Item.maxAmount > 1)
                container.SubstracItems(ingredient.Item, ingredient.Amount);
        }

        if (resultName != "")
        {
            container.AddItem(this, resultAmount);
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

    public string GetRequiresString(InventoryEntityComponent container)
    {
        string aux = "";

        for (int i = 0; i < materials.Count; i++)
        {
            var itemCount = container.ItemCount(materials[i].Item);
            aux  += materials[i].Item.nameDisplay + " "+ (itemCount <= 0 ? 0 : itemCount) + " / "+ materials[i].Amount + "\n";
        }
        
        return aux.RichText("color", "#ffa500ff");
    }

    public string GetIngredientsStr()
    {
        string aux = "Materials: \n";

        for (int i = 0; i < materials.Count; i++)
        {
            aux += materials[i].Item.nameDisplay + " " + materials[i].Amount + "\n";
        }

        return aux.RichText("color", "#ffa500ff");
    }

    public string GetIngredientsStr(float porcentual)
    {
        string aux = "Materials: \n";

        for (int i = 0; i < materials.Count; i++)
        {
            aux += GetMaterials(materials[i],porcentual);
        }

        return aux.RichText("color", "#ffa500ff");
    }
    string GetMaterials(Ingredient ing, float porcentual)
    {
        var aux = Mathf.RoundToInt(ing.Amount * porcentual);
        if (aux == 0)
            return "";
        else
            return ing.Item.nameDisplay + " " + aux + "\n";
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

    public override System.Type GetItemType()
    {
        return null;
    }

    protected override void MyEnable()
    {
        base.MyEnable();
        Manager<Recipes>.pic.Add(nameDisplay, this);
    }*/
    public override System.Type GetItemType()
    {
        return null;
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