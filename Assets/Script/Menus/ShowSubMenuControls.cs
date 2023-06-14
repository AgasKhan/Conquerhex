using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ShowSubMenuControls : CreateSubMenu
{
    [SerializeField]
    ShowDetails[] scriptObjects;

    DetailsWindow myDetailsWindow;
    DetailsWindow myDetailsWindow2;

    public override void Create()
    {
       subMenu.navbar.DestroyAll();
       subMenu.ClearBody();
       base.Create();
       
    }

    protected override void InternalCreate()
    {
        CreateNavBar((SubMenu) =>
        {

            foreach (var item in scriptObjects)
            {
                subMenu.AddNavBarButton(item.nameDisplay, () => SetControls(item));
            }

        });

        subMenu.CreateSection(0, 3);

            myDetailsWindow = subMenu.AddComponent<DetailsWindow>().SetActiveGameObject(true);


        subMenu.CreateSection(3, 6);
            
            myDetailsWindow2 = subMenu.AddComponent<DetailsWindow>().SetActiveGameObject(true);

        SetControls(scriptObjects[0]);

    }
    void SetControls(ShowDetails scriptObj)
    {
        subMenu.RetardedOn(myDetailsWindow.gameObject);
        subMenu.RetardedOn(myDetailsWindow2.gameObject);
        myDetailsWindow2.SetImage(scriptObj.image);
        myDetailsWindow.SetTexts(scriptObj.nameDisplay, scriptObj.GetDetails().ToString());
    }

}
