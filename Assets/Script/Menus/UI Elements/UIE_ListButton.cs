using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class UIE_ListButton : VisualElement
{
    [UnityEngine.Scripting.Preserve]
    public new class UxmlFactory : UxmlFactory<UIE_ListButton, UxmlTraits> { }
    private VisualElement itemImage => this.Query<VisualElement>("ItemImage").First();
    private Label itemName => this.Query<Label>("ItemName").First();
    private Label typeItem => this.Query<Label>("TypeText").First();
    private Label specialityItem => this.Q<Label>("SpecialityText");
    private Label changeLabel => this.Q<Label>("ChangeButton");
    private VisualElement changeButton => this.Q<VisualElement>("ChangeButton");

    System.Action<ClickEvent> mainAction;
    System.Action<ClickEvent> changeAction;

    public void Init(Texture2D _itemImage, string _itemName, string _typeItem, string _specialityItem, bool changeButtonVisible, System.Action<ClickEvent> _mainAction, System.Action<ClickEvent> _changeAction, string changeText)
    {
        VisualTreeAsset asset = UIE_MenusManager.treeAsset["ListItemButton"];
        asset.CloneTree(this);
        /*
        Debug.Log("itemImage is null = " + (itemImage == null));
        Debug.Log("itemName is null = " + (itemName == null));
        Debug.Log("typeItem is null = " + (typeItem == null));
        Debug.Log("specialityItem is null = " + (specialityItem == null));
        Debug.Log("changeButton is null = " + (changeButton == null));
        */
        itemImage.style.backgroundImage = new StyleBackground(_itemImage);
        itemName.text = _itemName;
        typeItem.text = _typeItem;
        specialityItem.text = _specialityItem;

        changeButton.visible = changeButtonVisible;
        //changeLabel.text = changeText;

        mainAction = _mainAction;
        SetMainButtonAct();

        if(_changeAction!=null)
        {
            changeAction = _changeAction;
            SetChangeButtonAct();
        }
    }

    public void SetMainButtonAct()
    {
        this.RegisterCallback<ClickEvent>(myEvent);
    }

    public void SetChangeButtonAct()
    {
        changeButton.RegisterCallback<ClickEvent>(changeEvent);
    }

    void myEvent(ClickEvent clEvent)
    {
        mainAction?.Invoke(clEvent);
    }
    void changeEvent(ClickEvent clEvent)
    {
        changeAction?.Invoke(clEvent);
    }

    public UIE_ListButton() { }
}
