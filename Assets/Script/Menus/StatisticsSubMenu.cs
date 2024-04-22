using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class StatisticsSubMenu : CreateSubMenu
{
    public Character character;

    [SerializeField]
    InventorySubMenu inventorySubMenu;

    protected override void InternalCreate()
    {
        subMenu.navbar.DestroyAll();

        subMenu.AddNavBarButton("Statistics", Create).AddNavBarButton("Inventory", CreateInventory);

        subMenu.ClearBody();

        subMenu.CreateSection(0, 2);
        subMenu.CreateChildrenSection<ScrollRect>();
        subMenu.AddComponent<DetailsWindow>().SetImage(character.flyweight.image).SetTexts(character.flyweight.nameDisplay, character.flyweight.GetDetails().ToString("\n"));

        subMenu.CreateSection(2, 6);
        subMenu.CreateChildrenSection<ScrollRect>();

        CreateWeaponButtons(character);

        subMenu.OnClose += Exit;
    }

    private void Exit()
    {
        subMenu.ExitSubmenu();
    }

    void CreateInventory()
    {
        inventorySubMenu.Create();
        subMenu.OnClose -= Exit;
        subMenu.OnClose += SubMenuOnClose;
    }

    private void SubMenuOnClose()
    {
        subMenu.OnClose -= SubMenuOnClose;
        Create();
    }

    void CreateWeaponButtons(Character charac)
    {
        for (int i = 0; i < charac.caster.weapons.Count; i++)
        {
            CreateGenericButton(charac.caster.weapons[i], "Equip Weapon", 
            (_slotItem, _index) =>
            {
                _slotItem.indexEquipedItem = _index; subMenu.TriggerOnClose();
            });
        }

        for (int i = 0; i < charac.caster.katasCombo.Count ; i++)
        {
            CreateKataCombosButtons(charac.caster.katasCombo[i]);
        }

        for (int i = 0; i < charac.caster.abilities.Count; i++)
        {
            CreateGenericButton(charac.caster.abilities[i], "Equip Ability",
            (_slotItem, _index) =>
            {
                var abilityCopy = ((AbilityExtCast)_slotItem.inventoryComponent[_index]).CreateCopy();
                abilityCopy.Init(_slotItem.inventoryComponent);
                _slotItem.indexEquipedItem = _slotItem.inventoryComponent.Count - 1;
                subMenu.TriggerOnClose();
            });
        }
    }
    void CreateGenericButton<T>(SlotItem<T> item, string defaultName, System.Action<SlotItem, int> equipAction) where T : Item
    {
        var info = new SlotInfo(defaultName, null, "", typeof(T));

        UnityEngine.Events.UnityAction action = () =>
        {
            inventorySubMenu.SetEquipMenu<MeleeWeapon>(item, info.filter, equipAction);
            CreateInventory();
        };

        if (item.equiped != null)
        {
            info.name = item.equiped.nameDisplay;
            info.sprite = item.equiped.image;

            if(item.equiped is MeleeWeapon)
                info.str = "Uses: " + (item.equiped as MeleeWeapon).current;
        }

        subMenu.AddComponent<ButtonA>().SetButtonA(info.name, info.sprite, info.str, action);
    }

    void CreateKataCombosButtons(SlotItem<WeaponKata> kata)
    {
        var infoKata = new SlotInfo("Equip Kata", null, "", typeof(WeaponKata));
        UnityEngine.Events.UnityAction actionKata;

        var infoWeapon = new SlotInfo("Equip Weapon", null, "", typeof(MeleeWeapon));
        UnityEngine.Events.UnityAction actionWeapon;

        bool interactiveWeap = false;

        if (kata.equiped != null)
        {
            infoKata.name = kata.equiped.nameDisplay;
            infoKata.sprite = kata.equiped.image;
            interactiveWeap = true;

            if (kata.equiped.WeaponEnabled != null)
            {
                infoWeapon.name = kata.equiped.Weapon.nameDisplay;
                infoWeapon.sprite = kata.equiped.Weapon.image;
                infoWeapon.str = "Uses: " + kata.equiped.Weapon.current;
            }
        }

        System.Action<SlotItem, int> equipKataAction = (_slotItem, _index) =>
        {
            var kataCopy = ((WeaponKata)_slotItem.inventoryComponent[_index]).CreateCopy();
            kataCopy.Init(_slotItem.inventoryComponent);
            _slotItem.indexEquipedItem = _slotItem.inventoryComponent.Count - 1;
            subMenu.TriggerOnClose();
        };

        System.Action<SlotItem, int> equipWeaponAction = (_slotItem, _index) =>
        {
            (_slotItem as SlotItem<WeaponKata>).equiped.ChangeWeapon(_slotItem.inventoryComponent[_index]);
            subMenu.TriggerOnClose();
        };

        actionKata = () =>
        {
            inventorySubMenu.SetEquipMenu<WeaponKata>(kata, infoKata.filter, equipKataAction);
            CreateInventory();
        };

        actionWeapon = () =>
        {
            inventorySubMenu.SetEquipMenu<WeaponKata>(kata, infoWeapon.filter, equipWeaponAction);
            CreateInventory();
        };


        var doubleButton = subMenu.AddComponent<DoubleButtonA>();
        doubleButton.left.SetButtonA(infoKata.name, infoKata.sprite, infoKata.str, actionKata);
        doubleButton.right.SetButtonA(infoWeapon.name, infoWeapon.sprite, infoWeapon.str, actionWeapon).button.interactable = interactiveWeap;
    }
}

public struct SlotInfo
{
    public string name;
    public Sprite sprite;
    public string str;
    public System.Type filter;

    public SlotInfo(string name, Sprite sprite, string str, Type filter)
    {
        this.name = name;
        this.sprite = sprite;
        this.str = str;
        this.filter = filter;
    }
}