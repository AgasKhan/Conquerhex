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

    public void Init(Sprite _kataImage, string _kataText, UnityAction _kataAction, Sprite _weaponImage, string _weaponText, UnityAction _weaponAction)
    {
        VisualTreeAsset asset = UIE_MenusManager.treeAsset["KataComboButtom"];
        asset.CloneTree(this);

        kataImage.style.backgroundImage = new StyleBackground(_kataImage);
        kataText.text = _kataText;
        auxAct = _kataAction;

        tooltip = _kataText + " descripción";

        var weaponButton = new UIE_SlotButton();

        weaponConteiner.Add(weaponButton);
        weaponButton.Init(_weaponImage, _weaponText, _weaponAction);

        kataButton.RegisterCallback<ClickEvent>(buttonEvent);
        //weaponButton.RegisterMouseEvents(EnterWeaponButton, LeaveWeaponButton);

    }

    UnityAction auxAct;

    void buttonEvent(ClickEvent clEvent)
    {
        kataImage.AddToClassList("slotButtonClicked");
        kataText.AddToClassList("slotTextClicked");

        auxAct.Invoke();
    }

    void EnterWeaponButton(MouseEnterEvent clEvent)
    {
        kataButton.SetEnabled(false);
    }
    void LeaveWeaponButton(MouseLeaveEvent clEvent)
    {

    }

    public UIE_KataButton() { }
}