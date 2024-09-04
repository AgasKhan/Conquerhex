using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIE_MenusManager : SingletonMono<UIE_MenusManager>
{
    public string EquipItemMenu = "EquipItem_UIDoc";
    public string EquipmentMenu = "Equipment_UIDoc";
    public string CombosMenu = "Combos_UIDoc";

    public Pictionarys<string, UIE_BaseMenu> menuList = new Pictionarys<string, UIE_BaseMenu>();

    string currentMenu = "";
    string lastMenu;

    public static Dictionary<string, VisualTreeAsset> treeAsset = new Dictionary<string, VisualTreeAsset>();

    public bool isInMenu = false;
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
        isInMenu = true;
        GameManager.instance.Menu(true);
        currentMenu = name;
        menuList[name].EnableMenu();
    }

    public void DisableMenu(string name)
    {
        isInMenu = false;
        GameManager.instance.Menu(false);
        lastMenu = name;
        menuList[name].DisableMenu();
    }

    public void TriggerOnClose()
    {
        menuList[currentMenu].TriggerOnClose(default);
    }

    public void BackLastMenu()
    {
        SwitchMenu(lastMenu);
    }
}
