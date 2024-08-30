using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UIE_Equipment : UIE_BaseMenu
{
    VisualElement basicsButtons;
    VisualElement abilitiesButtons;
    VisualElement katasButtons;

    public Sprite defaultWeaponImage;
    public string defaultWeaponText;

    public Sprite defaultAbilityImage;
    public string defaultAbilityText;

    public UIE_EquipMenu equipMenu;

    protected override void Config()
    {
        base.Config();
        MyAwakes += myAwake;
    }

    void myAwake()
    {
        onEnableMenu += myEnableMenu;

        basicsButtons = ui.Q<VisualElement>("Basics");
        abilitiesButtons = ui.Q<VisualElement>("Abilities");
        katasButtons = ui.Q<VisualElement>("Katas");

        onClose += () => manager.DisableMenu(gameObject.name);
    }
    void myEnableMenu()
    {
        basicsButtons.Clear();
        abilitiesButtons.Clear();
        katasButtons.Clear();

        CreateBasicsButtons();
        CreateEquipamentAbilities();
    }

    Texture2D GetImage(SlotItem slot)
    {
        if (slot.equiped != null)
            return slot.equiped.image.texture;
        else
            return defaultAbilityImage.texture;
    }

    string GetText(SlotItem slot)
    {
        if (slot.equiped != null)
            return slot.equiped.nameDisplay;
        else
            return defaultAbilityText;
    }

    void CreateBasicsButtons()
    {
        UIE_SlotButton basicButton = new UIE_SlotButton();

        basicsButtons.Add(basicButton);
        basicButton.Init(GetImage(character.caster.weapons[0]), GetText(character.caster.weapons[0]), WeaponAction(character.caster.weapons[0]));

        basicButton = new UIE_SlotButton();
        basicsButtons.Add(basicButton);
        basicButton.Init(GetImage(character.caster.abilities[0]), GetText(character.caster.abilities[0]), AbilityAction(character.caster.abilities[0]));

        basicButton = new UIE_SlotButton();
        basicsButtons.Add(basicButton);
        basicButton.Init(GetImage(character.caster.abilities[1]), GetText(character.caster.abilities[1]), AbilityAction(character.caster.abilities[1]));
    }

    void CreateEquipamentAbilities()
    {
        for (int i = 2; i < character.caster.abilities.Count; i++)
        {
            UIE_SlotButton abilityButton = new UIE_SlotButton();
            abilitiesButtons.Add(abilityButton);
            abilityButton.Init(GetImage(character.caster.abilities[i]), GetText(character.caster.abilities[i]), AbilityAction(character.caster.abilities[i]));
        }
    }
    /*
    void CreateEquipamentKatas(Character charac)
    {
        UIE_SlotButton kataButton = new UIE_SlotButton();
        for (int i = 0; i < charac.caster.katasCombo.Count; i++)
        {

            myAbilityKataModule.SetKataComboButton(i, charac.caster.katasCombo[i],
                KataAction(charac.caster.katasCombo[i]), WeaponOfKataAction(charac.caster.katasCombo[i]));
        }
    }
    */

    UnityAction WeaponAction(SlotItem<MeleeWeapon> item)
    {
        Action<SlotItem, int> equipAction = (_slotItem, _index) =>
        {
            _slotItem.indexEquipedItem = _index;
            TriggerOnClose(default);
        };

        return () =>
        {
            equipMenu.SetEquipMenu<MeleeWeapon>(item, typeof(MeleeWeapon), equipAction);
            manager.SwitchMenu(manager.EquipItemMenu);
        };
    }

    UnityAction AbilityAction<T>(SlotItem<T> item) where T : ItemEquipable
    {
        Action<SlotItem, int> equipAction = (_slotItem, _index) =>
        {
            var abilityCopy = ((AbilityExtCast)_slotItem.inventoryComponent[_index]).CreateCopy(out int indexCopy);
            _slotItem.indexEquipedItem = indexCopy;
            TriggerOnClose(default);
        };

        return () =>
        {
            equipMenu.SetEquipMenu<MeleeWeapon>(item, typeof(T), equipAction);
            manager.SwitchMenu(manager.EquipItemMenu);
        };
    }

}
