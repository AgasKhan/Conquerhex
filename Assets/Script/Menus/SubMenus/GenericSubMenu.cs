using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GenericSubMenu : CreateSubMenu
{
    [HideInInspector]
    public DetailsWindow detailsWindow;

    [HideInInspector]
    public Character myCharacter;

    [HideInInspector]
    public InteractEntityComponent interactComponent;

    public System.Action<SubMenus> createAction;

    public List<ButtonHor> buttonsList = new List<ButtonHor>();

    EventsCall lastButton = null;

    public void InteractAction()
    {
        subMenu.CreateSection(0, 2);
        subMenu.CreateChildrenSection<UnityEngine.UI.ScrollRect>();

        foreach (var item in interactComponent.interact)
        {
            item.value.InteractInit(interactComponent);
            subMenu.AddComponent<EventsCall>().Set(item.key.Name.Replace("Action", "") , () => { DestroyLastButtons(); item.value.ShowMenu(myCharacter); GoToOtherMenu(); }, "").rectTransform.sizeDelta = new Vector2(300, 75);
        }

        subMenu.CreateSection(2, 6);
        subMenu.CreateChildrenSection<UnityEngine.UI.ScrollRect>();
        detailsWindow = subMenu.AddComponent<DetailsWindow>().SetTexts("", interactComponent.container.flyweight.GetDetails()["Descripción"]).SetImage(interactComponent.container.flyweight.image);

        subMenu.CreateTitle(interactComponent.container.flyweight.nameDisplay);
    }


    public override void Create(Character character)
    {
        myCharacter = character;

        //Debug.Log("El character que interactuo es: " + myCharacter.gameObject.name);
        subMenu = MenuManager.instance.modulesMenu.ObtainMenu<SubMenus>();

        subMenu.navbar.DestroyAll();
        subMenu.ClearBody();
        DestroyLastButtons();
        base.Create();
    }
    protected override void InternalCreate()
    {
        createAction?.Invoke(subMenu);
        subMenu.OnClose += Exit;
    }

    public EventsCall CreateButton(string text, UnityEngine.Events.UnityAction action)
    {
        DestroyLastButtons();
        lastButton = subMenu.AddComponent<EventsCall>().Set(text, action, "");
        return lastButton;
    }

    public void ShowItemDetails(string nameDisplay, string details, Sprite Image)
    {
        detailsWindow.SetTexts(nameDisplay, details).SetImage(Image);
        detailsWindow.SetActiveGameObject(true);
    }
    /*
    public void CreateButtonA(string name, Sprite image, string textNum)
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

            buttonsList.Add(button.SetButtonA(item.nameDisplay, item.image, "", action).SetType(item.GetType().ToString()));
        }
    }
    */
    public void DestroyLastButtons()
    {
        if (lastButton != null)
            Object.Destroy(lastButton.gameObject);
    }

    public void SetCreateAct(System.Action<SubMenus> _createAction)
    {
        createAction = _createAction;
    }

    void Exit()
    {
        subMenu.ExitSubmenu();
    }

    public void GoToOtherMenu()
    {
        subMenu.OnClose -= Exit;
        subMenu.OnClose += SubMenuOnClose;
    }
    void SubMenuOnClose()
    {
        subMenu.OnClose -= SubMenuOnClose;
        Create(myCharacter);
    }

    public GenericSubMenu(InteractEntityComponent _entity)
    {
        interactComponent = _entity;
    }
}