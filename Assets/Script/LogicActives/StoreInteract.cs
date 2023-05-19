using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    //ParcheFeo--------------------------------------
    public DisplayMaterials[] materialsPrefab;

    public TextMeshProUGUI inventarioEmergencia;
    //-----------------------------------------------

    private void Awake()
    {
        store = GetComponent<StaticEntity>();
        //obtenes la instancia del character

        instance = this;
    }

    protected override void InternalActivate(params Character[] specificParam)
    {
        MenuManager.instance.ShowWindow("Store");
        //aca se configuraria

        //ejecuta las funciones de configuracion

        for (int i = 0; i < recipes.Count; i++)
        {
            //Se que esta horrible, sepa disculpar--------------------------------------------------------------------------
            Manager<DetailsWindow>.pic["Store"].CreateStoreButton(recipes[i].result.Item.nameDisplay, "Store");

            if(i==0)
            {
                var aux = Manager<DetailsWindow>.pic["Store"].buttonsGrid.GetChild(0).GetComponent<UnityEngine.UI.Button>();
                aux.onClick.Invoke();
            }
            //----------------------------------------------------------------------------------------------------------------

        }
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

    public void RefreshMaterials(Recipes recipe)
    {
        for (int i = 0; i < recipe.materials.Count; i++)
        {
            materialsPrefab[i].sprite = recipe.materials[i].Item.image;
            materialsPrefab[i].amount.text = character.ItemCount(recipe.materials[i].Item.nameDisplay) + " / " + recipe.materials[i].Amount;
        }
    }

    public void RefreshInventory()
    {
        inventarioEmergencia.text = string.Join("", character.inventory);
    }

}

public struct DisplayMaterials
{
    public Sprite sprite;
    public TextMeshProUGUI amount;
}