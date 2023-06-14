using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShowSubMenuSettings : CreateSubMenu
{

    public override void Create()
    {
        subMenu.ClearBody();
        base.Create();
    }

    protected override void InternalCreate()
    {
        CreateNavBar((subMenus)=>
        {
            subMenus.AddNavBarButton("Sounds","Settings");
        
        });

        subMenu.CreateSection(0, 6);
        subMenu.AddComponent<DetailsWindow>().SetTexts("dddd","ssss");
    }
}
