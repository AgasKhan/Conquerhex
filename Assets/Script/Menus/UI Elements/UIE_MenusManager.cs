using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIE_MenusManager : SingletonMono<UIE_MenusManager>
{
    public string EquipItemMenu = "EquipItem_UIDoc";
    public string EquipmentMenu = "Equipment_UIDoc";

    public Pictionarys<string, UIE_BaseMenu> menuList = new Pictionarys<string, UIE_BaseMenu>();

    string currentMenu = "";
    string lastMenu;

    public void SwitchMenu(string menuToGo)
    {
        DisableMenu(currentMenu);
        EnableMenu(menuToGo);
    }

    public void EnableMenu(string name)
    {
        currentMenu = name;
        menuList[name].EnableMenu();
    }

    public void DisableMenu(string name)
    {
        lastMenu = name;
        menuList[name].DisableMenu();
    }

    public void BackLastMenu()
    {
        SwitchMenu(lastMenu);
    }
}
