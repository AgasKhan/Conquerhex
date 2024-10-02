using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class UIE_BaseMenu : MyScripts
{
    public VisualElement ui;
    protected UIE_MenusManager manager;
    protected Character character;


    public event System.Action onEnableMenu;
    public event System.Action onDisableMenu;

    public event System.Action onClose;

    public UIE_Tooltip tooltip;

    VisualElement closeButton;

    protected override void Config()
    {
        MyAwakes += myAwake;
    }

    void myAwake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
        manager = GetComponentInParent<UIE_MenusManager>();
        closeButton = ui.Q<VisualElement>("closeButton");
        tooltip = ui.Q<UIE_Tooltip>("UIE_Tooltip");
        

        closeButton.RegisterCallback<ClickEvent>(TriggerOnClose);

        ui.style.display = DisplayStyle.None;
        ui.AddToClassList("opacityHidden");

        /*
        LoadSystem.AddPostLoadCorutine(() => 
        {
            if (tooltip == null)
            {
                tooltip = new UIE_Tooltip();
                var auxiliar = ui.Children().ToArray();
                auxiliar[0].Add(tooltip);

                tooltip.Init();
            }
        });*/
    }

    public void TriggerOnClose(ClickEvent clickEvent)
    {
        onClose.Invoke();
    }

    public void EnableMenu()
    {
        character = GameManager.instance.playerCharacter;

        ui.style.display = DisplayStyle.Flex;
        //var timeEnabler = TimersManager.Create(0f, 100f, 2, Mathf.Lerp, (save) => ui.style.opacity = save).AddToEnd(()=> ui.style.opacity = 100);

        //ui.style.opacity = 100;
        tooltip.Init();

        ui.RemoveFromClassList("opacityHidden");
        //ui.AddToClassList("opacityVisible");
        character.GetInContainer<AnimatorController>().SetScaleController(AnimatorUpdateMode.UnscaledTime);

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


        character.GetInContainer<AnimatorController>().SetScaleController();
        onDisableMenu?.Invoke();
    }

}
