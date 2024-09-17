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
            onClose += () => manager.DisableMenu(gameObject.name);

            basicsButtons = ui.Q<VisualElement>("Basics");
            abilitiesButtons = ui.Q<VisualElement>("Abilities");
            katasButtons = ui.Q<VisualElement>("Katas");
            statisticsLabel = ui.Q<Label>("statisticsLabel");
            combosButton = ui.Q<VisualElement>("combosButton");

            combosButton.RegisterCallback<ClickEvent>((clEvent) => manager.SwitchMenu(UIE_MenusManager.instance.CombosMenu));
        }
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
            return manager.defaultWeaponImage;
        else if (_type == typeof(WeaponKata))
            return manager.defaultKataImage;
        else
            return manager.defaultAbilityImage;
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
            return manager.defaultWeaponText;
        else if (_type == typeof(WeaponKata))
            return manager.defaultKataText;
        else
            return manager.defaultAbilityText;
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
            abilityButton.Block();

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
                if(aux.Weapon!=null)
                    kataButton.InitWeaponTooltip(aux.Weapon.nameDisplay, aux.Weapon.itemBase.GetTooltip(), null);
                else
                    kataButton.InitTooltip("Arma de Kata", "Herramienta vital para efectuar el da�o de la kata\n\n" + ("Requiere de equipar una kata en la casilla contigua".RichText("color", "#c9ba5d")), null);
            }
            else
            {
                kataButton.InitTooltip("Kata", "Movimiento marcial\n\n" + ("Requiere de equipar un arma en la casilla contigua".RichText("color", "#c9ba5d")), null);
                kataButton.InitTooltip("Arma de Kata", "Herramienta vital para efectuar el da�o de la kata\n\n" + ("Requiere de equipar una kata en la casilla contigua".RichText("color", "#c9ba5d")), null);
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
            _slotButton.InitTooltip("Habilidad", "Utilizas la energ�a de tu alrededor para materializarla en da�o", _sprite);
        }
    }

    UnityAction WeaponAction(SlotItem<MeleeWeapon> slotItem)
    {
        Action<int> equipAction = (_index) =>
        {
            slotItem.indexEquipedItem = _index;
        };

        return () =>
        {
            equipMenu.SetEquipMenu(slotItem, typeof(MeleeWeapon), equipAction);
            manager.SwitchMenu(manager.EquipItemMenu);
        };
    }

    UnityAction AbilityAction<T>(SlotItem<T> slotItem) where T : ItemEquipable
    {
        Action<int> equipAction = (_index) =>
        {
            if(_index >= 0)
            {
                var abilityCopy = ((AbilityExtCast)slotItem.inventoryComponent[_index]).CreateCopy(out int indexCopy);

                slotItem.indexEquipedItem = indexCopy;
            }
            else
                slotItem.indexEquipedItem = _index;
        };

        return () =>
        {
            equipMenu.SetEquipMenu(slotItem, typeof(T), equipAction);
            manager.SwitchMenu(manager.EquipItemMenu);
        };
    }

    UnityAction WeaponOfKataAction(SlotItem<WeaponKata> item)
    {
        Action<int> equipAction = (_index) =>
        {
            if (_index >= 0)
            {
                item.equiped.ChangeWeapon(item.inventoryComponent[_index]);
            }
            else
                item.equiped.TakeOutWeapon();
        };

        return () =>
        {
            equipMenu.SetEquipMenu(item, typeof(MeleeWeapon), equipAction);
            manager.SwitchMenu(manager.EquipItemMenu);
        };
    }

    UnityAction KataAction(SlotItem<WeaponKata> slotItem)
    {
        Action<int> equipAction = (_index) =>
        {
            if (_index >= 0)
            {
                var kataCopy = ((WeaponKata)slotItem.inventoryComponent[_index]).CreateCopy(out int indexCopy);

                slotItem.indexEquipedItem = indexCopy;
            }
            else
                slotItem.indexEquipedItem = _index;
        };

        return () =>
        {
            equipMenu.SetEquipMenu(slotItem, typeof(WeaponKata), equipAction);
            manager.SwitchMenu(manager.EquipItemMenu);
        };
    }

}
