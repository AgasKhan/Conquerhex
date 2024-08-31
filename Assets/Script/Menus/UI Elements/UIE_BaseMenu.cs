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

        ui.style.display = DisplayStyle.None;
        ui.AddToClassList("opacityHidden");
    }

    protected void TriggerOnClose(ClickEvent clickEvent)
    {
        onClose.Invoke();
    }

    public void EnableMenu()
    {
        character = GameManager.instance.playerCharacter;

        ui.style.display = DisplayStyle.Flex;
        //var timeEnabler = TimersManager.Create(0f, 100f, 2, Mathf.Lerp, (save) => ui.style.opacity = save).AddToEnd(()=> ui.style.opacity = 100);

        //ui.style.opacity = 100;

        ui.RemoveFromClassList("opacityHidden");
        //ui.AddToClassList("opacityVisible");

        onEnableMenu?.Invoke();
    }

    System.Func<float, float, float, float> UpdateOpacity()
    {
        return default;
    }
    public void DisableMenu()
    {
        //ui.style.opacity = 0;
        //var timeEnabler = TimersManager.Create(100f, 0f, 2, Mathf.Lerp, (save) => ui.style.opacity = save).AddToEnd(()=> ui.style.display = DisplayStyle.None);
        
        //ui.RemoveFromClassList("opacityVisible");
        ui.AddToClassList("opacityHidden");
        ui.style.display = DisplayStyle.None;

        //TimersManager.Create(0.2f, () => ui.style.display = DisplayStyle.None);
        

        onDisableMenu?.Invoke();
    }

}
