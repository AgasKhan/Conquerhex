using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreInteract : LogicActive<Character>
{
    StaticEntity store;

    [SerializeField]
    Character character;

    public List<Recipes> recipes = new List<Recipes>();


    //Parche-----------------------------------------
    public static StoreInteract instance;

    public override void Activate()
    {
        Activate(character, store);
    }
    //-----------------------------------------------

    private void Awake()
    {
        store = GetComponent<StaticEntity>();
        //obtenes la instancia del character

        instance = this;
    }

    protected override void InternalActivate(params Character[] specificParam)
    {
        MenuManager.instance.ShowWindow("");
        //aca se configuraria

        //ejecuta las funciones de configuracion
    }


    public bool BuyAnItem(string recipeName) //(Character customer, string recipeName)
    {
        if (character == null)
            return false;

        bool aux = true;

        Recipes recipe = null;

        foreach (var item in recipes)
        {
            if (recipeName == item.name)
            {
                aux = false;
                recipe = item;
                break;
            }
        }

        if (aux)
        {
            Debug.Log("No se encontro la receta: " + recipeName);
            return false;
        }

        if (recipe.CanCraft(character))
        {
            recipe.Craft(character);
            recipes.Remove(recipe);
            return true;
        }
        else
            return false;

    }

    public void ClearCustomerInventory()
    {
        character.inventory.Clear();
    }


}