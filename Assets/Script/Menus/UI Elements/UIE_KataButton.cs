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
    private VisualElement blockerKata => this.Q<VisualElement>("blockerKata");
    private Label blockerText => this.Q<Label>("blockerText");

    bool isOnWeapon = false;
    bool isBlocked = false;
    SlotItem<WeaponKata> slotItem;

    System.Action mainAct = default;
    System.Action weaponAct = default;
    System.Action enterMouseAct = default;
    System.Action hoverMouseAct = default;
    System.Action leaveMouseAct = default;

    UIE_SlotButton weaponButton;
    public void Init(SlotItem<WeaponKata> _slotItem, UnityAction _action, UnityAction _weaponAction)
    {
        VisualTreeAsset asset = UIE_MenusManager.treeAsset["KataComboButtom"];
        asset.CloneTree(this);

        slotItem = _slotItem;
        mainAct = () => _action.Invoke();
        weaponAct = () => _weaponAction.Invoke();

        kataImage.style.backgroundImage = new StyleBackground(UIE_MenusManager.instance.GetImage<WeaponKata>(slotItem.equiped));
        kataText.text = UIE_MenusManager.instance.GetText<WeaponKata>(slotItem.equiped);

        if (slotItem.equiped != null)
            kataButton.AddToClassList("kataBorder");

        kataButton.RegisterCallback<ClickEvent>((clevent) => mainAct.Invoke());

        weaponButton = new UIE_SlotButton();

        weaponConteiner.Add(weaponButton);
        weaponButton.Init<MeleeWeapon>(UIE_MenusManager.instance.GetImage<MeleeWeapon>(slotItem.equiped?.Weapon), UIE_MenusManager.instance.GetText<MeleeWeapon>(slotItem.equiped?.Weapon), ()=> weaponAct.Invoke());

        RegisterCallback<MouseEnterEvent>((mouseEvent) => enterMouseAct?.Invoke());
        RegisterCallback<MouseOverEvent>((mouseEvent) => hoverMouseAct?.Invoke());
        RegisterCallback<MouseLeaveEvent>((mouseEvent) => leaveMouseAct?.Invoke());

        weaponButton.HideBackImage();

        InitTooltip();
    }

    public void Init(Sprite _kataImage, string _kataText, UnityAction _kataAction, Sprite _weaponImage, string _weaponText, UnityAction _weaponAction)
    {
        VisualTreeAsset asset = UIE_MenusManager.treeAsset["KataComboButtom"];
        asset.CloneTree(this);

        kataImage.style.backgroundImage = new StyleBackground(_kataImage);
        kataText.text = _kataText;
        auxAct = _kataAction;

        var weaponButton = new UIE_SlotButton();

        weaponConteiner.Add(weaponButton);
        weaponButton.Init<MeleeWeapon>(_weaponImage, _weaponText, _weaponAction);

        if(_kataText != "Equipar Kata")
            kataButton.AddToClassList("kataBorder");

        kataButton.RegisterCallback<ClickEvent>(buttonEvent);
        //weaponButton.RegisterMouseEvents(EnterWeaponButton, LeaveWeaponButton);

    }

    void InitTooltip()
    {
        WeaponKata aux = slotItem.equiped;
        string _title;
        string _content;

        if (aux != null)
        {
            _title = aux.nameDisplay;
            _content = aux.GetItemBase().GetTooltip();
        }
        else
        {
            _title = "Kata";
            _content = "Movimiento marcial\n\n" + "Requiere de equipar un arma en la casilla contigua".RichText("color", "#c9ba5d");
        }

        AddHoverMouseEvent(() =>
        {
            if (!isOnWeapon && !isBlocked)
                UIE_MenusManager.instance.SetTooltipTimer(_title, _content, slotItem.GetSlotType().ToString() + slotItem.indexSlot);
        });

        AddLeaveMoususeEvent(() =>UIE_MenusManager.instance.StartHideTooltip(default));

        MeleeWeapon weapon = slotItem.equiped?.Weapon;
        string _titleWeapon;
        string _contentWeapon;

        if (weapon != null)
        {
            _titleWeapon = weapon.nameDisplay;
            _contentWeapon = weapon.GetItemBase().GetTooltip();
        }
        else
        {
            _titleWeapon = "Arma de Kata";
            _contentWeapon = "Herramienta vital para efectuar el daño de la kata\n\n" + "Requiere de equipar una kata en la casilla contigua".RichText("color", "#c9ba5d");
        }

        weaponButton.AddEnterMouseEvent(() =>
        {
            isOnWeapon = true;
            if (!isBlocked)
                UIE_MenusManager.instance.SetTooltipTimer(_titleWeapon, _contentWeapon, slotItem.GetSlotType().ToString() + slotItem.indexSlot);
        });

        weaponButton.AddLeaveMouseEvent(() =>
        {
            isOnWeapon = false;
            UIE_MenusManager.instance.StartHideTooltip(default);
        });
    }
    /*
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
    */
    public void AddEnterMouseEvent(System.Action _action)
    {
        enterMouseAct += _action;
    }
    public void AddHoverMouseEvent(System.Action _action)
    {
        hoverMouseAct += _action;
    }
    public void AddLeaveMoususeEvent(System.Action _action)
    {
        leaveMouseAct += _action;
    }


    public void SetAuxText(string _auxText)
    {
        UIE_MenusManager.instance.GetAuxTooltipText(_auxText);
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
            blockerKata.ShowInUIE();
        else
            blockerKata.HideInUIE();
    }
    public void Block(string _text)
    {
        blockerText.text = _text;
        Block(true);
    }

    public void FreezzeButton()
    {
        kataButton.RemoveFromClassList("kataButton");
        kataButton.AddToClassList("kataButtonNoHover");
        AddToClassList("halfScale");

        weaponButton.FreezzeButton();
    }

    public void Enable()
    {
        kataButton.pickingMode = PickingMode.Position;
        weaponButton.pickingMode = PickingMode.Position;
    }
    public void Disable()
    {
        kataButton.pickingMode = PickingMode.Ignore;
        weaponButton.pickingMode = PickingMode.Ignore;
    }

    public UIE_KataButton() { }
}