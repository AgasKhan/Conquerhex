using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CraftingSubMenu : CreateSubMenu
{
    CraftingAction craftAction;

    List<ButtonA> buttonsList = new List<ButtonA>();

    EventsCall lastButtonCraft;

    DetailsWindow myDetailsW;

    public event System.Action onCraft;

    Character myCharacter;

    public override void Create(Character character)
    {
        myCharacter = character;
        subMenu = MenuManager.instance.modulesMenu.ObtainMenu<SubMenus>();
        base.Create();
    }
    protected override void InternalCreate()
    {
        subMenu.navbar.DestroyAll();

        /*
        if (craftAction.NavBarButtons.Length > 1)
        {
            subMenu.AddNavBarButton("All", ButtonAct);
            foreach (var item in craftAction.NavBarButtons)
            {
                subMenu.AddNavBarButton(item.ToString(), () => { ButtonAct(item.ToString()); });
            }
        }
        */

        subMenu.CreateTitle(craftAction.interactComp.container.flyweight.nameDisplay);

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

        foreach (var item in ((CraftingBuild)craftAction.interactComp.container).currentRecipes)
        {
            ButtonA button = subMenu.AddComponent<ButtonA>();

            UnityEngine.Events.UnityAction action =
                () =>
                {
                    RefreshDetailW(item);
                };

            buttonsList.Add(button.SetButtonA(item.nameDisplay, item.image,"", action).SetType(item.GetType().ToString()));
        }
    }
    void DestroyButtonCraft()
    {
        if (lastButtonCraft != null)
            Object.Destroy(lastButtonCraft.gameObject);
    }

    public void RefreshDetailW(MeleeWeaponBase item)
    {
        ShowResultDetails(item.nameDisplay, item.GetDetails().ToString() + "Materiales necesarios: \n" + item.recipe.GetRequiresString(myCharacter.inventory), item.image);

        lastButtonCraft = subMenu.AddComponent<EventsCall>().Set("Crear", () => 
        {
            MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(true).SetWindow("", "�Seguro deseas crear este item?")
                .AddButton("Si", () => { ButtonAction(item); MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false); })
                .AddButton("No", () => MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false));

        }, "");

        lastButtonCraft.button.interactable = item.recipe.CanCraft(myCharacter.inventory);
    }

    void ButtonAction(MeleeWeaponBase item)
    {
        craftAction.Activate((myCharacter, item));
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

    public CraftingSubMenu(CraftingAction _craftAction)
    {
        craftAction = _craftAction;
    }
}