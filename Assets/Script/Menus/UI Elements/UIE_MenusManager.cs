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

    public static Dictionary<string, VisualTreeAsset> treeAsset = new Dictionary<string, VisualTreeAsset>();

    protected override void Awake()
    {
        base.Awake();
        if(treeAsset.Count == 0)
            LoadSystem.AddPostLoadCorutine(ChargeResources);
    }

    void ChargeResources()
    {
        foreach (var item in LoadSystem.LoadAssets<VisualTreeAsset>("UI Toolkit/"))
        {
            treeAsset.Add(item.name, item);
        }
    }


    public void SwitchMenu(string menuToGo)
    {
        DisableMenu(currentMenu);
        EnableMenu(menuToGo);
    }

    public void EnableMenu(string name)
    {
        GameManager.instance.Menu(true);
        currentMenu = name;
        menuList[name].EnableMenu();
    }

    public void DisableMenu(string name)
    {
        GameManager.instance.Menu(false);
        lastMenu = name;
        menuList[name].DisableMenu();
    }

    public void BackLastMenu()
    {
        SwitchMenu(lastMenu);
    }
}
