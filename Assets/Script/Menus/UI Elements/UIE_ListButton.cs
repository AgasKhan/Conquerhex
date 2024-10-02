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

    System.Action<ClickEvent> mainAction;
    System.Action<ClickEvent> changeAction;

    System.Action mainAct;
    
    public void Init(Sprite _itemImage, string _itemName, string _typeItem, string _specialityItem, System.Action _mainAction)
    {
        Init();
        Set(_itemImage, _itemName, _typeItem, _specialityItem, _mainAction);
    }

    public void Init()
    {
        VisualTreeAsset asset = UIE_MenusManager.treeAsset["ListItemButton"];
        asset.CloneTree(this);
    }

    public void Set(Sprite _itemImage, string _itemName, string _typeItem, string _specialityItem, System.Action _mainAction)
    {
        itemImage.style.backgroundImage = new StyleBackground(_itemImage);
        itemName.text = _itemName;
        typeItem.text = _typeItem;
        specialityItem.text = _specialityItem;

        mainAct = _mainAction;
        RegisterCallback<ClickEvent>((clEvent) => mainAct.Invoke());
    }

    public void SetHoverAction(System.Action _action)
    {
        RegisterCallback<MouseEnterEvent>((clEvent) =>_action.Invoke());
    }

    public void SetEquipText(string _text)
    {
        equipText.text = _text;

        RegisterCallback<MouseEnterEvent>((clEvent) => equipText.AddToClassList("listItemTextHover"));
        RegisterCallback<MouseLeaveEvent>((clEvent) => equipText.RemoveFromClassList("listItemTextHover"));
    }

    public void InitOnlyName(Sprite _itemImage, string _itemName, System.Action _mainAction, System.Type _type)
    {
        VisualTreeAsset asset = UIE_MenusManager.treeAsset["ListItemButton"];
        asset.CloneTree(this);

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

        itemImage.style.backgroundImage = new StyleBackground(_itemImage);
        itemName.text = _itemName;

        RegisterCallback<ClickEvent>((clEvent) => _mainAction.Invoke());
    }
    public void InitOnlyName<T>(ItemEquipable _item, System.Action _mainAction, System.Type _type) where T:ItemEquipable
    {
        VisualTreeAsset asset = UIE_MenusManager.treeAsset["ListItemButton"];
        asset.CloneTree(this);

        typeItem.HideInUIE();
        specialityItem.HideInUIE();
        buttonContainer.HideInUIE();

        if (typeof(T) == typeof(WeaponKata))
            itemImage.AddToClassList("kataBorder");
        else if (typeof(T) == typeof(AbilityExtCast))
            itemImage.AddToClassList("abilityBorder");

        itemImage.style.backgroundImage = new StyleBackground(UIE_MenusManager.instance.GetImage<T>(_item));

        if (_item != null)
            itemName.text = _item.nameDisplay;
        else
            itemName.text = "Desequipar";

        RegisterCallback<ClickEvent>((clEvent) => _mainAction.Invoke());
    }

    public UIE_ListButton() { }
}
