using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class UIE_CombosButton : VisualElement
{
    [UnityEngine.Scripting.Preserve]
    public new class UxmlFactory : UxmlFactory<UIE_CombosButton, UxmlTraits> { }

    public int index = -1;

    private VisualElement equipOrAbilityCombo => this.Q<VisualElement>("equipOrAbilityCombo");
    private VisualElement kataCombo => this.Q<VisualElement>("kataCombo");


    private VisualElement abilityButton => this.Q<VisualElement>("abilityButton");
    private VisualElement abilityImage => this.Q<VisualElement>("abilityImage");
    private Label abilityText => this.Q<Label>("abilityText");

    private VisualElement kataButton => this.Q<VisualElement>("kataButton");
    private VisualElement kataImage => this.Q<VisualElement>("kataImage");
    private Label kataText => this.Q<Label>("kataText");
    private VisualElement weaponButton => this.Q<VisualElement>("weaponButton");

    UnityAction mainAct;
    UnityAction auxAct;

    public void SetEquipOrAbility(Sprite image, string text, UnityAction action)
    {
        equipOrAbilityCombo.RemoveFromClassList("displayHidden");
        abilityImage.style.backgroundImage = new StyleBackground(image);
        abilityText.text = text;

        mainAct = action;
        abilityButton.RegisterCallback<ClickEvent>(mainButtonEvent);
    }

    public void SetKata(Sprite image, string text, UnityAction kataAction, UnityAction weaponAction)
    {
        kataCombo.RemoveFromClassList("displayHidden");
        kataImage.style.backgroundImage = new StyleBackground(image);
        kataText.text = text;

        mainAct = kataAction;
        auxAct = weaponAction;

        kataButton.RegisterCallback<ClickEvent>(mainButtonEvent);
        weaponButton.RegisterCallback<ClickEvent>(auxButtonEvent);
    }

    public void Init()
    {
        VisualTreeAsset asset = UIE_MenusManager.treeAsset["ComboButton"];
        asset.CloneTree(this);
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

    void ResetButton()
    {
        abilityImage.RemoveFromClassList("slotButtonClicked");
        abilityText.RemoveFromClassList("slotTextClicked");
    }

    public UIE_CombosButton() { }
}
