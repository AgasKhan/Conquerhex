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

        subMenu.CreateSection(0, 3);
        subMenu.CreateChildrenSection<ScrollRect>();
        subMenu.AddComponent<DetailsWindow>().SetImage(character.flyweight.image).SetTexts(character.flyweight.nameDisplay, character.flyweight.GetDetails().ToString("\n"));

        subMenu.CreateSection(3, 6);
        subMenu.CreateChildrenSection<ScrollRect>();

        CreateWeaponButtons(character);
    }

    void CreateWeaponButtons(Character charac)
    {
        for (int i = 0; i < charac.caster.weapons.Count; i++)
        {
            //CreateWeaponButtons(charac, i);
            CreateWeapButtons(charac.caster.weapons[i], i);
        }
        //CreateWeaponButtons(charac, 0);
        //CreateWeaponButtons(charac, 1);
        //CreateWeaponButtons(charac, 2);
    }

    void CreateWeapButtons(SlotItem<MeleeWeapon> item, int index)
    {
        string nameWeapon = "Equip Base Weapon";
        Sprite spriteWeapon = null;
        string strWeapon = ">";

        UnityEngine.Events.UnityAction action = () =>
        {
            inventorySubMenu.Create();
        };

        if (item.equiped != null)
        {
            nameWeapon = item.equiped.nameDisplay;
            spriteWeapon = item.equiped.image;
            strWeapon = "Uses: " + item.equiped.current;
            action = () => { inventorySubMenu.SetFilter(item.equiped.GetItemBase()); /*inventorySubMenu.SetEquipAct(item.toChange, index);*/ inventorySubMenu.Create(); };
        }

        subMenu.AddComponent<ButtonA>().SetButtonA(nameWeapon, spriteWeapon, "", action);
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
