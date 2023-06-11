using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySubMenu : SubMenus
{


    void BodyCreate()
    {
        CreateSection(0, 3);
            var aux = AddComponent<MenuList>();

        CreateSection(3, 6);
            AddComponent<DetailsWindow>().SetTexts("", "").SetImage();
    }

}
