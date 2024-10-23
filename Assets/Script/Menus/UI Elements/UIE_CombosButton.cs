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

    private Label blockerText => this.Q<Label>("blockerText");


    bool isBlocked = false;
    UnityAction mainAct;
    UnityAction auxAct;

    System.Action enterMouseAct;
    System.Action enterMouseDynAct;
    System.Action hoverMouseAct;
    System.Action leaveMouseAct;
    System.Action leaveMouseDynAct;


    System.Action enterWeaponMouseAct;
    System.Action leaveWeaponMouseAct;
    public void Init(int slotIndex)
    {
        VisualTreeAsset asset = UIE_MenusManager.treeAsset["ComboButton"];
        asset.CloneTree(this);

        indexSlot = slotIndex;

        RegisterCallback<MouseEnterEvent>((mouseEvent) => { enterMouseAct?.Invoke(); enterMouseDynAct?.Invoke(); });
        RegisterCallback<MouseOverEvent>((mouseEvent) => hoverMouseAct?.Invoke());
        RegisterCallback<MouseLeaveEvent>((mouseEvent) => { leaveMouseAct?.Invoke(); leaveMouseDynAct?.Invoke(); });

        weaponButton.RegisterCallback<MouseEnterEvent>((mouseEvent) => enterWeaponMouseAct?.Invoke());
        weaponButton.RegisterCallback<MouseLeaveEvent>((mouseEvent) => leaveWeaponMouseAct?.Invoke());
    }

    void HideKata()
    {
        kataCombo.HideInUIE();
    }
    void HideAbility()
    {
        equipOrAbilityCombo.HideInUIE();
    }

    public void SetHoverActs(System.Action _actionEnter, System.Action _actionLeave)
    {
        enterMouseDynAct = _actionEnter;
        leaveMouseDynAct = _actionLeave;
    }
    public void SetWeaponHoverActs(System.Action _actionEnter, System.Action _actionLeave, MeleeWeapon _weapon)
    {
        enterWeaponMouseAct = ()=> 
        { 
            _actionEnter?.Invoke(); 
            UIE_MenusManager.instance.SetTooltipTimer(_weapon?.nameDisplay, _weapon?.GetItemBase().GetTooltip(), ""); 
        };
        leaveWeaponMouseAct = ()=>
        { 
            _actionLeave.Invoke();
            UIE_MenusManager.instance.StartHideTooltip(default);
        };
    }

    public void SetEquipOrAbility(AbilityExtCast _ability, UnityAction _action)
    {
        equipOrAbilityCombo.ShowInUIE();
        HideKata();

        abilityImage.style.backgroundImage = new StyleBackground(UIE_MenusManager.instance.GetImage<AbilityExtCast>(_ability));
        abilityText.text = UIE_MenusManager.instance.GetText<AbilityExtCast>(_ability);
        mainAct = _action;

        if (_ability != null)
            abilityButton.AddToClassList("abilityBorder");
        else
            abilityButton.RemoveFromClassList("abilityBorder");

        abilityButton.RegisterCallback<ClickEvent>((clEvent) => mainAct.Invoke());

        InitTooltip(_ability);
    }

    public void SetKata(WeaponKata _kata, UnityAction _kataAction, UnityAction _weaponAction)
    {
        kataCombo.ShowInUIE();
        HideAbility();

        kataImage.style.backgroundImage = new StyleBackground(UIE_MenusManager.instance.GetImage<WeaponKata>(_kata));
        kataText.text = UIE_MenusManager.instance.GetText<WeaponKata>(_kata);
        mainAct = _kataAction;
        auxAct = _weaponAction;

        kataButton.AddToClassList("kataBorder");
        kataButton.RegisterCallback<ClickEvent>((clEvent)=> mainAct.Invoke());

        weaponButton.style.backgroundImage = new StyleBackground(UIE_MenusManager.instance.GetImage<MeleeWeapon>(_kata.Weapon));
        weaponButton.RegisterCallback<ClickEvent>((clEvent) => auxAct.Invoke());

        InitTooltip(_kata);
    }

    
    public void InitTooltip(ItemEquipable _item)
    {
        string _title;
        string _content;

        if (_item != null)
        {
            _title = _item.nameDisplay;
            _content = "Puedes intercambiar este movimiento por otra kata o habilidad";
        }
        else
        {
            _title = "Equipar combo";
            _content = "Puedes equiparte en este movimiento una kata o habilidad";
        }

        SetHoverMouseEvent(() =>
        {
            if (!isBlocked)
                UIE_MenusManager.instance.SetTooltipTimer(_title, _content, "");
        });

        SetLeaveMoususeEvent(() =>
        {
            UIE_MenusManager.instance.StartHideTooltip(default);
        });
    }

    public void SetEnterMouseEvent(System.Action _action)
    {
        enterMouseAct = _action;
    }
    public void SetHoverMouseEvent(System.Action _action)
    {
        hoverMouseAct = _action;
    }
    public void SetLeaveMoususeEvent(System.Action _action)
    {
        leaveMouseAct = _action;
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

    public UIE_CombosButton() { }
}
