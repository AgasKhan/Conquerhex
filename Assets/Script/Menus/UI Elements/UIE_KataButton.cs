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

    public void Init(SlotItem<WeaponKata> _slotItem, UnityAction action, UnityAction _weaponAction)
    {
        VisualTreeAsset asset = UIE_MenusManager.treeAsset["KataComboButtom"];
        asset.CloneTree(this);

        slotItem = _slotItem;

        kataImage.style.backgroundImage = new StyleBackground(UIE_MenusManager.instance.GetImage<WeaponKata>(slotItem.equiped));
        kataText.text = UIE_MenusManager.instance.GetText<WeaponKata>(slotItem.equiped);

        if (slotItem.equiped != null)
            kataButton.AddToClassList("kataBorder");

        kataButton.RegisterCallback<ClickEvent>((clevent) => action.Invoke());

        var weaponButton = new UIE_SlotButton();

        weaponConteiner.Add(weaponButton);
        weaponButton.Init<MeleeWeapon>(UIE_MenusManager.instance.GetImage<MeleeWeapon>(slotItem.equiped?.Weapon), UIE_MenusManager.instance.GetText<MeleeWeapon>(slotItem.equiped?.Weapon), _weaponAction);

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

        RegisterCallback<MouseOverEvent>((mouseEvent) =>
        {
            if (!isOnWeapon && !isBlocked)
                UIE_MenusManager.instance.SetTooltipTimer(_title, _content, slotItem.GetSlotType().ToString() + slotItem.indexSlot);
        });
        RegisterCallback<MouseLeaveEvent>((mouseEvent) =>
        {
            UIE_MenusManager.instance.StartHideTooltip(mouseEvent);
        });

        MeleeWeapon weapon = slotItem.equiped?.Weapon;

        if (weapon != null)
        {
            _title = weapon.nameDisplay;
            _content = weapon.GetItemBase().GetTooltip();
        }
        else
        {
            _title = "Arma de Kata";
            _content = "Herramienta vital para efectuar el da�o de la kata\n\n" + "Requiere de equipar una kata en la casilla contigua".RichText("color", "#c9ba5d");
        }

        weaponConteiner.RegisterCallback<MouseEnterEvent>((mouseEvent) =>
        {
            isOnWeapon = true;
            if (!isBlocked)
                UIE_MenusManager.instance.SetTooltipTimer(_title, _content, slotItem.GetSlotType().ToString() + slotItem.indexSlot);
        });

        weaponConteiner.RegisterCallback<MouseLeaveEvent>((mouseEvent) =>
        {
            isOnWeapon = false;
            UIE_MenusManager.instance.StartHideTooltip(mouseEvent);
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
    public UIE_KataButton() { }
}