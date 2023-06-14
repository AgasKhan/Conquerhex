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

    public override void Create()
    {
        subMenu.ClearBody();
        base.Create();
    }

    protected override void InternalCreate()
    {
        CreateNavBar
        (
            (submenu) =>
            {
                submenu.AddNavBarButton("Statistics", Create).AddNavBarButton("Inventory", () => inventorySubMenu.Create());
            }
        );

        subMenu.CreateSection(0, 3);
        subMenu.AddComponent<DetailsWindow>().SetImage(character.bodyBase.image).SetTexts(character.bodyBase.nameDisplay, character.bodyBase.GetDetails().ToString());

        subMenu.CreateSection(3, 6);
        subMenu.CreateChildrenSection<ScrollRect>();

        CreateWeaponButtons(character);
        /*
        ItemIsNull(character.prin.weapon);
        ItemIsNull(character.sec.weapon);
        ItemIsNull(character.ter.weapon);
        */
    }

    void ItemIsNull(Item item)
    {
        if(item != null)
        {
            subMenu.AddComponent<ButtonA>().SetButtonA(item.nameDisplay, item.image, "Uses: " + ((MeleeWeapon)item).durability.current, null);
        }
    }

    void CreateWeaponButtons(Character charac)
    {
        if(charac.prin != null)
            ItemIsNull(charac.prin.weapon);
        if (charac.sec != null)
            ItemIsNull(charac.prin.weapon);
        if (charac.ter != null)
            ItemIsNull(charac.prin.weapon);
    }

}
