using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreInteract : LogicActive<Character>
{
    StaticEntity store;

    [SerializeField]
    Character character;

    public static StoreInteract storeInteract;

    public List<Recipes> recipes = new List<Recipes>();

    private void Awake()
    {
        store = GetComponent<StaticEntity>();
        //obtenes la instancia del character

        storeInteract = this;
    }

    protected override void InternalActivate(params Character[] specificParam)
    {
        MenuManager.instance.ShowWindow("");
        //aca se configuraria

        //ejecuta las funciones de configuracion
    }

    public override void Activate()
    {
        Activate(character, store);
    }

    public bool BuyAnItem(string recipeName, Character customer) //(ItemContainer customer, string recipeName)
    {
        if (customer == null)
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

        if (recipe.CanCraft(customer))
        {
            recipe.Craft(customer);
            recipes.Remove(recipe);
            return true;
        }
        else
            return false;

    }
}