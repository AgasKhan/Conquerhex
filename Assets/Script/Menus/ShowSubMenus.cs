using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShowSubMenus : CreateBodySubMenu
{
    [SerializeField]
    Sprite sprite;

    [SerializeField]
    string text;

    protected override void InternalCreate()
    {
        subMenu.CreateSection(0, 4);

            subMenu.AddComponent<DetailsWindow>().SetTexts("Titulo", text);

        subMenu.CreateSection(0, 8);

            subMenu.AddComponent<DetailsWindow>().SetImage(sprite);
    }


    public void Create(Sprite sprite, string text)
    {
        this.sprite = sprite;
        this.text = text;

        Create();
    }
}
