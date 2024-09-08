using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UIE_CombosMenu : UIE_Equipment
{
    VisualElement comboButtons;
    VisualElement listItems;

    VisualElement noClickPanel;

    List<UIE_CombosButton> comboButtonsList = new List<UIE_CombosButton>();
    List<UIE_ListButton> listEquipableItems = new List<UIE_ListButton>();

    VisualElement detailsWindow;
    VisualElement itemImage;
    Label itemName;
    Label itemDescription;
    Label itemDetails;

    VisualElement equipmentButton;

    protected override void Config()
    {
        base.Config();
        MyAwakes += myAwake;
    }

    void myAwake()
    {
        onEnableMenu += myEnableMenu;
        onDisableMenu += myOnDisable;

        comboButtons = ui.Q<VisualElement>("comboButtons");
        listItems = ui.Q<VisualElement>("listItems");
        noClickPanel = ui.Q<VisualElement>("noClickPanel");

        detailsWindow = ui.Q<VisualElement>("detailsWindow");
        itemImage = ui.Q<VisualElement>("itemImage");
        itemName = ui.Q<Label>("itemName");
        itemDescription = ui.Q<Label>("itemDescription");
        itemDetails = ui.Q<Label>("itemDetails");

        equipmentButton = ui.Q<VisualElement>("equipmentButton");
        
        noClickPanel.RegisterCallback<ClickEvent>((cleEvent)=> HiddeItemList());
        equipmentButton.RegisterCallback<ClickEvent>((clEvent) => manager.SwitchMenu(UIE_MenusManager.instance.EquipmentMenu));
    }

    private void myOnDisable()
    {
        HiddeItemList();
        comboButtonsList.Clear();
    }

    void SetComboButton<T>(T auxAbility, int indexSlot) where T:Ability
    {
        var aux = comboButtonsList[indexSlot];

        if (auxAbility is WeaponKata)
        {
            aux.SetKata
                (
                    GetImage(auxAbility),
                    GetText(auxAbility),
                    () => GetAction(aux.index),
                    GetImage((auxAbility as WeaponKata).Weapon),
                    () => GetWeaponAction(aux.index, auxAbility as WeaponKata)
                );
        }
        else
        {
            aux.SetEquipOrAbility(GetImage(auxAbility), GetText(auxAbility), () => GetAction(aux.index));
        }
    }

    void SetComboButton(UIE_CombosButton _button)
    {
        var aux = _button.index;
        var auxAbility = _button.interAbility;
        if (auxAbility is WeaponKata)
        {
            _button.SetKata
                (
                    GetImage(auxAbility),
                    GetText(auxAbility),
                    () => GetAction(aux),
                    GetImage((auxAbility as WeaponKata).Weapon),
                    () => GetWeaponAction(aux, auxAbility as WeaponKata)
                );
        }
        else
        {
            _button.SetEquipOrAbility(GetImage(auxAbility), GetText(auxAbility), () => GetAction(aux));
        }
    }

    void myEnableMenu()
    {
        comboButtons.Clear();
        HiddeItemList();

        for (int i = 0; i < character.caster.combos.Count; i++)
        {
            UIE_CombosButton aux = new UIE_CombosButton();
            
            int myIndex = i;
            aux.Init(character.caster.combos[myIndex].equiped, myIndex);

            comboButtonsList.Add(aux);
            comboButtons.Add(aux);
        }

        for (int i = 0; i < comboButtonsList.Count; i++)
        {
            int myIndex = i;
            var auxAbility = character.caster.combos[myIndex].equiped;

            SetComboButton(auxAbility, myIndex);
        }

        /*
        for (int i = 0; i < character.caster.combos.Count; i++)
        {
            UIE_CombosButton aux = new UIE_CombosButton();
            aux.Init();

            comboButtonsList.Add(aux);

            int myIndex = i;

            var auxAbility = character.caster.combos[myIndex].equiped;


            if (auxAbility is WeaponKata)
                aux.SetKata(GetImage(character.caster.combos[myIndex]), 
                    GetText(character.caster.combos[myIndex]), 
                    () => GetAction(myIndex), 
                    GetImage(((WeaponKata)auxAbility).Weapon),()=> GetWeaponAction(myIndex, (WeaponKata)auxAbility));
            else
                aux.SetEquipOrAbility(GetImage(character.caster.combos[myIndex]), GetText(character.caster.combos[myIndex]), ()=> GetAction(myIndex));
            
            
            //aux.SetEquipOrAbility(GetImage(character.caster.combos[myIndex]), GetText(character.caster.combos[myIndex]), () => GetAction(myIndex));

            comboButtons.Add(aux);
        }
        */
    }

    void GetWeaponAction(int index, WeaponKata kata)
    {
        listItems.RemoveFromClassList("displayHidden");
        noClickPanel.RemoveFromClassList("displayHidden");

        Vector2 pos = Input.mousePosition.Vect3To2();

        pos = listItems.WorldToLocal(pos);

        listItems.style.top = Screen.height - pos.y;
        listItems.style.left = pos.x;

        var weaponEquiped = kata.Weapon;

        //listItems.position

        //Debug.Log("INDEX: " + index);

        if (weaponEquiped != null)
        {
            SetDetailsWindow(GetImage(weaponEquiped), GetText(weaponEquiped), "", "");
            for (int i = 0; i < character.inventory.Count; i++)
            {
                int itemIndex = i;

                if (!(character.inventory[itemIndex] is MeleeWeapon))
                    continue;
                /*
                if (!((Ability)character.inventory[itemIndex]).visible)
                    continue;
                */
                if (character.inventory[itemIndex].GetItemBase().nameDisplay == weaponEquiped.nameDisplay)
                {
                    UIE_ListButton button = new UIE_ListButton();

                    listItems.Add(button);
                    button.InitOnlyName(null, "Desequipar", (clEvent) =>
                    {
                        kata.TakeOutWeapon();
                        SetComboButton(kata, index);
                        HiddeItemList();
                    });

                    listEquipableItems.Add(button);
                    break;
                }
            }
        }
        else
        {
            detailsWindow.AddToClassList("opacityHidden");
        }

        for (int i = 0; i < character.inventory.Count; i++)
        {
            int itemIndex = i;
            if (!(character.inventory[itemIndex] is MeleeWeapon))
                continue;
            /*
            if (!((Ability)character.inventory[itemIndex]).visible)
                continue;
            */
            if(weaponEquiped!=null)
            {
                if (character.inventory[itemIndex].GetItemBase().nameDisplay == weaponEquiped.nameDisplay)
                    continue;
            }

            System.Action<ClickEvent> changeAction = null;

            changeAction = (clEvent) =>
            {
                kata.ChangeWeapon(character.inventory[itemIndex]);

                SetComboButton(kata, index);
                HiddeItemList();
            };

            UIE_ListButton button = new UIE_ListButton();
            listItems.Add(button);
            button.InitOnlyName(character.inventory[itemIndex].image, character.inventory[itemIndex].nameDisplay, changeAction);

            listEquipableItems.Add(button);
        }

        if (listEquipableItems.Count == 0)
        {
            UIE_ListButton button = new UIE_ListButton();
            listItems.Add(button);
            button.InitOnlyName(null, "No tienes nada para equipar", null);
        }

        return;
    }

    void GetAction(int index)
    {
        listItems.RemoveFromClassList("displayHidden");
        noClickPanel.RemoveFromClassList("displayHidden");

        Vector2 pos = Input.mousePosition.Vect3To2();

        pos = listItems.WorldToLocal(pos);

        listItems.style.top = Screen.height - pos.y;
        listItems.style.left = pos.x;

        //listItems.position

        //Debug.Log("INDEX: " + index);
        if (character.caster.combos[index].equiped != default)
        {
            SetDetailsWindow(character.caster.combos[index].equiped.image, character.caster.combos[index].equiped.nameDisplay, ((ShowDetails)character.caster.combos[index].equiped.GetItemBase()).GetDetails().ToString(), "");

            for (int i = 0; i < character.inventory.Count; i++)
            {
                int itemIndex = i;

                if (!(character.inventory[itemIndex] is Ability))
                    continue;

                if (!((Ability)character.inventory[itemIndex]).visible)
                    continue;

                if (character.inventory[itemIndex].GetItemBase().nameDisplay == character.caster.combos[index].equiped.GetItemBase().nameDisplay)
                {
                    UIE_ListButton button = new UIE_ListButton();

                    listItems.Add(button);
                    button.InitOnlyName(null, "Desequipar", (clEvent) =>
                    {
                        character.caster.combos[index].indexEquipedItem = -1;
                        SetComboButton(character.caster.combos[index].equiped, index);
                        HiddeItemList();
                    });

                    listEquipableItems.Add(button);
                    break;
                }
            }
        }
        else
        {
            detailsWindow.AddToClassList("opacityHidden");
        }
        for (int i = 0; i < character.inventory.Count; i++)
        {
            int itemIndex = i;
            if (!(character.inventory[itemIndex] is Ability))
                continue;

            if (!((Ability)character.inventory[itemIndex]).visible)
                continue;

            if (character.caster.combos[index].equiped != default && character.inventory[itemIndex].GetItemBase().nameDisplay == character.caster.combos[index].equiped.GetItemBase().nameDisplay)
                continue;

            System.Action<ClickEvent> changeAction = null;

            changeAction = (clEvent) => 
            {
                character.caster.combos[index].indexEquipedItem = itemIndex;
                SetComboButton(character.caster.combos[index].equiped, index);
                HiddeItemList();
            };

            UIE_ListButton button = new UIE_ListButton();
            listItems.Add(button);
            button.InitOnlyName(character.inventory[itemIndex].image, character.inventory[itemIndex].nameDisplay, changeAction);

            listEquipableItems.Add(button);
        }

        if (listEquipableItems.Count == 0)
        {
            UIE_ListButton button = new UIE_ListButton();
            listItems.Add(button);
            button.InitOnlyName(null, "No tienes nada para equipar", null);
        }

        return;
    }

    void SetDetailsWindow(Sprite _sprite, string _title, string _description, string _details)
    {
        if (detailsWindow.ClassListContains("opacityHidden"))
            detailsWindow.RemoveFromClassList("opacityHidden");

        itemImage.style.backgroundImage = new StyleBackground(_sprite);
        itemName.text = _title;
        itemDescription.text = _description;
        itemDetails.text = _details;
    }

    void HiddeItemList()
    {
        listItems.Clear();
        listEquipableItems.Clear();

        if (!listItems.ClassListContains("displayHidden"))
            listItems.AddToClassList("displayHidden");

        if (!noClickPanel.ClassListContains("displayHidden"))
            noClickPanel.AddToClassList("displayHidden");
    }

}
