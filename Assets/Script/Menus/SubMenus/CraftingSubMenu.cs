using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CraftingSubMenu : CreateSubMenu
{
    public CraftingBuild buildingBase;
    
    List<ButtonA> buttonsList = new List<ButtonA>();

    EventsCall lastButtonCraft;

    DetailsWindow myDetailsW;

    //public List<Recipes> recipes;

    protected override void InternalCreate()
    {
        subMenu.navbar.DestroyAll();

        if(buildingBase.NavBarButtons.Length > 1)
        {
            subMenu.AddNavBarButton("All", ButtonAct);
            foreach (var item in buildingBase.NavBarButtons)
            {
                subMenu.AddNavBarButton(item.ToString(), () => { ButtonAct(item.ToString()); });
            }
        }

        subMenu.CreateTitle(buildingBase.structureBase.nameDisplay);

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

        foreach (var item in buildingBase.recipes)
        {
            ButtonA button = subMenu.AddComponent<ButtonA>();

            UnityEngine.Events.UnityAction action =
                () =>
                {
                    ShowResultDetails(item.result.Item.nameDisplay, item.result.Item.GetDetails().ToString() + "Materiales necesarios: \n" + item.GetRequiresString(), item.result.Item.image);
                    DestroyButtonCraft();
                    lastButtonCraft = subMenu.AddComponent<EventsCall>().Set("Crear", () => { CraftAnItem(item.nameDisplay); }, "");

                };

            buttonsList.Add(button.SetButtonA(item.result.Item.nameDisplay, item.result.Item.image, "", action).SetType(item.result.Item.itemType.ToString()));
        }
    }
    void DestroyButtonCraft()
    {
        if(lastButtonCraft!=null)
            Object.Destroy(lastButtonCraft.gameObject);
    }

    public bool CraftAnItem(string recipeName) //(Character customer, string recipeName)
    {
        if (buildingBase.character == null)
            return false;

        bool aux = true;

        Recipes recipe = null;

        foreach (var item in buildingBase.recipes)
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

        if (recipe.CanCraft(buildingBase.character))
        {
            recipe.Craft(buildingBase.character);
            return true;
        }
        else
            return false;

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

        myDetailsW.SetActiveGameObject(false);
        DestroyButtonCraft();
    }

    void ButtonAct()
    {
        ButtonAct("");
    }
}