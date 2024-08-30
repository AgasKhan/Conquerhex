using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIE_BaseMenu : MyScripts
{
    protected VisualElement ui;
    protected UIE_MenusManager manager;
    protected Character character;


    public event System.Action onEnableMenu;
    public event System.Action onDisableMenu;

    public event System.Action onClose;

    VisualElement closeButtons;

    protected override void Config()
    {
        MyAwakes += myAwake;
    }

    void myAwake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
        manager = GetComponentInParent<UIE_MenusManager>();
        closeButtons = ui.Q<VisualElement>("closeButton");

        closeButtons.RegisterCallback<ClickEvent>(TriggerOnClose);
        DisableMenu();
    }

    protected void TriggerOnClose(ClickEvent clickEvent)
    {
        onClose.Invoke();
    }

    public void EnableMenu()
    {
        ui.style.display = DisplayStyle.Flex;
        character = GameManager.instance.playerCharacter;
        
        onEnableMenu?.Invoke();
    }
    public void DisableMenu()
    {
        ui.style.display = DisplayStyle.None;

        onDisableMenu?.Invoke();
    }

}
