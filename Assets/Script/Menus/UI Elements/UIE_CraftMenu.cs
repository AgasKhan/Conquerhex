using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;

public class UIE_CraftMenu : UIE_EquipMenu
{
    List<UIE_ListButton> buttonsList = new List<UIE_ListButton>();
    CraftingBuild building;

    VisualElement tagsBar;

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

        onClose += () => manager.DisableMenu(gameObject.name);
    }

    private void myOnEnable()
    {
        listContainer.Clear();
        buttonsList.Clear();
        tagsBar.Clear();
        filterType = null;

        //animController = character.GetInContainer<AnimatorController>();

        CreateTagsButtons<MeleeWeapon>();
        CreateTagsButtons<AbilityExtCast>();
        CreateTagsButtons<WeaponKata>();

        CreateListRecipes();
    }
    private void myOnDisable()
    {
        //filterType = null;
    }

    void CreateListRecipes()
    {
        ShowDetails("", "", null);

        for (int i = 0; i < building.currentRecipes.Count; i++)
        {
            if (filterType != null && !filterType.IsAssignableFrom(building.currentRecipes[i].GetType()))
                continue;

            AddButton(building.currentRecipes[i]);
        }
    }

    void CreateTagsButtons<T>() where T : ItemEquipable
    {
        var aux = new VisualElement();
        tagsBar.Add(aux);
        aux.AddToClassList("tagButton");
        aux.RegisterCallback<ClickEvent>((clEvent) => 
        {
            filterType = typeof(T);
            CreateListRecipes();
        });

        aux.style.backgroundImage = new StyleBackground(UIE_MenusManager.instance.GetImage<T>(null));
    }

    void AddButton(ItemCrafteable _itemCraft)
    {
        UIE_ListButton button = new UIE_ListButton();
        listContainer.Add(button);
        buttonsList.Add(button);

        button.Init(_itemCraft.image, _itemCraft.nameDisplay, _itemCraft.GetType().ToString(), "-", () =>{});

        button.SetChangeButton(()=>
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
        });

        button.SetHoverAction(() =>
        {
            ShowDetails(_itemCraft.nameDisplay, _itemCraft.GetDetails().ToString("\n") + "Materiales necesarios: \n" + _itemCraft.GetRequiresString(character.inventory), _itemCraft.image);
        });


        if (_itemCraft.CanCraft(character.inventory))
            button.EnableChange();
        else
            button.DisableChange();

    }

    public void Init(CraftingBuild _building)
    {
        building = _building;
    }

}
