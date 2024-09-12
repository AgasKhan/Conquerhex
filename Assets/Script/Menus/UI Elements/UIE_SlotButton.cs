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

    public void Init(Sprite image, string text, UnityAction action)
    {
        VisualTreeAsset asset = UIE_MenusManager.treeAsset["SlotButton"];
        asset.CloneTree(this);

        //Debug.Log("slotImage is null = " + (slotImage == null) + "\nstyle is null= " + (slotImage.style == null) + "\nbackgroundImage is null = " + (slotImage.style.backgroundImage == null)+ "\nSended image is null = "+(image==null));
        slotImage.style.backgroundImage = new StyleBackground(image);
        slotText.text = text;
        auxAct = action;

        slotImage.RegisterCallback<ClickEvent>(buttonEvent);
    }

    public void InitTooltip(string _title, string _content, Sprite _sprite)
    {
        RegisterCallback<MouseEnterEvent>((mouseEvent) => UIE_MenusManager.instance.SetTooltipTimer(_title, _content, _sprite));
        RegisterCallback<MouseLeaveEvent>(UIE_MenusManager.instance.HideTooltip);
    }

    public void Init(Item _item, UnityAction action)
    {
        VisualTreeAsset asset = UIE_MenusManager.treeAsset["SlotButton"];
        asset.CloneTree(this);

        slotImage.style.backgroundImage = new StyleBackground(_item.image);
        slotText.text = _item.nameDisplay;
        auxAct = action;

        slotImage.RegisterCallback<ClickEvent>(buttonEvent);

        RegisterCallback<MouseEnterEvent>((mouseEvent) => UIE_MenusManager.instance.SetTooltipTimer(_item.nameDisplay, _item.GetDetails().ToString(), _item.image));
        RegisterCallback<MouseLeaveEvent>(UIE_MenusManager.instance.HideTooltip);
    }


    UnityAction auxAct;

    void buttonEvent(ClickEvent clEvent)
    {
        slotImage.AddToClassList("slotButtonClicked");
        slotText.AddToClassList("slotTextClicked");

        auxAct.Invoke();
    }

    public void RegisterMouseEvents(EventCallback<MouseEnterEvent> onEnter, EventCallback<MouseLeaveEvent> onLeave)
    {
        slotImage.RegisterCallback<MouseEnterEvent>(onEnter);
        slotImage.RegisterCallback<MouseLeaveEvent>(onLeave);
    }


    public UIE_SlotButton() { }
}