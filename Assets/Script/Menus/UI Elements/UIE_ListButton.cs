using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine.Events;

public class UIE_ListButton : VisualElement
{
    [UnityEngine.Scripting.Preserve]
    public new class UxmlFactory : UxmlFactory<UIE_ListButton, UxmlTraits> { }
    private VisualElement itemImage => this.Query<VisualElement>("ItemImage").First();
    private Label itemName => this.Query<Label>("ItemName").First();
    private Label typeItem => this.Query<Label>("TypeText").First();
    private Label specialityItem => this.Q<Label>("SpecialityText");
    private Label equipText => this.Q<Label>("equipText");
    private VisualElement changeButton => this.Q<VisualElement>("ChangeButton");
    private VisualElement buttonContainer => this.Q<VisualElement>("buttonContainer");
    private VisualElement mainButton => this.Q<VisualElement>("MainButton");

    System.Action mainAct;
    System.Action copyAction;

    System.Action changeAction;
    System.Action changeActCopy;

    System.Action hoverAct;

    public void Init(Sprite _itemImage, string _itemName, string _typeItem, string _specialityItem, System.Action _mainAction)
    {
        Init();
        Set(_itemImage, _itemName, _typeItem, _specialityItem, _mainAction);
    }

    public void Init()
    {
        VisualTreeAsset asset = UIE_MenusManager.treeAsset["ListItemButton"];
        asset.CloneTree(this);

        mainButton.RegisterCallback<ClickEvent>((clEvent) => mainAct?.Invoke());
        mainButton.RegisterCallback<MouseEnterEvent>((clEvent) => hoverAct.Invoke());
    }

    public void Set(Sprite _itemImage, string _itemName, string _typeItem, string _specialityItem, System.Action _mainAction)
    {
        itemImage.style.backgroundImage = new StyleBackground(_itemImage);
        itemName.text = _itemName;
        typeItem.text = _typeItem;
        specialityItem.text = _specialityItem;

        mainAct = _mainAction;
        copyAction = _mainAction;
    }

    public void SetHoverAction(System.Action _action)
    {
        hoverAct = _action;
    }

    public void SetChangeButton(System.Action _changeAction)
    {
        changeAction = _changeAction;
        changeActCopy = _changeAction;
        changeButton.RegisterCallback<ClickEvent>((clEvent) => changeAction.Invoke());

        equipText.HideInUIE();
        changeButton.ShowInUIE();
    }

    public void SetEquipText(string _text)
    {
        equipText.text = _text;

        mainButton.RegisterCallback<MouseEnterEvent>((clEvent) => equipText.AddToClassList("listItemTextHover"));
        mainButton.RegisterCallback<MouseLeaveEvent>((clEvent) => equipText.RemoveFromClassList("listItemTextHover"));
    }

    public void InitOnlyName(Sprite _itemImage, string _itemName, System.Action _mainAction, System.Type _type)
    {
        Init();

        typeItem.HideInUIE();
        specialityItem.HideInUIE();
        buttonContainer.HideInUIE();

        if (_type == typeof(WeaponKata))
        {
            itemImage.AddToClassList("kataBorder");
        }
        else if(_type == typeof(AbilityExtCast))
        {
            itemImage.AddToClassList("abilityBorder");
        }/*
        else if(_itemName != "Desequipar" && _type != typeof(MeleeWeapon))
        {
            itemImage.AddToClassList("abilityBorder");
        }*/

        mainAct = _mainAction;
        copyAction = _mainAction;

        itemImage.style.backgroundImage = new StyleBackground(_itemImage);
        itemName.text = _itemName;
    }
    public void InitOnlyName<T>(ItemEquipable _item, System.Action _mainAction, System.Type _type) where T:ItemEquipable
    {
        Init();

        typeItem.HideInUIE();
        specialityItem.HideInUIE();
        buttonContainer.HideInUIE();

        if (typeof(T) == typeof(WeaponKata))
            itemImage.AddToClassList("kataBorder");
        else if (typeof(T) == typeof(AbilityExtCast))
            itemImage.AddToClassList("abilityBorder");

        itemImage.style.backgroundImage = new StyleBackground(UIE_MenusManager.instance.GetImage<T>(_item));

        mainAct = _mainAction;
        copyAction = _mainAction;

        if (_item != null)
            itemName.text = _item.nameDisplay;
        else
            itemName.text = "Desequipar";
    }

    public void Enable()
    {
        mainAct = copyAction;
        mainButton.RemoveFromClassList("itemSelected");
        mainButton.AddToClassList("ListItemButton");
        SetEquipText("Equipar");
    }
    public void Disable()
    {
        mainAct = () => { };
        mainButton.AddToClassList("itemSelected");
        mainButton.RemoveFromClassList("ListItemButton");
        SetEquipText("");
    }

    public void EnableChange(string _text)
    {
        changeAction = changeActCopy;
        changeButton.RemoveFromClassList("changeButtonDisable");
        changeButton.AddToClassList("changeButton");

        (changeButton as Button).text = _text;
    }
    public void DisableChange()
    {
        changeAction = () => { };
        changeButton.AddToClassList("changeButtonDisable");
        changeButton.RemoveFromClassList("changeButton");
    }

    public UIE_ListButton() { }
}
