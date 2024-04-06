using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavingAction : InteractAction<(Character character, int slot)>
{
    public BaseData baseData;
    public override void Activate((Character character, int slot) specificParam)
    {
        var customer = specificParam.character;

        baseData.SaveGame(specificParam.slot);
    }

    public override void InteractInit(InteractEntityComponent _interactComp)
    {
        base.InteractInit(_interactComp);
        subMenu = new SaveSubMenu(this);
    }
}

[System.Serializable]
public class SaveSubMenu : CreateSubMenu
{
    SavingAction saveAction;

    [HideInInspector]
    public DetailsWindow detailsWindow;

    EventsCall lastButton = null;

    Character myCharacter;

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
        subMenu.CreateSection(0, 3);
        subMenu.CreateChildrenSection<ScrollRect>();

        foreach (var item in saveAction.baseData.savedGames)
        {
            subMenu.AddComponent<EventsCall>().Set("Save game in slot " + item.key, () => { DestroyLastButtons(); ShowSlot(item.key); }, "").rectTransform.sizeDelta = new Vector2(350, 75);
        }

        subMenu.CreateSection(3, 6);
        subMenu.CreateChildrenSection<ScrollRect>();
        detailsWindow = subMenu.AddComponent<DetailsWindow>().SetTexts("", "").SetImage(null);

        subMenu.CreateTitle("Save Building");
    }

    public EventsCall CreateButton(string text, UnityEngine.Events.UnityAction action)
    {
        DestroyLastButtons();
        lastButton = subMenu.AddComponent<EventsCall>().Set(text, action, "");
        return lastButton;
    }

    public void ShowSlot(int slot)
    {
        detailsWindow.SetTexts("Game slot: " + slot, "Last modification date: ").SetImage(null);
        CreateButton("Save Game", ()=> saveAction.Activate((myCharacter, slot)));
    }

    public void DestroyLastButtons()
    {
        if (lastButton != null)
            Object.Destroy(lastButton.gameObject);
    }

    public SaveSubMenu(SavingAction _entity)
    {
        saveAction = _entity;
    }
}