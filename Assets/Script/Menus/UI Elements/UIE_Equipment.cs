using System;
using System.Collections.Generic;
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

    public Sprite defaultKataImage;
    public string defaultKataText;

    public UIE_EquipMenu equipMenu;

    public List<Sprite> basicsKeys = new List<Sprite>();
    public List<Sprite> abilitiesKeys = new List<Sprite>();
    public List<Sprite> katasKeys = new List<Sprite>();

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

    protected Sprite GetImage(ItemEquipable itemEquiped, Type _type)
    {
        if (itemEquiped != null)
            return itemEquiped.image;
        else if (_type == typeof(MeleeWeapon))
            return defaultWeaponImage;
        else if (_type == typeof(WeaponKata))
            return defaultKataImage;
        else
            return defaultAbilityImage;
    }

    protected Sprite GetImage(SlotItem itemEquiped)
    {
        return GetImage(itemEquiped.equiped, itemEquiped.GetSlottype());
    }

    protected string GetText(ItemEquipable itemEquiped, Type _type)
    {
        if (itemEquiped != null)
            return itemEquiped.nameDisplay;
        else if (_type == typeof(MeleeWeapon))
            return defaultWeaponText;
        else if (_type == typeof(WeaponKata))
            return defaultKataText;
        else
            return defaultAbilityText;
    }

    protected string GetText(SlotItem itemEquiped)
    {
        return GetText(itemEquiped.equiped, itemEquiped.GetSlottype());

        /*
        if (itemEquiped.equiped != null)
            return itemEquiped.equiped.nameDisplay;
        else if (itemEquiped.equiped is MeleeWeapon)
            return defaultWeaponText;
        else
            return defaultAbilityText;*/
    }

    void CreateBasicsButtons()
    {
        UIE_SlotButton basicButton = new UIE_SlotButton();

        basicsButtons.Add(basicButton);
        basicButton.Init(GetImage(character.caster.weapons[0]), GetText(character.caster.weapons[0]), WeaponAction(character.caster.weapons[0]), character.caster.weapons[0].GetSlottype());
        SetButtonTooltip(basicButton, character.caster.weapons[0], basicsKeys[0]);

        basicButton = new UIE_SlotButton();
        basicsButtons.Add(basicButton);
        basicButton.Init(GetImage(character.caster.abilities[0]), GetText(character.caster.abilities[0]), AbilityAction(character.caster.abilities[0]), character.caster.abilities[0].GetSlottype());
        SetButtonTooltip(basicButton, character.caster.abilities[0], basicsKeys[1]);

        basicButton = new UIE_SlotButton();
        basicsButtons.Add(basicButton);
        basicButton.Init(GetImage(character.caster.abilities[1]), GetText(character.caster.abilities[1]), AbilityAction(character.caster.abilities[1]), character.caster.abilities[1].GetSlottype());
        SetButtonTooltip(basicButton, character.caster.abilities[1], basicsKeys[2]);
    }

    void CreateEquipamentAbilities()
    {
        for (int i = 2; i < character.caster.abilities.Count; i++)
        {
            UIE_SlotButton abilityButton = new UIE_SlotButton();
            abilitiesButtons.Add(abilityButton);
            abilityButton.Init(GetImage(character.caster.abilities[i]), GetText(character.caster.abilities[i]), AbilityAction(character.caster.abilities[i]), character.caster.abilities[i].GetSlottype());

            SetButtonTooltip(abilityButton, character.caster.abilities[i-2], abilitiesKeys[i-2]);
        }
    }
    
    void CreateEquipamentKatas()
    {
        for (int i = 0; i < character.caster.katas.Count; i++)
        {
            UIE_KataButton kataButton = new UIE_KataButton();

            katasButtons.Add(kataButton);
            kataButton.Init(GetImage(character.caster.katas[i]), GetText(character.caster.katas[i]), KataAction(character.caster.katas[i])
                , GetImage(character.caster.katas[i].equiped?.Weapon, typeof(MeleeWeapon)), GetText(character.caster.katas[i].equiped?.Weapon, typeof(MeleeWeapon)), character.caster.katas[i].equiped == null? ()=> { } : WeaponOfKataAction(character.caster.katas[i]));

            var aux = character.caster.katas[i].equiped;
            if(aux!=null)
            {
                kataButton.InitTooltip(aux.nameDisplay, aux.itemBase.GetTooltip(), katasKeys[i]);
            }
            else
            {
                kataButton.InitTooltip("Kata", "Movimiento marcial\n\n" + ("Requiere de equipar un arma en la casilla contigua".RichText("color", "#c9ba5d")), null);
            }
        }
    }
    
    void SetButtonTooltip(UIE_SlotButton _slotButton, SlotItem _slotItem, Sprite _sprite)
    {
        ItemEquipable aux = _slotItem.equiped;

        if (aux != null)
        {
            _slotButton.InitTooltip(aux.nameDisplay, aux.GetItemBase().GetTooltip(), _sprite);
        }
        else if(_slotItem.GetSlottype() == typeof(MeleeWeapon))
        {
            _slotButton.InitTooltip("Arma", "Herramienta usada tanto para atacar como para recolectar recursos\n\n" + "Primer ataque de combo".RichText("color", "#c9ba5d"), _sprite);
        }
        else
        {
            _slotButton.InitTooltip("Habilidad", "Utilizas la energía de tu alrededor para materializarla en daño", _sprite);
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
