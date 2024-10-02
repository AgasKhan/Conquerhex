using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine.Events;

public class UIE_SlotButton : VisualElement
{
    [UnityEngine.Scripting.Preserve]
    public new class UxmlFactory : UxmlFactory<UIE_SlotButton, UxmlTraits> { }
    private VisualElement slotImage => this.Q<VisualElement>("slotImage");
    private Label slotText => this.Q<Label>("slotText");
    private VisualElement blocker => this.Q<VisualElement>("blocker");
    private Label blockerText => this.Q<Label>("blockerText");


    SlotItem slotItem;
    bool isBlocked = false;

    System.Action mainAct = default;
    System.Action enterMouseAct = default;
    System.Action leaveMouseAct = default;

    public void Init<T>(SlotItem _slotItem, UnityAction action) where T : ItemEquipable
    {
        VisualTreeAsset asset = UIE_MenusManager.treeAsset["SlotButton"];
        asset.CloneTree(this);

        slotItem = _slotItem;

        slotImage.style.backgroundImage = new StyleBackground(UIE_MenusManager.instance.GetImage<T>(slotItem.equiped));
        slotText.text = UIE_MenusManager.instance.GetText<T>(slotItem.equiped);

        if (slotItem.equiped?.GetType() == typeof(AbilityExtCast))
            slotImage.AddToClassList("abilityBorder");

        mainAct = () => action.Invoke();
        slotImage.RegisterCallback<ClickEvent>((clevent)=> mainAct.Invoke());

        RegisterCallback<MouseEnterEvent>((mouseEvent) => enterMouseAct.Invoke());

        RegisterCallback<MouseLeaveEvent>((mouseEvent) => leaveMouseAct.Invoke());

        InitTooltip();
    }
    
    public void Init<T>(Sprite image, string text, UnityAction action) where T : ItemEquipable
    {
        VisualTreeAsset asset = UIE_MenusManager.treeAsset["SlotButton"];
        asset.CloneTree(this);

        //Debug.Log("slotImage is null = " + (slotImage == null) + "\nstyle is null= " + (slotImage.style == null) + "\nbackgroundImage is null = " + (slotImage.style.backgroundImage == null)+ "\nSended image is null = "+(image==null));
        slotImage.style.backgroundImage = new StyleBackground(image);
        slotText.text = text;
        mainAct = ()=> action.Invoke();

        if (typeof(T) == typeof(AbilityExtCast))
        {
            slotImage.AddToClassList("abilityBorder");
        }

        slotImage.RegisterCallback<ClickEvent>((clevent) => mainAct.Invoke());

        RegisterCallback<MouseEnterEvent>((mouseEvent) => enterMouseAct.Invoke());

        RegisterCallback<MouseLeaveEvent>((mouseEvent) => leaveMouseAct.Invoke());
    }
    

    void InitTooltip()
    {
        ItemEquipable aux = slotItem.equiped;

        string _title;
        string _content;

        if (aux != null)
        {
            _title = aux.nameDisplay;
            _content = aux.GetItemBase().GetTooltip();
            
        }
        else if (slotItem.GetSlotType() == typeof(MeleeWeapon))
        {
            _title = "Arma";
            _content = "Herramienta usada tanto para atacar como para recolectar recursos\n\n" + "Primer ataque de combo".RichText("color", "#c9ba5d");
        }
        else
        {
            _title = "Habilidad";
            _content = "Utilizas la energía de tu alrededor para materializarla en daño";
        }

        AddEnterMouseEvent(() =>
        {
            if (!isBlocked)
                UIE_MenusManager.instance.SetTooltipTimer(_title, _content, slotItem.GetSlotType().ToString() + slotItem.indexSlot);
        });

        AddLeaveMouseEvent(()=> UIE_MenusManager.instance.StartHideTooltip(default));

    }

    public void AddEnterMouseEvent(System.Action _action)
    {
        enterMouseAct += _action;
    }
    public void AddLeaveMouseEvent(System.Action _action)
    {
        leaveMouseAct += _action;
    }

    public void Block(bool _condition)
    {
        isBlocked = _condition;

        if (_condition)
        {
            blocker.ShowInUIE();
            slotText.HideInUIE();
        }
        else
            blocker.HideInUIE();
    }
    public void Block(string _text)
    {
        blockerText.text = _text;
        Block(true);
    }

    public UIE_SlotButton() { }
}