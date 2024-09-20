using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class UIE_CombosButton : VisualElement
{
    [UnityEngine.Scripting.Preserve]
    public new class UxmlFactory : UxmlFactory<UIE_CombosButton, UxmlTraits> { }

    public int indexSlot = -1;

    private VisualElement equipOrAbilityCombo => this.Q<VisualElement>("equipOrAbilityCombo");
    private VisualElement kataCombo => this.Q<VisualElement>("kataCombo");


    private VisualElement abilityButton => this.Q<VisualElement>("abilityButton");
    private VisualElement abilityImage => this.Q<VisualElement>("abilityImage");
    private Label abilityText => this.Q<Label>("abilityText");

    private VisualElement kataButton => this.Q<VisualElement>("kataButton");
    private VisualElement kataImage => this.Q<VisualElement>("kataImage");
    private Label kataText => this.Q<Label>("kataText");
    private VisualElement weaponButton => this.Q<VisualElement>("weaponButton");

    private VisualElement blockerCombo => this.Q<VisualElement>("blockerCombo");
    //private VisualElement blockerKata => this.Q<VisualElement>("blockerKata");

    private Label blockerText => this.Q<Label>("blockerText");

    UnityAction mainAct;
    UnityAction auxAct;

    bool isBlocked = false;
    void HideKata()
    {
        kataCombo.HideInUIE();
    }
    void HideAbility()
    {
        equipOrAbilityCombo.HideInUIE();
    }
    public void SetEquipOrAbility(Sprite image, string text, UnityAction action, System.Type _type)
    {
        equipOrAbilityCombo.ShowInUIE();
        HideKata();

        abilityImage.style.backgroundImage = new StyleBackground(image);
        abilityText.text = text;

        mainAct = action;
        
        if (_type == typeof(AbilityExtCast))
        {
            abilityButton.AddToClassList("abilityBorder");
        }
        else
        {
            abilityButton.RemoveFromClassList("abilityBorder");
        }
        
        abilityButton.RegisterCallback<ClickEvent>(mainButtonEvent);
    }

    public void SetKata(Sprite image, string text, UnityAction kataAction, Sprite weaponImage, UnityAction weaponAction)
    {
        kataCombo.ShowInUIE();
        HideAbility();

        kataImage.style.backgroundImage = new StyleBackground(image);
        kataText.text = text;

        mainAct = kataAction;
        auxAct = weaponAction;

        kataButton.AddToClassList("kataBorder");

        kataButton.RegisterCallback<ClickEvent>(mainButtonEvent);

        weaponButton.style.backgroundImage = new StyleBackground(weaponImage);
        weaponButton.RegisterCallback<ClickEvent>(auxButtonEvent);
    }

    public void Init(int slotIndex)
    {
        VisualTreeAsset asset = UIE_MenusManager.treeAsset["ComboButton"];
        asset.CloneTree(this);
        
        indexSlot = slotIndex;
    }

    public void InitTooltip(string _title, string _content, Sprite _sprite)
    {
        RegisterCallback<MouseEnterEvent>((mouseEvent) =>
        {
            if(!isBlocked)
                UIE_MenusManager.instance.SetTooltipTimer(_title, _content, _sprite);
        });

        RegisterCallback<MouseLeaveEvent>((mouseEvent) =>
        {
            UIE_MenusManager.instance.StartHideTooltip(mouseEvent);
        });
    }

    public void Block(bool _condition)
    {
        isBlocked = _condition;
        if (_condition)
            blockerCombo.ShowInUIE();
        else
            blockerCombo.HideInUIE();
    }
    public void Block(string _text)
    {
        blockerText.text = _text;
        Block(true);
    }

    void mainButtonEvent(ClickEvent clEvent)
    {
        //abilityButton.AddToClassList("slotButtonClicked");
        //abilityText.AddToClassList("slotTextClicked");

        mainAct.Invoke();
    }
    void auxButtonEvent(ClickEvent clEvent)
    {
        //kataButton.AddToClassList("slotButtonClicked");
        //kataText.AddToClassList("slotTextClicked");

        auxAct.Invoke();
    }

    public UIE_CombosButton() { }
}
