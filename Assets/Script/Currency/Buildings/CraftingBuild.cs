using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingBuild : BuildingBase
{
    public Pictionarys<int, List<Recipes>> levelRecipes = new Pictionarys<int, List<Recipes>>();

    CraftingSubMenu craftSubMenu;

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

public class CraftingSubMenu : CreateSubMenu
{
    public Character character;

    public ScrollVertComponent itemList;

    List<ButtonA> buttonsList = new List<ButtonA>();

    List<EventsCall> buttonsListActions = new List<EventsCall>();

    DetailsWindow myDetailsW;

    DetailsWindow myRequirements;

    public List<Recipes> recipes;

    protected override void InternalCreate()
    {
        subMenu.navbar.DestroyAll();

        subMenu.AddNavBarButton("Weapons", ButtonAct).AddNavBarButton("Modules", () => { ButtonAct(ItemType.Equipment.ToString()); });

        subMenu.CreateTitle("Crafting");

        CreateBody();

    }

    void CreateBody()
    {
        subMenu.ClearBody();

        subMenu.CreateSection(0, 3);
        subMenu.CreateChildrenSection<ScrollRect>();

        CreateButtons();

        subMenu.CreateSection(3, 6);
        myDetailsW = subMenu.AddComponent<DetailsWindow>();
        myRequirements = subMenu.AddComponent<DetailsWindow>();
        subMenu.AddComponent<EventsCall>();
    }

    public void CreateButtons()
    {
        buttonsList.Clear();

        foreach (var item in recipes)
        {
            ButtonA button = subMenu.AddComponent<ButtonA>();

            UnityEngine.Events.UnityAction action =
                () =>
                {
                    ShowItemDetails(item.result.Item.nameDisplay, item.result.Item.GetDetails().ToString(), item.result.Item.image);
                    DestroyButtonsActions();
                    //CreateButtonsActions(item.GetItemBase().buttonsAcctions);
                };

            //buttonsList.Add(button.SetButtonA(item.nameDisplay, item.image, SetTextforItem(item), action).SetType(item.itemType.ToString()));
        }
    }
    void DestroyButtonsActions()
    {

        foreach (var item in buttonsListActions)
        {
            if (item != null)
                Object.Destroy(item.gameObject);
        }

        buttonsListActions.Clear();
    }

    void CreateButtonsActions(Dictionary<string, System.Action<Character>> dic)
    {
        foreach (var item in dic)
        {
            buttonsListActions.Add(subMenu.AddComponent<EventsCall>().Set(item.Key,
                () => {
                    item.Value(character);
                    CreateBody();
                }, ""));

            buttonsListActions[buttonsListActions.Count - 1].rectTransform.sizeDelta = new Vector2(300, 75);

            //subMenu.AddComponent<EventsCall>().Set(item.Key, () => item.Value(character), "");
        }
    }


    void ShowItemDetails(string nameDisplay, string details, Sprite Image)
    {
        myDetailsW.SetTexts(nameDisplay, details).SetImage(Image);

        subMenu.RetardedOn(myDetailsW.gameObject);
    }

    void ButtonAct(string type)
    {
        foreach (var item in buttonsList)
        {
            if (item.type != type && type != "")
                item.SetActiveGameObject(false);
            else
                item.SetActiveGameObject(true);
        }

        myDetailsW.SetActiveGameObject(false);
        DestroyButtonsActions();
    }

    void ButtonAct()
    {
        ButtonAct("");
    }

    string SetTextforItem(Item item)
    {
        string details = "";

        if (item.itemType == ItemType.Equipment && item is MeleeWeapon)
        {
            details = "Uses: " + ((MeleeWeapon)item).durability.current;

        }
        else
        {
            item.GetAmounts(out int actual, out int max);
            details = actual + " / " + max;

        }

        return details;
    }

}
