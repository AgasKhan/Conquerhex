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
        CreateWeaponButtons(charac, 0);
        CreateWeaponButtons(charac, 1);
        CreateWeaponButtons(charac, 2);
    }


    void CreateWeaponButtons(Character ch, int index)
    {
        ch.attack.katasCombo.Actual(index);

        string nameKata = "Ranura habilidad vacia";
        Sprite spriteKata = null;
        string nameArmas = "Ranura arma vacia";
        Sprite spriteWeapon = null;
        string strWeapon = "";

        UnityEngine.Events.UnityAction action = () =>
        {
            ch.attack.katasCombo.Actual(index);
            inventorySubMenu.Create();
        };

        if (ch.attack.katasCombo.actual.equiped != null)
        {
            nameKata = ch.attack.katasCombo.actual.equiped.nameDisplay;
            spriteKata = ch.attack.katasCombo.actual.equiped.image;

            if(ch.attack.katasCombo.actual.equiped.weaponEnabled != null)
            {
                nameArmas = ch.attack.katasCombo.actual.equiped.weaponEnabled.nameDisplay;
                spriteWeapon = ch.attack.katasCombo.actual.equiped.weaponEnabled.image;
                strWeapon = "Uses: " + ch.attack.katasCombo.actual.equiped.weaponEnabled.current;

            }

            subMenu.AddComponent<ButtonA>().SetButtonA(nameArmas, spriteWeapon, "", action);
        }

        //subMenu.AddComponent<ButtonA>().SetButtonA(nameKata, spriteKata, "", action);
    }
}
