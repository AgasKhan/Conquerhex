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
            onDisableMenu += myDisable;
            onClose += () => manager.DisableMenu(gameObject.name);

            basicsButtons = ui.Q<VisualElement>("Basics");
            abilitiesButtons = ui.Q<VisualElement>("Abilities");
            katasButtons = ui.Q<VisualElement>("Katas");
            statisticsLabel = ui.Q<Label>("statisticsLabel");
            combosButton = ui.Q<VisualElement>("combosButton");

            combosButton.RegisterCallback<ClickEvent>((clEvent) => manager.SwitchMenu(UIE_MenusManager.instance.CombosMenu));

            animTimer = TimersManager.Create(5, () => ShowAnimation()).SetLoop(true).Stop().SetUnscaled(true);
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

        animController = character.GetInContainer<AnimatorController>();

        animController.CancelAllAnimations();
        character.GetInContainer<ModularEquipViewEntityComponent>().DeSpawnWeapon();
    }

    void myDisable()
    {
        animTimer.Stop();
        animController.CancelAllAnimations();
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
        return GetImage(itemEquiped.equiped, itemEquiped.GetSlotType());
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
        return GetText(itemEquiped.equiped, itemEquiped.GetSlotType());
    }


    UIE_SlotButton CreateSlotButton<T>(SlotItem<T> slotInCaster) where T : ItemEquipable
    {
        UIE_SlotButton basicButton = new UIE_SlotButton();

        UnityAction equipedAction;

        if(typeof(MeleeWeapon) == typeof(T))
        {
            equipedAction = WeaponAction(slotInCaster);
        }
        else
        {
            equipedAction = AbilityAction(slotInCaster as SlotItem<AbilityExtCast>);
        }

        basicButton.Init<T>(slotInCaster, equipedAction);

        if(character.caster.abilityCasting != null && character.caster.abilityCasting.Equals(slotInCaster.equiped))
            basicButton.Block("En uso");
        else
            basicButton.Block(slotInCaster.isBlocked);

        return basicButton;
    }

    void CreateBasicsButtons()
    {
        var basicWeapon = CreateSlotButton(character.caster.weapons[0]);
        basicsButtons.Add(basicWeapon);
        basicWeapon.AddEnterMouseEvent(()=>ShowHideWeaponInMenu(true));
        basicWeapon.AddLeaveMouseEvent(() => ShowHideWeaponInMenu(false));

        basicsButtons.Add(CreateSlotButton(character.caster.abilities[0]));
        basicsButtons.Add(CreateSlotButton(character.caster.abilities[1]));
    }

    void ShowHideWeaponInMenu(bool _condition)
    {
        Debug.Log("ShowHideWeaponInMenu-------------------------------------------");
        if (character.caster.weapons[0].equiped == null)
            return;
        
        if (_condition)
            character.GetInContainer<ModularEquipViewEntityComponent>().SpawnWeapon(character.caster.weapons[0].equiped.itemBase.weaponModel);
        else
            character.GetInContainer<ModularEquipViewEntityComponent>().DeSpawnWeapon(character.caster.weapons[0].equiped.itemBase.weaponModel);
    }

    void CreateEquipamentAbilities()
    {
        for (int i = 2; i < character.caster.abilities.Count; i++)
        {
            UIE_SlotButton abilityButton = CreateSlotButton(character.caster.abilities[i]);

            abilitiesButtons.Add(abilityButton);
            abilityButton.Block(character.caster.abilities[i].isBlocked);
        }
    }
    
    void CreateEquipamentKatas()
    {
        for (int i = 0; i < character.caster.katas.Count; i++)
        {
            UIE_KataButton kataButton = new UIE_KataButton();
            int index = i;
            /*
            kataButton.Init(GetImage(character.caster.katas[i]), GetText(character.caster.katas[i]), KataAction(character.caster.katas[i])
                , GetImage(character.caster.katas[i].equiped?.Weapon, typeof(MeleeWeapon)), GetText(character.caster.katas[i].equiped?.Weapon, 
            typeof(MeleeWeapon)), character.caster.katas[i].equiped == null? ()=> { } : WeaponOfKataAction(character.caster.katas[i]));
            */

            kataButton.Init(character.caster.katas[i], KataAction(character.caster.katas[i]), character.caster.katas[i].equiped == null ? () => { } : WeaponOfKataAction(character.caster.katas[i]));
            
            if (character.caster.abilityCasting != null && character.caster.abilityCasting.Equals(character.caster.katas[i].equiped))
                kataButton.Block("En uso");
            else
                kataButton.Block(character.caster.katas[i].isBlocked);

            if(character.caster.katas[index].equiped != null)
            {
                kataButton.AddEnterMouseEvent(() => HoverKata(index));
                kataButton.AddLeaveMoususeEvent(() => StopAnimation());
            }

            katasButtons.Add(kataButton);
        }
    }

    Timer animTimer;
    AnimationInfo.Data animData;
    AnimatorController animController;
    void HoverKata(int _index)
    {
        animData = character.caster.katas[_index].equiped.itemBase.animations.animClips["Cast"];

        ShowAnimation();
        animTimer.Set(animData.Length + 0.25f).Reset();
    }

    void ShowAnimation()
    {
        animController.ChangeActionAnimation(animData);
    }
    void StopAnimation()
    {
        animTimer.Stop();
        animController.CancelAllAnimations();
    }
    UnityAction WeaponAction(SlotItem slotItem)
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

    UnityAction AbilityAction(SlotItem slotItem)
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
            equipMenu.SetEquipMenu(slotItem, equipAction);
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
            equipMenu.SetEquipMenu(item, typeof(MeleeWeapon),equipAction);
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
            equipMenu.SetEquipMenu(slotItem, equipAction);
            manager.SwitchMenu(manager.EquipItemMenu);
        };
    }

}
