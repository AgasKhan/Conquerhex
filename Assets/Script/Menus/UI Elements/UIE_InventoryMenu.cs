using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;

public class UIE_InventoryMenu : UIE_EquipMenu
{
    protected List<UIE_ListButton> buttonsList = new List<UIE_ListButton>();

    VisualElement equipmentButton;
    VisualElement combosButton;

    protected override void Config()
    {
        base.Config();

        MyAwakes += myAwake;
    }

    private void myAwake()
    {
        if (gameObject.name == manager.InventoryMenu)
        {
            onEnableMenu += myOnEnable;

            equipTitle = ui.Q<Label>("equipTitle");
            listContainer = ui.Q<VisualElement>("listContainer");
            titleDW = ui.Q<Label>("titleDW");
            descriptionDW = ui.Q<Label>("descriptionDW");
            imageDW = ui.Q<VisualElement>("imageDW");
            containerDW = ui.Q<VisualElement>("containerDW");

            tagsBar = ui.Q<VisualElement>("TagsBar");
            inputField = ui.Q<TextField>("inputField");
            searchButton = ui.Q<VisualElement>("searchButton");

            equipmentButton = ui.Q<VisualElement>("equipmentButton");
            combosButton = ui.Q<VisualElement>("combosButton");

            equipmentButton.RegisterCallback<ClickEvent>((clEvent) => manager.SwitchMenu(UIE_MenusManager.instance.EquipmentMenu));
            combosButton.RegisterCallback<ClickEvent>((clEvent) => manager.SwitchMenu(UIE_MenusManager.instance.CombosMenu));

            onClose += () => manager.DisableMenu(gameObject.name);
            searchButton.RegisterCallback<ClickEvent>((clEvent) => CreateInvList(inputField.value));
            inputField.Children().First().AddToClassList("searchBar");
        }
    }

    private void myOnEnable()
    {
        listContainer.Clear();
        tagsBar.Clear();
        filterType = null;
        inputField.value = "";
        //animController = character.GetInContainer<AnimatorController>();

        CreateTagsButton(() => CreateInvList());
        CreateTagsButton<MeleeWeapon>(() => CreateInvList());
        CreateTagsButton<AbilityExtCast>(() => CreateInvList());
        CreateTagsButton<WeaponKata>(() => CreateInvList());

        CreateInvList();
    }

    void CreateInvList(string _filter = "")
    {
        ShowDetails("", "", null);
        buttonsList.Clear();
        listContainer.Clear();

        for (int i = 0; i < character.inventory.Count; i++)
        {
            if (filterType != null && !filterType.IsAssignableFrom(character.inventory[i].GetType()))
                continue;

            if (character.inventory[i] is Ability && !((Ability)character.inventory[i]).visible)
                continue;

            if (_filter != "" && !(character.inventory[i].nameDisplay.ToLower().Contains(_filter.ToLower())))
                continue;

            int index = i;
            AddButton(character.inventory[i].GetItemBase(), null, ()=> ShowDetails(character.inventory[index].nameDisplay, character.inventory[index].GetDetails().ToString("\n"), character.inventory[index].image));
        }

        if (buttonsList.Count <= 0)
        {
            var aux = new Label();
            listContainer.Add(aux);
            aux.text = "No se encontro ningun objeto";
            aux.AddToClassList("notFoundText");
        }
    }

    protected UIE_ListButton AddButton(ItemBase _item, Action _mainAct, Action _hoverAct)
    {
        UIE_ListButton button = new UIE_ListButton();
        listContainer.Add(button);
        buttonsList.Add(button);

        button.Init(_item.image, _item.nameDisplay, _item.GetType().ToString(), "-", () => { });

        if (_mainAct != null)
            button.SetChangeButton(_mainAct);

        if (_hoverAct != null)
            button.SetHoverAction(_hoverAct);

        return button;
    }

}
