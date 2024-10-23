using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;

public class UIE_TransmuteMenu : UIE_InventoryMenu
{
    VisualElement craftButton;

    protected override void Config()
    {
        base.Config();

        MyAwakes += myAwake;
    }

    private void myAwake()
    {
        onEnableMenu += myOnEnable;
        onDisableMenu += myOnDisable;

        listContainer = ui.Q<VisualElement>("listContainer");
        titleDW = ui.Q<Label>("titleDW");
        descriptionDW = ui.Q<Label>("descriptionDW");
        imageDW = ui.Q<VisualElement>("imageDW");
        containerDW = ui.Q<VisualElement>("containerDW");

        tagsBar = ui.Q<VisualElement>("TagsBar");
        inputField = ui.Q<TextField>("inputField");
        searchButton = ui.Q<VisualElement>("searchButton");

        craftButton = ui.Q<VisualElement>("craftButton");
        craftButton.RegisterCallback<ClickEvent>((clEvent) => manager.SwitchMenu(UIE_MenusManager.instance.CraftMenu));

        onClose += () => manager.DisableMenu(gameObject.name);
        searchButton.RegisterCallback<ClickEvent>((clEvent) => CreateListRecipes(inputField.value));
        inputField.Children().First().AddToClassList("searchBar");
    }

    private void myOnEnable()
    {
        listContainer.Clear();
        tagsBar.Clear();
        filterType = null;
        inputField.value = "";
        //animController = character.GetInContainer<AnimatorController>();

        CreateTagsButton(() => CreateListRecipes());
        CreateTagsButton<MeleeWeapon>(() => CreateListRecipes());
        CreateTagsButton<AbilityExtCast>(() => CreateListRecipes());
        CreateTagsButton<WeaponKata>(() => CreateListRecipes());

        CreateListRecipes();

        animController = character.GetInContainer<AnimatorController>();
        animController.CancelAllAnimations();
        animController.ChangeActionAnimation(manager.idleAnim, true);
    }
    private void myOnDisable()
    {
        animController.CancelAllAnimations();
    }

    void CreateListRecipes(string _filter = "")
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

            if (!typeof(MeleeWeapon).IsAssignableFrom(character.inventory[i].GetType()))
                continue;

            int index = i;
            UIE_ListButton button = default;
            button = AddButton(character.inventory[index].GetItemBase(), () =>
            {
                Transmute(character.inventory[index]);
                ShowDetails("", "", null);
                CreateListRecipes();
            },
            () => ShowDetails(character.inventory[index].nameDisplay, 
            character.inventory[index].GetDetails().ToString("\n") + "Materiales obtenidos por transmutar: \n" + ((character.inventory[index].GetItemBase() as ItemCrafteable).GetRequiresString(character.inventory)).ClearRichText().RichText("color", "#6ae26a")
            , character.inventory[index].image));
            button.EnableChange("Transmutar");
        }

        if (buttonsList.Count <= 0)
        {
            var aux = new Label();
            listContainer.Add(aux);
            aux.text = "No se encontro ningun objeto";
            aux.AddToClassList("notFoundText");
        }
    }

    void Transmute(Item _item)
    {
        ItemCrafteable _itemCraft = _item.GetItemBase() as ItemCrafteable;

        if (character == null || _itemCraft == null)
            return;

        foreach (var ingredient in _itemCraft.ingredients)
        {
            character.inventory.AddItem(ingredient.Item, ingredient.Amount);
        }

        _item.Destroy();

        //Item Despawn
        /*
        MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(true).SetWindow("", "Transmutación exitosa")
                        .AddButton("Cerrar", () => MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false));*/
    }
}
