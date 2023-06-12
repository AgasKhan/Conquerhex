using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingBuild : BuildingBase
{
    public Pictionarys<int, List<Recipes>> levelRecipes = new Pictionarys<int, List<Recipes>>();

    public List<Recipes> recipes = new List<Recipes>();

    protected override void InternalAction()
    {
        base.InternalAction();

        buttonsFuncs.AddRange(new Pictionarys<string, System.Action>()
        {
            {"Open", Internal}
        });
    }

    void Internal()
    {
        for (int i = 0; i < recipes.Count; i++)
        {
            //Manager<DetailsWindow>.pic["CraftingMenu"].CreateStoreButton(recipes[i].result.Item.nameDisplay, "CraftingMenu");

            Debug.Log("Entro al for");

            if (i == 0)
            {
                //var aux = Manager<DetailsWindow>.pic["CraftingMenu"].buttonsGrid.GetChild(0).GetComponent<UnityEngine.UI.Button>();
                //aux.onClick.Invoke();
            }

        }
    }

    bool myBool = true;
    public void ShowCraftingW()
    {
        //ejecuta las funciones de configuracion

        if (myBool)
        {
            for (int i = 0; i < recipes.Count; i++)
            {
                //MenuManager.instance.detailsWindows["CraftingMenu"].CreateStoreButton(recipes[i].result.Item.nameDisplay, "CraftingMenu");


                if (i == 0)
                {
                    //var aux = Manager<DetailsWindow>.pic["CraftingMenu"].buttonsGrid.GetChild(0).GetComponentInChildren<UnityEngine.UI.Button>();
                    //aux.onClick?.Invoke();
                }

                //----------------------------------------------------------------------------------------------------------------
            }

            myBool = false;
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
            //materialsPrefab[i].sprite = recipe.materials[i].Item.image;
            //materialsPrefab[i].amount.text = character.ItemCount(recipe.materials[i].Item.nameDisplay) + " / " + recipe.materials[i].Amount;
        }
    }

    public void RefreshInventory()
    {
        //inventarioEmergencia.text = string.Join("", character.inventory);
    }


    public override void UpgradeLevel()
    {
        base.UpgradeLevel();

        for (int i = 0; i <levelRecipes[currentLevel].Count; i++)
        {
            recipes.Add(levelRecipes[currentLevel][i]);
        }
    }

    //---------------------------------------------------------------------

    



}
