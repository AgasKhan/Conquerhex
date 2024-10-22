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

    VisualElement inventoryButton;
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

        inventoryButton = ui.Q<VisualElement>("inventoryButton");
        equipmentButton = ui.Q<VisualElement>("equipmentButton");

        inventoryButton.RegisterCallback<ClickEvent>((clEvent) => manager.SwitchMenu(UIE_MenusManager.instance.InventoryMenu));
        equipmentButton.RegisterCallback<ClickEvent>((clEvent) => manager.SwitchMenu(UIE_MenusManager.instance.EquipmentMenu));

        noClickPanel.RegisterCallback<ClickEvent>((cleEvent)=> HiddeItemList());
        
        onClose += () => manager.DisableMenu(gameObject.name);
    }

    void myEnableMenu()
    {
        comboButtons.Clear();
        HiddeItemList();

        for (int i = 0; i < character.caster.combos.Count; i++)
        {
            UIE_CombosButton aux = new UIE_CombosButton();
            comboButtonsList.Add(aux);
            comboButtons.Add(aux);

            int myIndex = i;
            aux.Init(myIndex);
            SetComboButton(character.caster.combos[myIndex].equiped, myIndex);

            if (character.caster.abilityCasting != null && character.caster.abilityCasting.Equals(character.caster.combos[myIndex].equiped))
                aux.Block("En uso");
            else
                aux.Block(character.caster.combos[myIndex].isBlocked);
        }

        animController = character.GetInContainer<AnimatorController>();
    }

    private void myOnDisable()
    {
        HiddeItemList();
        comboButtonsList.Clear();
    }

    void SetComboButton<T>(T auxAbility, int indexSlot) where T : Ability
    {
        var aux = comboButtonsList[indexSlot];

        //Debug.Log("Al setear el ComboButton, su indice de combos es: " + aux.indexSlot);

        System.Action enterAct = GetHoverActs(auxAbility, out System.Action _leaveAct);
        aux.SetHoverActs(enterAct, _leaveAct);

        if (auxAbility is WeaponKata)
        {
            aux.SetKata(auxAbility as WeaponKata, GetAction(aux.indexSlot), GetWeaponAction(aux.indexSlot, auxAbility as WeaponKata));

            enterAct = GetHoverActs((auxAbility as WeaponKata).Weapon, out _leaveAct);
            aux.SetWeaponHoverActs(enterAct, _leaveAct, (auxAbility as WeaponKata).Weapon);
        }
        else
        {
            aux.SetEquipOrAbility(auxAbility as AbilityExtCast, GetAction(aux.indexSlot));
        }
    }

    System.Action GetHoverActs(ItemEquipable ability, out System.Action _leaveAct)
    {
        if(ability==null)
        {
            _leaveAct = default;
            return () => detailsWindow.AddToClassList("opacityHidden");
        }
        else
        {
            _leaveAct = () => detailsWindow.AddToClassList("opacityHidden");
            return () => SetDetailsWindow(ability.image, ability.nameDisplay, ability.GetItemBase().GetTooltip(), "");
        }
    }

    UnityAction GetWeaponAction(int index, WeaponKata kata)
    {
        return ()=>
        {
            ShowListItem();

            List<int> buffer = new List<int>();

            //Filtrar inventario
            for (int i = 0; i < character.inventory.Count; i++)
            {
                int itemIndex = i;

                if (!(character.inventory[itemIndex] is MeleeWeapon))
                    continue;

                buffer.Add(itemIndex);
            }

            //Crear botón deesequipar
            var weaponEquiped = kata.Weapon;

            UIE_ListButton initButton = new UIE_ListButton();

            listItems.Add(initButton);

            if (weaponEquiped != null)
            {
                foreach (var itemIndex in buffer)
                {
                    if (character.inventory[itemIndex].Equals(weaponEquiped))
                    {
                        initButton.InitOnlyName(null, "Desequipar", () =>
                        {
                            kata.TakeOutWeapon();
                            SetComboButton(character.caster.combos[index].equiped, index);
                            HiddeItemList();
                        }, null);

                        listEquipableItems.Add(initButton);
                        buffer.Remove(itemIndex);
                        break;
                    }
                }
            }

            //Crear otros botones
            foreach (var itemIndex in buffer)
            {
                System.Action changeAction = () =>
                {
                    kata.ChangeWeapon(character.inventory[itemIndex]);

                    SetComboButton(character.caster.combos[index].equiped, index);
                    HiddeItemList();
                };

                UIE_ListButton button = new UIE_ListButton();
                listItems.Add(button);

                button.InitOnlyName(character.inventory[itemIndex].image, character.inventory[itemIndex].nameDisplay, changeAction, typeof(MeleeWeapon));

                listEquipableItems.Add(button);
            }

            if (listEquipableItems.Count == 0)
            {
                initButton.InitOnlyName(null, "No tienes nada para equipar", null, null);
            }
        };
    }

    UnityAction GetAction(int index)
    {
        return () =>
        {
            ShowListItem();

            List<int> buffer = new List<int>();

            for (int i = 0; i < character.inventory.Count; i++)
            {
                int itemIndex = i;

                if (!(character.inventory[itemIndex] is Ability))
                    continue;

                if (!((Ability)character.inventory[itemIndex]).visible)
                    continue;

                buffer.Add(itemIndex);
            }

            if (character.caster.combos[index].equiped != default(ItemEquipable))
            {
                foreach (var itemIndex in buffer)
                {
                    if (character.inventory[itemIndex].nameDisplay == character.caster.combos[index].equiped.nameDisplay)
                    {
                        UIE_ListButton button = new UIE_ListButton();

                        listItems.Add(button);
                        button.InitOnlyName(null, "Desequipar", () =>
                        {
                            character.caster.combos[index].indexEquipedItem = -1;
                            SetComboButton(character.caster.combos[index].equiped, index);
                            HiddeItemList();
                        }, null);

                        listEquipableItems.Add(button);
                        buffer.Remove(itemIndex);
                        break;
                    }
                }
            }

            foreach (var itemIndex in buffer)
            {
                Ability auxAbility = (Ability)character.inventory[itemIndex];

                System.Action changeAction = () =>
                {
                    auxAbility.CreateCopy(out int indexCopy);
                    character.caster.combos[index].indexEquipedItem = indexCopy;

                    //Debug.Log("Indice de la copia creada: " + indexCopy);
                    //Debug.Log("Copia equipada en el slot: " + index);
                    SetComboButton(character.caster.combos[index].equiped, index);
                    HiddeItemList();
                };

                UIE_ListButton button = new UIE_ListButton();
                listItems.Add(button);
                button.InitOnlyName(auxAbility.image, auxAbility.nameDisplay, changeAction, character.caster.combos[index].equiped?.GetType());

                listEquipableItems.Add(button);
            }

            if (listEquipableItems.Count == 0)
            {
                UIE_ListButton button = new UIE_ListButton();
                listItems.Add(button);
                button.InitOnlyName(null, "No tienes nada para equipar", null, null);
            }
        };
    }

    void ShowListItem()
    {
        manager.HideTooltip();

        var mousePosition = Input.mousePosition;
        Vector2 mousePositionCorrected = new Vector2(mousePosition.x, Screen.height - mousePosition.y);
        mousePositionCorrected = RuntimePanelUtils.ScreenToPanel(manager.GetCurrentMenu().ui.panel, mousePositionCorrected);

        listItems.style.top = mousePositionCorrected.y;
        listItems.style.left = mousePositionCorrected.x;

        listItems.ShowInUIE();
        noClickPanel.ShowInUIE();
    }

    void SetDetailsWindow(Sprite _sprite, string _title, string _description, string _details)
    {
        itemImage.style.backgroundImage = new StyleBackground(_sprite);
        itemName.text = _title;
        itemDescription.text = _description;
        itemDetails.text = _details;

        if (detailsWindow.ClassListContains("opacityHidden"))
            detailsWindow.RemoveFromClassList("opacityHidden");
    }

    void HiddeItemList()
    {
        listItems.Clear();
        listEquipableItems.Clear();

        listItems.HideInUIE();
        noClickPanel.HideInUIE();
    }

}
