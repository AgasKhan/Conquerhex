using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UIE_Equipment : UIE_BaseMenu
{
    VisualElement basicsButtons;
    VisualElement abilitiesButtons;
    VisualElement katasButtons;

    Label statisticsLabel;

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
        statisticsLabel = ui.Q<Label>("statisticsLabel");

        onClose += () => manager.DisableMenu(gameObject.name);
    }
    void myEnableMenu()
    {
        basicsButtons.Clear();
        abilitiesButtons.Clear();
        katasButtons.Clear();

        CreateBasicsButtons();
        CreateEquipamentAbilities();
        CreateEquipamentKatas();

        SetStatistics();
    }

    void SetStatistics()
    {
        statisticsLabel.text = character.flyweight.GetFlyWeight<BodyBase>().GetStatistics();
    }

    Texture2D GetImage(ItemEquipable itemEquiped)
    {
        if (itemEquiped != null)
            return itemEquiped.image.texture;
        /*else if(itemEquiped is MeleeWeapon)
            return defaultWeaponImage.texture;*/
        else
            return defaultAbilityImage.texture;
    }
    Texture2D GetImage(SlotItem itemEquiped)
    {
        if (itemEquiped.equiped != null)
            return itemEquiped.equiped.image.texture;
        else if(itemEquiped.equiped is MeleeWeapon)
            return defaultWeaponImage.texture;
        else
            return defaultAbilityImage.texture;
    }

    string GetText(ItemEquipable itemEquiped)
    {
        if (itemEquiped != null)
            return itemEquiped.nameDisplay;
        /*else if (itemEquiped is MeleeWeapon)
            return defaultWeaponText;*/
        else
            return defaultAbilityText;
    }

    string GetText(SlotItem itemEquiped)
    {
        if (itemEquiped.equiped != null)
            return itemEquiped.equiped.nameDisplay;
        else if (itemEquiped.equiped is MeleeWeapon)
            return defaultWeaponText;
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
    
    void CreateEquipamentKatas()
    {
        for (int i = 0; i < character.caster.katas.Count; i++)
        {
            UIE_KataButton kataButton = new UIE_KataButton();

            katasButtons.Add(kataButton);
            kataButton.Init(GetImage(character.caster.katas[i]), GetText(character.caster.katas[i]), KataAction(character.caster.katas[i])
                , GetImage(character.caster.katas[i].equiped?.Weapon), GetText(character.caster.katas[i].equiped?.Weapon), WeaponOfKataAction(character.caster.katas[i]));
        }
    }
    

    UnityAction WeaponAction(SlotItem<MeleeWeapon> item)
    {
        Action<SlotItem, int> equipAction = (_slotItem, _index) =>
        {
            _slotItem.indexEquipedItem = _index;
            //TriggerOnClose(default);
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

    UnityAction WeaponOfKataAction(SlotItem<WeaponKata> item)
    {
        Action<SlotItem, int> equipAction = (_slotItem, _index) =>
        {
            (_slotItem as SlotItem<WeaponKata>).equiped.ChangeWeapon(_slotItem.inventoryComponent[_index]);
            TriggerOnClose(default);
        };

        return () =>
        {
            equipMenu.SetEquipMenu<MeleeWeapon>(item, typeof(MeleeWeapon), equipAction);
            manager.SwitchMenu(manager.EquipItemMenu);
        };
    }

    UnityAction KataAction(SlotItem<WeaponKata> item)
    {
        Action<SlotItem, int> equipAction = (_slotItem, _index) =>
        {
            ((WeaponKata)_slotItem.inventoryComponent[_index]).CreateCopy(out int indexCopy);
            _slotItem.indexEquipedItem = indexCopy;
            TriggerOnClose(default);
        };

        return () =>
        {
            equipMenu.SetEquipMenu<MeleeWeapon>(item, typeof(WeaponKata), equipAction);
            manager.SwitchMenu(manager.EquipItemMenu);
        };
    }

}
