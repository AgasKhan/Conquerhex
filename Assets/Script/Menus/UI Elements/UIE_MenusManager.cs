using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class UIE_MenusManager : SingletonMono<UIE_MenusManager>
{
    public string EquipItemMenu = "EquipItem_UIDoc";
    public string EquipmentMenu = "Equipment_UIDoc";
    public string CombosMenu = "Combos_UIDoc";
    public string CraftMenu = "Craft_UIDoc";

    public Pictionarys<string, UIE_BaseMenu> menuList = new Pictionarys<string, UIE_BaseMenu>();
    public static Dictionary<string, VisualTreeAsset> treeAsset = new Dictionary<string, VisualTreeAsset>();

    public bool isInMenu = false;
    public float timeToTooltip = 1.1f;
    public float timeToLeaveTooltip = 0.2f;

    public TimedAction tooltipTimer;
    public TimedAction tooltipLeaveTimer;

    public Sprite defaultWeaponImage;
    public string defaultWeaponText;

    public Sprite defaultAbilityImage;
    public string defaultAbilityText;

    public Sprite defaultKataImage;
    public string defaultKataText;

    public Sprite defaultNoFilterImage;
    public Sprite oldDefaultWeaponImage;
    public Sprite oldDefaultAbilityImage;
    public Sprite oldDefaultKataImage;

    public Pictionarys<string, string> shortCuts = new Pictionarys<string, string>();

    string currentMenu = "";
    string lastMenu;

    string tooltTile = "";
    string tooltContent = "";
    Sprite tooltSprite = null;
    string tooltAuxText = "";

    public AnimationClip idleAnim;

    //HARDCODEO-----------------------------------
    public AnimationClip bowAnim;

    public void BowCastAnim()
    {
        GameManager.instance.playerCharacter.GetInContainer<AnimatorController>().ChangeActionAnimation(bowAnim);
    }
    //HARDCODEO-----------------------------------


    public AnimationInfo showWeapon;

    EventControllerMediator escapeEventMediator;

    protected override void Awake()
    {
        base.Awake();
        if(treeAsset.Count == 0)
            LoadSystem.AddPostLoadCorutine(ChargeResources);

        tooltipTimer = TimersManager.Create(timeToTooltip, ShowTooltip);
        tooltipTimer.Stop();
        tooltipTimer.SetUnscaled(true);

        tooltipLeaveTimer = TimersManager.Create(timeToLeaveTooltip, HideTooltip);
        tooltipLeaveTimer.Stop();
        tooltipLeaveTimer.SetUnscaled(true);

        escapeEventMediator = new EventControllerMediator();
        escapeEventMediator.eventDown += EscapeEventMediator_eventDown;

        VirtualControllers.Escape.SuscribeController(escapeEventMediator);
    }

    private void EscapeEventMediator_eventDown(Vector2 arg1, float arg2)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
            return;

        MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(true)
               .SetWindow("", "¿Deseas volver al menu principal?")
               .AddButton("Si", () => GameManager.instance.Load("MainMenu"))
               .AddButton("No", () => { MenuManager.instance.modulesMenu.ObtainMenu<PopUp>(false).SetActiveGameObject(false); });

        VirtualControllers.Escape.SuscribeController(escapeEventMediator);
    }

    void ShowTooltip()
    {
        tooltipLeaveTimer.Stop();
        menuList[currentMenu].tooltip.SetParams(tooltTile, tooltContent, tooltSprite, tooltAuxText);
        menuList[currentMenu].tooltip.BringToFront();

        var mousePosition = Input.mousePosition;
        Vector2 mousePositionCorrected = new Vector2(mousePosition.x, Screen.height - mousePosition.y);
        mousePositionCorrected = RuntimePanelUtils.ScreenToPanel(menuList[currentMenu].ui.panel, mousePositionCorrected);

        menuList[currentMenu].tooltip.style.top = mousePositionCorrected.y;
        menuList[currentMenu].tooltip.style.left = mousePositionCorrected.x;
    }
    
    public void SetTooltipTimer(string _title, string _content, string _keyAuxText)
    {
        tooltipLeaveTimer.Stop();
        tooltipTimer.Reset();

        tooltTile = _title;
        tooltContent = _content;

        if (_keyAuxText != "")
            tooltAuxText = shortCuts[_keyAuxText];
        else
            tooltAuxText = "";
    }

    public string GetAuxTooltipText(string _key)
    {
        return shortCuts[_key];
    }

    public void StartHideTooltip(MouseLeaveEvent mouseEvent)
    {
        tooltipTimer.Stop();
        tooltipLeaveTimer.Reset();
    }

    public void HideTooltip()
    {
        tooltipLeaveTimer.Stop();
        menuList[currentMenu].tooltip.HideTooltip();
    }

    public Sprite GetImage<T>(ItemEquipable itemEquiped) where T : ItemEquipable
    {
        if (itemEquiped != null)
            return itemEquiped.image;
        else if (typeof(T) == typeof(MeleeWeapon))
            return defaultWeaponImage;
        else if (typeof(T) == typeof(WeaponKata))
            return defaultKataImage;
        else
            return defaultAbilityImage;
    }
    public Sprite GetOldImage<T>() where T : ItemEquipable
    {
        if (typeof(T) == typeof(MeleeWeapon))
            return oldDefaultWeaponImage;
        else if (typeof(T) == typeof(WeaponKata))
            return oldDefaultKataImage;
        else
            return oldDefaultAbilityImage;
    }

    public string GetText<T>(ItemEquipable itemEquiped) where T : ItemEquipable
    {
        if (itemEquiped != null)
            return itemEquiped.nameDisplay;
        else if (typeof(T) == typeof(MeleeWeapon))
            return defaultWeaponText;
        else if (typeof(T) == typeof(WeaponKata))
            return defaultKataText;
        else
            return defaultAbilityText;
    }

    void ChargeResources()
    {
        treeAsset.Clear();
        foreach (var item in LoadSystem.LoadAssets<VisualTreeAsset>("UI Toolkit/"))
        {
            treeAsset.Add(item.name, item);
        }
    }

    public UIE_BaseMenu GetCurrentMenu()
    {
        return menuList[currentMenu];
    }

    public void SwitchMenu(string menuToGo)
    {
        DisableMenu(currentMenu);
        EnableMenu(menuToGo);

        tooltipTimer.Stop();
        tooltipLeaveTimer.Stop();
        HideTooltip();
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

    public UIE_BaseMenu GetMenu(string name)
    {
        return menuList[name];
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
