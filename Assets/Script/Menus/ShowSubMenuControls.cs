using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ShowSubMenuControls : CreateBodySubMenu
{
    [SerializeField]
    Sprite sprite;

    [SerializeField]
    string title;

    [SerializeField]
    [TextArea(3,6)]
    string text;

    [SerializeField]
    TextAnchor childAlignment = TextAnchor.UpperCenter;


    public override void Create()
    {
        subMenu.navbar.DestroyAll();
        subMenu.ClearBody();
        base.Create();
    }

    protected override void InternalCreate()
    {

        subMenu.CreateSection(0, 3).lastSectionAlign = childAlignment;
        
        subMenu.AddComponent<DetailsWindow>().SetTexts(title, text).SetAlignment(TMPro.TextAlignmentOptions.Top, TMPro.TextAlignmentOptions.TopJustified);
            

        subMenu.CreateSection(3, 6);
        
        
        //subMenu.AddComponent<DetailsWindow>().SetImage(sprite);
        subMenu.AddComponent<PopUp>().SetWindow("","");
        //subMenu.AddComponent<ButtonA>().AddButtonA(sprite,"Hola",()=> { }, text);

    }
}
