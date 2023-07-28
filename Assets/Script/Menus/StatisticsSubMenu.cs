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
        ch.weaponKataIndex = index;

        string nameKata = "Ranura habilidad vacia";
        Sprite spriteKata = null;
        string nameArmas = "Ranura arma vacia";
        Sprite spriteWeapon = null;
        string strWeapon = "";

        UnityEngine.Events.UnityAction action = () =>
        {
            ch.weaponKataIndex = index;
            inventorySubMenu.Create();
        };

        if (ch.actualKata.equiped != null)
        {
            nameKata = ch.actualKata.equiped.nameDisplay;
            spriteKata = ch.actualKata.equiped.image;

            if(ch.actualKata.equiped.weapon != null)
            {
                nameArmas = ch.actualKata.equiped.weapon.nameDisplay;
                spriteWeapon = ch.actualKata.equiped.weapon.image;
                strWeapon = "Uses: " + ch.actualKata.equiped.weapon.current;

            }

            subMenu.AddComponent<ButtonA>().SetButtonA(nameArmas, spriteWeapon, "", action);
        }

        //subMenu.AddComponent<ButtonA>().SetButtonA(nameKata, spriteKata, "", action);
    }
}
