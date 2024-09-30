using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class UIE_Tooltip : VisualElement
{
    [UnityEngine.Scripting.Preserve]
    public new class UxmlFactory : UxmlFactory<UIE_Tooltip, UxmlTraits> { }

    private VisualElement tooltipContainer => this.Q<VisualElement>("tooltipContainer");
    private VisualElement titlesContainer => this.Q<VisualElement>("titlesContainer");
    private VisualElement tooltipImage => this.Q<VisualElement>("tooltipImage");
    private Label tooltipTitle => this.Q<Label>("tooltipTitle");
    private Label tooltipDescription => this.Q<Label>("tooltipDescription");
    private Label tooltipAuxText => this.Q<Label>("tooltipAuxText");

    public void Init()
    {
        VisualTreeAsset asset = UIE_MenusManager.treeAsset["Tooltip"];
        asset.CloneTree(this);

        RegisterCallback<MouseEnterEvent>((clEvent) => { UIE_MenusManager.instance.tooltipLeaveTimer.Stop(); });
        RegisterCallback<MouseLeaveEvent>((clEvent) => { UIE_MenusManager.instance.tooltipLeaveTimer.Reset(); });
    }


    public void SetParams(string _title, string _content, Sprite _image, string _auxText)
    {
        tooltipContainer.ShowInUIE();

        if(_title != "")
        {
            titlesContainer.ShowInUIE();
            tooltipTitle.ShowInUIE();
            tooltipTitle.text = _title;
        }

        if(_content != "")
        {
            tooltipDescription.ShowInUIE();
            tooltipDescription.text = _content;
        }

        if(_image != null)
        {
            titlesContainer.ShowInUIE();
            tooltipImage.ShowInUIE();
            tooltipImage.style.backgroundImage = new StyleBackground(_image);
        }

        if(_auxText != "")
        {
            titlesContainer.ShowInUIE();
            tooltipAuxText.ShowInUIE();
            tooltipAuxText.text = _auxText;
        }
    }

    public void HideTooltip()
    {
        tooltipContainer.HideInUIE();
        titlesContainer.HideInUIE();
        tooltipTitle.HideInUIE();
        tooltipDescription.HideInUIE();
        tooltipImage.HideInUIE();
        tooltipAuxText.HideInUIE();
    }

    public UIE_Tooltip() { }
}