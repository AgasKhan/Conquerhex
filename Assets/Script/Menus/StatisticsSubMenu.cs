using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
                var abilityCopy = ((AbilityExtCast)_slotItem.inventoryComponent.inventory[_index]).CreateCopy();
                abilityCopy.Init(_slotItem.inventoryComponent);
                _slotItem.inventoryComponent.inventory.Add(abilityCopy);
                _slotItem.indexEquipedItem = _slotItem.inventoryComponent.inventory.Count - 1;
                subMenu.TriggerOnClose();
            });
        }
    }
    void CreateGenericButton<T>(SlotItem<T> item, string defaultName, System.Action<SlotItem, int> equipAction) where T : Item
    {
        string name = defaultName;
        Sprite sprite = null;
        string str = "";
        System.Type filter = typeof(T);

        UnityEngine.Events.UnityAction action = () =>
        {
            inventorySubMenu.SetEquipMenu<MeleeWeapon>(item, filter, equipAction);
            CreateInventory();
        };


        if (item.equiped != null)
        {
            name = item.equiped.nameDisplay;
            sprite = item.equiped.image;

            if(item.equiped is MeleeWeapon)
                str = "Uses: " + (item.equiped as MeleeWeapon).current;
        }

        subMenu.AddComponent<ButtonA>().SetButtonA(name, sprite, str, action);
    }

    void CreateKataCombosButtons(SlotItem<WeaponKata> kata)
    {
        string nameKata = "Equip Kata";
        Sprite spriteKAta = null;
        string strKata = "";
        System.Type filterKata = typeof(WeaponKata);

        UnityEngine.Events.UnityAction actionKata;
        UnityEngine.Events.UnityAction actionWeapon;

        string nameWeapon = "Equip Weapon";
        Sprite spriteWeapon = null;
        string strWeapon = "";
        System.Type filterWeapon = typeof(MeleeWeapon);
        bool interactiveWeap = false;

        if (kata.equiped != null)
        {
            nameKata = kata.equiped.nameDisplay;
            spriteKAta = kata.equiped.image;
            interactiveWeap = true;

            if (kata.equiped.WeaponEnabled != null)
            {
                nameWeapon = kata.equiped.Weapon.nameDisplay;
                spriteWeapon = kata.equiped.Weapon.image;
                strWeapon = "Uses: " + kata.equiped.Weapon.current;
            }
        }

        System.Action<SlotItem, int> equipKataAction = (_slotItem, _index) =>
        {
            var kataCopy = ((WeaponKata)_slotItem.inventoryComponent.inventory[_index]).CreateCopy();
            kataCopy.Init(_slotItem.inventoryComponent);
            _slotItem.inventoryComponent.inventory.Add(kataCopy);
            _slotItem.indexEquipedItem = _slotItem.inventoryComponent.inventory.Count - 1;
            subMenu.TriggerOnClose();
        };

        System.Action<SlotItem, int> equipWeaponAction = (_slotItem, _index) =>
        {
            (_slotItem as SlotItem<WeaponKata>).equiped.ChangeWeapon(_slotItem.inventoryComponent.inventory[_index]);
            subMenu.TriggerOnClose();
        };

        actionKata = () =>
        {
            inventorySubMenu.SetEquipMenu<WeaponKata>(kata, filterKata, equipKataAction);
            CreateInventory();
        };

        actionWeapon = () =>
        {
            inventorySubMenu.SetEquipMenu<WeaponKata>(kata, filterWeapon, equipWeaponAction);
            CreateInventory();
        };


        var doubleButton = subMenu.AddComponent<DoubleButtonA>();
        doubleButton.left.SetButtonA(nameKata, spriteKAta, strKata, actionKata);
        doubleButton.right.SetButtonA(nameWeapon, spriteWeapon, strWeapon, actionWeapon).button.interactable = interactiveWeap;
    }
}
