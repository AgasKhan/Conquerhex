using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CraftingSubMenu : CreateSubMenu
{
    CraftingBuild buildingBase;

    List<ButtonA> buttonsList = new List<ButtonA>();

    EventsCall lastButtonCraft;

    DetailsWindow myDetailsW;

    public event System.Action onCraft;

    public override void Create()
    {
        subMenu = MenuManager.instance.modulesMenu.ObtainMenu<SubMenus>();
        base.Create();
    }
    protected override void InternalCreate()
    {
        subMenu.navbar.DestroyAll();

        if (buildingBase.NavBarButtons.Length > 1)
        {
            subMenu.AddNavBarButton("All", ButtonAct);
            foreach (var item in buildingBase.NavBarButtons)
            {
                subMenu.AddNavBarButton(item.ToString(), () => { ButtonAct(item.ToString()); });
            }
        }

        subMenu.CreateTitle(buildingBase.flyweight.nameDisplay);

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
    }

    public void CreateButtons()
    {
        buttonsList.Clear();

        foreach (var item in buildingBase.currentRecipes)
        {
            ButtonA button = subMenu.AddComponent<ButtonA>();

            UnityEngine.Events.UnityAction action =
                () =>
                {
                    RefreshDetailW(item);
                };

            buttonsList.Add(button.SetButtonA(item.result.Item.nameDisplay, item.result.Item.image,"", action).SetType(item.result.Item.itemType.ToString()));
        }
    }
    void DestroyButtonCraft()
    {
        if (lastButtonCraft != null)
            Object.Destroy(lastButtonCraft.gameObject);
    }

    public bool CraftAnItem(string recipeName) //(Character customer, string recipeName)
    {
        if (buildingBase.character == null)
            return false;

        bool aux = true;

        Recipes recipe = null;

        foreach (var item in buildingBase.currentRecipes)
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

        if (recipe.CanCraft(buildingBase.character.inventory))
        {
            recipe.Craft(buildingBase.character.inventory);
            RefreshDetailW(recipe);
            return true;
        }
        else
            return false;

    }

    void RefreshDetailW(Recipes item)
    {
        ShowResultDetails(item.result.Item.nameDisplay, item.result.Item.GetDetails().ToString() + "Materiales necesarios: \n" + item.GetRequiresString(buildingBase.character.inventory), item.result.Item.image);

        lastButtonCraft = subMenu.AddComponent<EventsCall>().Set("Crear", () => 
        {
            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(true).SetWindow("", "¿Seguro deseas crear este item?")
                .AddButton("Si", () => { ButtonAction(item); MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false); })
                .AddButton("No", () => MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false));

        }, "");

        lastButtonCraft.button.interactable = item.CanCraft(buildingBase.character.inventory);
    }

    void ButtonAction(Recipes item)
    {
        CraftAnItem(item.nameDisplay); 
        onCraft?.Invoke();
    }

    void ShowResultDetails(string nameDisplay, string details, Sprite Image)
    {
        DestroyButtonCraft();
        myDetailsW.SetTexts(nameDisplay, details).SetImage(Image);
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

        myDetailsW.SetTexts("", "").SetImage(null);
        DestroyButtonCraft();
    }

    void ButtonAct()
    {
        ButtonAct("");
    }

    public CraftingSubMenu(CraftingBuild _buildingBase)
    {
        buildingBase = _buildingBase;
    }
}