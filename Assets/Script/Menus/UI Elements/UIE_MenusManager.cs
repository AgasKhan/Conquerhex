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
    public float timeToTooltip = 1.1f;

    public TimedAction tooltipTimer;

    string tooltTile = "";
    string tooltContent = "";
    Sprite tooltSprite = null;

    protected override void Awake()
    {
        base.Awake();
        if(treeAsset.Count == 0)
            LoadSystem.AddPostLoadCorutine(ChargeResources);

        tooltipTimer = TimersManager.Create(timeToTooltip, () => { ShowTooltip(); });
        tooltipTimer.Stop();
        tooltipTimer.SetUnscaled(true);
    }


    void ShowTooltip()
    {
        menuList[currentMenu].tooltip.SetParams(tooltTile, tooltContent, tooltSprite);
        menuList[currentMenu].tooltip.BringToFront();

        var mousePosition = Input.mousePosition;
        Vector2 mousePositionCorrected = new Vector2(mousePosition.x, Screen.height - mousePosition.y);
        mousePositionCorrected = RuntimePanelUtils.ScreenToPanel(menuList[currentMenu].ui.panel, mousePositionCorrected);

        menuList[currentMenu].tooltip.style.top = mousePositionCorrected.y;
        menuList[currentMenu].tooltip.style.left = mousePositionCorrected.x;
    }
    
    public void SetTooltipTimer(string _title, string _content, Sprite _image)
    {
        Debug.Log("ShowTooltip");

        tooltipTimer.Reset();

        tooltTile = _title;
        tooltContent = _content;
        tooltSprite = _image;
    }

    public void HideTooltip(MouseLeaveEvent mouseEvent)
    {
        tooltipTimer.Stop();
        menuList[currentMenu].tooltip.HideTooltip();
        Debug.Log("HideTooltip");
    }


    void ChargeResources()
    {
        treeAsset.Clear();
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
