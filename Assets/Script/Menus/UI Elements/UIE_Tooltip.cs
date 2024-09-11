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

    public void Init()
    {
        VisualTreeAsset asset = UIE_MenusManager.treeAsset["Tooltip"];
        asset.CloneTree(this);
    }


    public void SetParams(string _title, string _content, Sprite _image)
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
    }

    public void HideTooltip()
    {
        tooltipContainer.HideInUIE();
        titlesContainer.HideInUIE();
        tooltipTitle.HideInUIE();
        tooltipDescription.HideInUIE();
        tooltipImage.HideInUIE();
    }

    public UIE_Tooltip() { }
}

public interface ITooltip
{
    public string toolTitle { get; set; }
    public string toolDescription { get; set; }
    public Sprite toolImagine { get; set; }

    //public void ShowTooltip(MouseEnterEvent mouseEvent);
}