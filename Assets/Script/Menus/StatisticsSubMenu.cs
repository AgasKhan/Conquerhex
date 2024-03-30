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

        subMenu.AddNavBarButton("Statistics", Create).AddNavBarButton("Inventory", inventorySubMenu.Create);

        subMenu.ClearBody();

        subMenu.CreateSection(0, 2);
        subMenu.CreateChildrenSection<ScrollRect>();
        subMenu.AddComponent<DetailsWindow>().SetImage(character.flyweight.image).SetTexts(character.flyweight.nameDisplay, character.flyweight.GetDetails().ToString("\n"));

        subMenu.CreateSection(2, 6);
        subMenu.CreateChildrenSection<ScrollRect>();

        CreateWeaponButtons(character);
    }

    void CreateWeaponButtons(Character charac)
    {
        for (int i = 0; i < charac.caster.weapons.Count; i++)
        {
            //CreateWeaponButtons(charac, i);
            CreateWeapButtons(charac.caster.weapons[i]);
        }

        for (int i = 0; i < charac.caster.katasCombo.Count ; i++)
        {
            CreateKataCombosButtons(charac.caster.katasCombo[i]);
        }
        //CreateWeaponButtons(charac, 0);
        //CreateWeaponButtons(charac, 1);
        //CreateWeaponButtons(charac, 2);
    }

    void CreateWeapButtons(SlotItem<MeleeWeapon> item)
    {
        string nameWeapon = "Equip Weapon";
        Sprite spriteWeapon = null;
        string strWeapon = "";
        System.Type filter =  typeof(MeleeWeapon);

        UnityEngine.Events.UnityAction action;

        System.Action< SlotItem,int > equipAction = (_slotItem, _index) =>
        {
            _slotItem.indexEquipedItem = _index;
            MenuManager.instance.modulesMenu.ObtainMenu<SubMenus>(false);
        };
        
        if (item.equiped != null)
        {
            nameWeapon = item.equiped.nameDisplay;
            spriteWeapon = item.equiped.image;
            strWeapon = "Uses: " + item.equiped.current;
        }

        action = () =>
        {
            inventorySubMenu.SetEquipMenu<MeleeWeapon>(item, filter, equipAction);
            inventorySubMenu.Create();
        };
        subMenu.AddComponent<ButtonA>().SetButtonA(nameWeapon, spriteWeapon, strWeapon, action);
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

        if (kata.equiped != null)
        {
            nameKata = kata.equiped.nameDisplay;
            spriteKAta = kata.equiped.image;

            if(kata.equiped.Weapon != null)
            {
                nameWeapon = kata.equiped.Weapon.nameDisplay;
                spriteWeapon = kata.equiped.Weapon.image;
                strWeapon = "Uses: " + kata.equiped.Weapon.current;
            }
        }

        System.Action<SlotItem, int> equipKataAction = (_slotItem, _index) =>
        {
            _slotItem.indexEquipedItem = _index;
            MenuManager.instance.modulesMenu.ObtainMenu<SubMenus>(false);
        };

        System.Action<SlotItem, int> equipWeaponAction = (_slotItem, _index) =>
        {
            (_slotItem as SlotItem<WeaponKata>).equiped.ChangeWeapon(_slotItem.inventoryComponent.inventory[_index]);
            MenuManager.instance.modulesMenu.ObtainMenu<SubMenus>(false);
        };

        actionKata = () =>
        {
            inventorySubMenu.SetEquipMenu<WeaponKata>(kata, filterKata, equipKataAction);
            inventorySubMenu.Create();
        };

        actionWeapon = () =>
        {
            inventorySubMenu.SetEquipMenu<WeaponKata>(kata, filterWeapon, equipWeaponAction);
            inventorySubMenu.Create();
        };

        //subMenu.AddComponent<ButtonA>().SetButtonA(nameWeapon, spriteWeapon, strWeapon, action);

        var doubleButton = subMenu.AddComponent<DoubleButtonA>();
        doubleButton.left.SetButtonA(nameKata, spriteKAta, strKata, actionKata);
        doubleButton.right.SetButtonA(nameWeapon, spriteWeapon, strWeapon, actionWeapon);
    }

    void CreateWeaponButtons(Character ch, int index)
    {
        ch.caster.katasCombo.Actual(index);

        string nameKata = "Ranura habilidad vacia";
        Sprite spriteKata = null;
        string nameArmas = "Ranura arma vacia";
        Sprite spriteWeapon = null;
        string strWeapon = "";

        UnityEngine.Events.UnityAction action = () =>
        {
            ch.caster.katasCombo.Actual(index);
            inventorySubMenu.Create();
        };

        if (ch.caster.katasCombo.actual.equiped != null)
        {
            nameKata = ch.caster.katasCombo.actual.equiped.nameDisplay;
            spriteKata = ch.caster.katasCombo.actual.equiped.image;

            if(ch.caster.katasCombo.actual.equiped.WeaponEnabled != null)
            {
                nameArmas = ch.caster.katasCombo.actual.equiped.WeaponEnabled.nameDisplay;
                spriteWeapon = ch.caster.katasCombo.actual.equiped.WeaponEnabled.image;
                strWeapon = "Uses: " + ch.caster.katasCombo.actual.equiped.WeaponEnabled.current;

            }

            subMenu.AddComponent<ButtonA>().SetButtonA(nameArmas, spriteWeapon, "", action);
        }

        //subMenu.AddComponent<ButtonA>().SetButtonA(nameKata, spriteKata, "", action);
    }
}
