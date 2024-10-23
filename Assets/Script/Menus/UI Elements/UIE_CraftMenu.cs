using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;

public class UIE_CraftMenu : UIE_InventoryMenu
{
    CraftingBuild building;
    VisualElement transmuteButton;

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

        transmuteButton = ui.Q<VisualElement>("transmuteButton");
        transmuteButton.RegisterCallback<ClickEvent>((clEvent) => manager.SwitchMenu(UIE_MenusManager.instance.TransmuteMenu));

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

        for (int i = 0; i < building.currentRecipes.Count; i++)
        {
            if (filterType != null && !filterType.IsAssignableFrom(building.currentRecipes[i].GetItemType()))
                continue;

            if (_filter != "" && !(building.currentRecipes[i].nameDisplay.ToLower().Contains(_filter.ToLower())))
                continue;

            AddButton(building.currentRecipes[i]);
        }

        if (buttonsList.Count <= 0)
        {
            var aux = new Label();
            listContainer.Add(aux);
            aux.text = "No se encontro ningun objeto";
            aux.AddToClassList("notFoundText");
        }
    }

    void AddButton(ItemCrafteable _itemCraft)
    {
        UIE_ListButton button = AddButton(_itemCraft, () =>
        {
            if (character == null || _itemCraft == null)
                return;

            if (_itemCraft.CanCraft(character.inventory))
            {
                _itemCraft.Craft(character.inventory, (string)_itemCraft.nameDisplay);
                //audioComponent.Play("CraftAudio");

                if (_itemCraft is MeleeWeaponBase)
                {
                    ShowDetails(_itemCraft.nameDisplay, _itemCraft.GetDetails().ToString("\n") + "Materiales necesarios: \n" + _itemCraft.GetRequiresString(character.inventory), _itemCraft.image);
                }
                else
                {
                    building.currentRecipes.Remove(_itemCraft);
                    ShowDetails("", "", null);
                    CreateListRecipes();
                }
                return;
            }
            else
            {
                MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(true).SetWindow("", "No tienes suficientes materiales")
                            .AddButton("Cerrar", () => MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false));
            }
        }, () => ShowDetails(_itemCraft.nameDisplay, _itemCraft.GetDetails().ToString("\n") + "Materiales necesarios: \n" + _itemCraft.GetRequiresString(character.inventory), _itemCraft.image));


        if (_itemCraft.CanCraft(character.inventory))
            button.EnableChange("Crear");
        else
            button.DisableChange();

    }

    public void Init(CraftingBuild _building)
    {
        building = _building;
    }

}
