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

    EventsCall lastButton = null;

    public override void Create(Character character)
    {
        myCharacter = character;
        subMenu = MenuManager.instance.modulesMenu.ObtainMenu<SubMenus>();

        subMenu.navbar.DestroyAll();
        subMenu.ClearBody();
        DestroyLastButtons();
        base.Create();
    }
    protected override void InternalCreate()
    {
        createAction?.Invoke(subMenu);
    }

    public EventsCall CreateButton(string text, UnityEngine.Events.UnityAction action)
    {
        DestroyLastButtons();
        lastButton = subMenu.AddComponent<EventsCall>().Set(text, action, "");
        return lastButton;
    }

    public void DestroyLastButtons()
    {
        if (lastButton != null)
            Object.Destroy(lastButton.gameObject);
    }

    public void SetCreateAct(System.Action<SubMenus> _createAction)
    {
        createAction = _createAction;
    }

    public string SetTextforItem(Item item)
    {
        string details = "";

        if (item is MeleeWeapon)
        {
            details = "Uses: " + ((MeleeWeapon)item).current;
        }
        else
        {
            item.GetAmounts(out int actual, out int max);
            details = actual + " / " + max;
        }

        return details;
    }

    public GenericSubMenu(InteractEntityComponent _entity)
    {
        interactComponent = _entity;
    }
}