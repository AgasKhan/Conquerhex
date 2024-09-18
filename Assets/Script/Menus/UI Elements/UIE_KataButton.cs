using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class UIE_KataButton : VisualElement
{
    [UnityEngine.Scripting.Preserve]
    public new class UxmlFactory : UxmlFactory<UIE_KataButton, UxmlTraits> { }

    private VisualElement kataButton => this.Q<VisualElement>("kataButton");
    private VisualElement kataImage => this.Q<VisualElement>("kataImage");
    private Label kataText => this.Q<Label>("kataText");
    private VisualElement weaponConteiner => this.Q<VisualElement>("weaponContainer");
    private VisualElement blocker => this.Q<VisualElement>("blocker");
    private Label blockerText => this.Q<Label>("blockerText");

    bool isOnWeapon = false;
    bool isBlocked = false;
    public void Init(Sprite _kataImage, string _kataText, UnityAction _kataAction, Sprite _weaponImage, string _weaponText, UnityAction _weaponAction)
    {
        VisualTreeAsset asset = UIE_MenusManager.treeAsset["KataComboButtom"];
        asset.CloneTree(this);

        kataImage.style.backgroundImage = new StyleBackground(_kataImage);
        kataText.text = _kataText;
        auxAct = _kataAction;

        var weaponButton = new UIE_SlotButton();

        weaponConteiner.Add(weaponButton);
        weaponButton.Init(_weaponImage, _weaponText, _weaponAction, typeof(MeleeWeapon));

        if(_kataText != "Equipar Kata")
            kataButton.AddToClassList("kataBorder");

        kataButton.RegisterCallback<ClickEvent>(buttonEvent);
        //weaponButton.RegisterMouseEvents(EnterWeaponButton, LeaveWeaponButton);

    }
    
    public void InitTooltip(string _title, string _content, Sprite _sprite)
    {
        RegisterCallback<MouseOverEvent>((mouseEvent) =>
        {
            if(!isOnWeapon && !isBlocked)
                UIE_MenusManager.instance.SetTooltipTimer(_title, _content, _sprite);
        });
        RegisterCallback<MouseLeaveEvent>((mouseEvent) =>
        {
            UIE_MenusManager.instance.StartHideTooltip(mouseEvent);
        });
    }
    public void InitWeaponTooltip(string _title, string _content, Sprite _sprite)
    {
        weaponConteiner.RegisterCallback<MouseEnterEvent>((mouseEvent) =>
        {
            isOnWeapon = true;
            if (!isBlocked)
                UIE_MenusManager.instance.SetTooltipTimer(_title, _content, _sprite);
        });

        weaponConteiner.RegisterCallback<MouseLeaveEvent>((mouseEvent) =>
        {
            isOnWeapon = false;
            UIE_MenusManager.instance.StartHideTooltip(mouseEvent);
        });
    }

    UnityAction auxAct;

    void buttonEvent(ClickEvent clEvent)
    {
        kataImage.AddToClassList("slotButtonClicked");
        kataText.AddToClassList("slotTextClicked");

        auxAct.Invoke();
    }

    public void Block(bool _condition)
    {
        isBlocked = _condition;
        if (_condition)
            blocker.ShowInUIE();
        else
            blocker.HideInUIE();
    }
    public void Block(string _text)
    {
        blockerText.text = _text;
        Block(true);
    }
    public UIE_KataButton() { }
}