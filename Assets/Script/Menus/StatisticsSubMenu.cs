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
        ch.attack.weaponKataIndex = index;

        string nameKata = "Ranura habilidad vacia";
        Sprite spriteKata = null;
        string nameArmas = "Ranura arma vacia";
        Sprite spriteWeapon = null;
        string strWeapon = "";

        UnityEngine.Events.UnityAction action = () =>
        {
            ch.attack.weaponKataIndex = index;
            inventorySubMenu.Create();
        };

        if (ch.attack.actualKata.equiped != null)
        {
            nameKata = ch.attack.actualKata.equiped.nameDisplay;
            spriteKata = ch.attack.actualKata.equiped.image;

            if(ch.attack.actualKata.equiped.weaponEnabled != null)
            {
                nameArmas = ch.attack.actualKata.equiped.weaponEnabled.nameDisplay;
                spriteWeapon = ch.attack.actualKata.equiped.weaponEnabled.image;
                strWeapon = "Uses: " + ch.attack.actualKata.equiped.weaponEnabled.current;

            }

            subMenu.AddComponent<ButtonA>().SetButtonA(nameArmas, spriteWeapon, "", action);
        }

        //subMenu.AddComponent<ButtonA>().SetButtonA(nameKata, spriteKata, "", action);
    }
}
