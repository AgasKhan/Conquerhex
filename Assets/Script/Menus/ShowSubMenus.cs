using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ShowSubMenus : CreateBodySubMenu
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


    protected override void InternalCreate()
    {

        subMenu.CreateSection(0, 3).lastSectionAlign = childAlignment;
        
        subMenu.AddComponent<DetailsWindow>().SetTexts(title, text).SetAlignment(TMPro.TextAlignmentOptions.Top, TMPro.TextAlignmentOptions.TopJustified);
            

        subMenu.CreateSection(3, 6);
        
        subMenu.AddComponent<DetailsWindow>().SetImage(sprite);
        //subMenu.AddComponent<ButtonA>().AddButtonA(sprite,"Hola",()=> { }, text);

    }
}
