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
        subMenu.AddComponent<DetailsWindow>().SetImage(character.bodyBase.image).SetTexts(character.bodyBase.nameDisplay, character.bodyBase.GetDetails().ToString());

        subMenu.CreateSection(3, 6);
        subMenu.CreateChildrenSection<ScrollRect>();

        CreateWeaponButtons(character);
    }

    void CreateWeaponButtons(Character charac)
    {
        CreateWeaponButtons(charac.prin);
        CreateWeaponButtons(charac.sec);
        CreateWeaponButtons(charac.ter);
    }


    void CreateWeaponButtons(WeaponKata kata)
    {
        if (kata != null && kata.weapon!=null)
        {   
            subMenu.AddComponent<ButtonA>().SetButtonA(kata.weapon.nameDisplay, kata.weapon.image, "Uses: " + kata.weapon.durability.current, null);       
        }
    }

}
