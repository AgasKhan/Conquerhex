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


    VisualElement combosButton;
    protected override void Config()
    {
        base.Config();
        MyAwakes += myAwake;
    }

    void myAwake()
    {
        if(gameObject.name == manager.EquipmentMenu)
        {
            onEnableMenu += myEnableMenu;

            basicsButtons = ui.Q<VisualElement>("Basics");
            abilitiesButtons = ui.Q<VisualElement>("Abilities");
            katasButtons = ui.Q<VisualElement>("Katas");
            statisticsLabel = ui.Q<Label>("statisticsLabel");
            combosButton = ui.Q<VisualElement>("combosButton");

            combosButton.RegisterCallback<ClickEvent>((clEvent) => manager.SwitchMenu(UIE_MenusManager.instance.CombosMenu));
        }
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

    protected Sprite GetImage(ItemEquipable itemEquiped)
    {
        if (itemEquiped != null)
            return itemEquiped.image;
        /*else if(itemEquiped is MeleeWeapon)
            return defaultWeaponImage.texture;*/
        else
            return defaultAbilityImage;
    }
    protected Sprite GetImage(SlotItem itemEquiped)
    {
        if (itemEquiped.equiped != null)
            return itemEquiped.equiped.image;
        else if(itemEquiped.equiped is MeleeWeapon)
            return defaultWeaponImage;
        else
            return defaultAbilityImage;
    }

    protected string GetText(ItemEquipable itemEquiped)
    {
        if (itemEquiped != null)
            return itemEquiped.nameDisplay;
        /*else if (itemEquiped is MeleeWeapon)
            return defaultWeaponText;*/
        else
            return defaultAbilityText;
    }

    protected string GetText(SlotItem itemEquiped)
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
        basicButton.InitTooltip("Arma Básica", "Utiliza click izquierdo para accionarla", null);

        basicButton = new UIE_SlotButton();
        basicsButtons.Add(basicButton);
        basicButton.Init(GetImage(character.caster.abilities[0]), GetText(character.caster.abilities[0]), AbilityAction(character.caster.abilities[0]));
        basicButton.InitTooltip("Habilidad Básica", "Utiliza click derecho para accionarla", null);

        basicButton = new UIE_SlotButton();
        basicsButtons.Add(basicButton);
        basicButton.Init(GetImage(character.caster.abilities[1]), GetText(character.caster.abilities[1]), AbilityAction(character.caster.abilities[1]));
        basicButton.InitTooltip("Habilidad Alternativa", "Utiliza shift izquierdo para accionarla", null);
    }

    void CreateEquipamentAbilities()
    {
        for (int i = 2; i < character.caster.abilities.Count; i++)
        {
            UIE_SlotButton abilityButton = new UIE_SlotButton();
            abilitiesButtons.Add(abilityButton);
            abilityButton.Init(GetImage(character.caster.abilities[i]), GetText(character.caster.abilities[i]), AbilityAction(character.caster.abilities[i]));
            abilityButton.InitTooltip("Habilidad numero " + (i + 1).ToString(), "Se acciona presionando dos veces una tecla de movimiento y el click derecho", null);
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
            
            kataButton.InitTooltip("Kata numero " + (i + 1).ToString(), "Se acciona presionando dos veces una tecla de movimiento y el click izquierdo", null);
        }
    }
    

    UnityAction WeaponAction(SlotItem<MeleeWeapon> slotItem)
    {
        Action<SlotItem, int> equipAction = (_slotItem, _index) =>
        {
            _slotItem.indexEquipedItem = _index;
            //TriggerOnClose(default);
        };

        return () =>
        {
            equipMenu.SetEquipMenu<MeleeWeapon>(slotItem, typeof(MeleeWeapon), equipAction);
            manager.SwitchMenu(manager.EquipItemMenu);
        };
    }

    UnityAction AbilityAction<T>(SlotItem<T> item) where T : ItemEquipable
    {
        Action<SlotItem, int> equipAction = (_slotItem, _index) =>
        {
            var abilityCopy = ((AbilityExtCast)_slotItem.inventoryComponent[_index]).CreateCopy(out int indexCopy);
            _slotItem.indexEquipedItem = indexCopy;
            //TriggerOnClose(default);
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
            //TriggerOnClose(default);
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
