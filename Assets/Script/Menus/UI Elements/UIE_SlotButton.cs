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

    public void Init(Texture2D image, string text, UnityAction action)
    {
        VisualTreeAsset asset = UIE_MenusManager.treeAsset["SlotButton"];
        asset.CloneTree(this);

        Debug.Log("slotImage is null = " + (slotImage == null) + "\nstyle is null= " + (slotImage.style == null) + "\nbackgroundImage is null = " + (slotImage.style.backgroundImage == null)+ "\nSended image is null = "+(image==null));
        slotImage.style.backgroundImage = new StyleBackground(image);
        slotText.text = text;
        auxAct = action;

        slotImage.RegisterCallback<ClickEvent>(buttonEvent);
    }

    UnityAction auxAct;

    void buttonEvent(ClickEvent clEvent)
    {
        auxAct.Invoke();
    }

    public UIE_SlotButton() { }
}

/*

string text;
    
    public new class UxmlFactory : UxmlFactory<UIE_SlotButton, UxmlTraits> { }
    public new class UxmlTraits : BindableElement.UxmlTraits
    {
        UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription { name = "text", defaultValue = "Button" };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var bar = ve as UIE_SlotButton;

            bar.text = m_Text.GetValueFromBag(bag, cc);
        }
    }

*/