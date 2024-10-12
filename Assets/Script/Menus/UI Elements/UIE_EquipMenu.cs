using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;

public class UIE_EquipMenu : UIE_Equipment
{
    protected VisualElement listContainer;

    protected Label equipTitle;

    //Details Window
    protected VisualElement containerDW;
    protected Label titleDW;
    protected Label descriptionDW;
    protected VisualElement imageDW;

    //Change Buttons
    VisualElement originalButton;
    VisualElement changeButton;

    Action<int> auxAction;
    SlotItem slotItem;
    protected Type filterType;
    ItemEquipable itemEquiped;
    VisualElement equipedItemContainer;
    UIE_ListButton equipedItemButton;

    VisualElement originalItemContainer;
    VisualElement cancelButton;

    //List<UIE_ListButton> buttonsList = new List<UIE_ListButton>();
    Dictionary<int, UIE_ListButton> buttonsList = new Dictionary<int, UIE_ListButton>();

    int itemToChangeIndex = -1;

    int equipedItemIndex = -1;
    int originalItemIndex = -1;

    bool isOnWeaponOfKata => filterType == typeof(MeleeWeapon) && slotItem.GetType() == typeof(WeaponKata);

    #region Config
    protected override void Config()
    {
        base.Config();

        MyAwakes += myAwake;
    }

    private void myAwake()
    {
        if (gameObject.name == manager.EquipItemMenu)
        {
            onEnableMenu += myOnEnable;
            onDisableMenu += myOnDisable;

            equipTitle = ui.Q<Label>("equipTitle");
            listContainer = ui.Q<VisualElement>("listContainer");
            titleDW = ui.Q<Label>("titleDW");
            descriptionDW = ui.Q<Label>("descriptionDW");
            imageDW = ui.Q<VisualElement>("imageDW");
            originalButton = ui.Q<VisualElement>("originalButton");
            changeButton = ui.Q<VisualElement>("changeButton");
            containerDW = ui.Q<VisualElement>("containerDW");
            equipedItemContainer = ui.Q<VisualElement>("equipedItemContainer");
            originalItemContainer = ui.Q<VisualElement>("originalItemContainer");
            cancelButton = ui.Q<VisualElement>("cancelButton");

            onClose += () =>
            {

                if (isOnWeaponOfKata)
                {
                    var weapon = (slotItem as SlotItem<WeaponKata>).equiped.Weapon;
                    if (weapon != null)
                        character.GetInContainer<ModularEquipViewEntityComponent>().DeSpawnWeapon(weapon.itemBase.weaponModel);
                }
                manager.BackLastMenu();
            };
        }
    }

    private void myOnEnable()
    {
        equipedItemContainer.Clear();
        listContainer.Clear();
        buttonsList.Clear();
        equipTitle.text = GetText(null, filterType) + " en: ";
        originalItemContainer.Clear();

        equipedItemButton = new UIE_ListButton();
        equipedItemButton.Init();
        equipedItemButton.AddToClassList("itemToChange");
        equipedItemContainer.Add(equipedItemButton);

        animController = character.GetInContainer<AnimatorController>();

        CreateListItems();
        SetOriginalButton();

        cancelButton.RegisterCallback<ClickEvent>(CancelChange);

        /*
        TimersManager.Create(0.1f, () => 
        { 
            if(slotItem.GetSlotType() == typeof(MeleeWeapon))
                character.GetInContainer<ModularEquipViewEntityComponent>().SpawnWeapon(); 
        }).SetUnscaled(true);
        */
        
        HideWeapon();

        if (filterType == typeof(MeleeWeapon))
            ShowWeapon();
    }
    private void myOnDisable()
    {
        auxAction = null;
        slotItem = null;
        filterType = null;
        itemEquiped = null;

        ShowDetails("", "", null);
        containerDW.AddToClassList("opacityHidden");

        itemToChangeIndex = -1;
        equipedItemIndex = -1;
        originalItemIndex = -1;

        originalButton.HideInUIE();
        changeButton.HideInUIE();
        StopAnimation();

        HideWeapon();
        //currentWeapon = null;
    }
    #endregion

    public void SetEquipMenu(SlotItem _slotItem, Action<int> _action)
    {
        itemEquiped = _slotItem.equiped;
        auxAction = _action;
        slotItem = _slotItem;

        filterType = itemEquiped?.GetType() == null ? _slotItem.GetSlotType() : itemEquiped.GetType();

        originalButton.ShowInUIE();
        originalButton.style.backgroundImage = new StyleBackground(GetImage(itemEquiped, filterType));
    }

    public void SetEquipMenu(SlotItem _slotItem, Type _type, Action<int> _action)
    {
        auxAction = _action;
        slotItem = _slotItem;
        filterType = _type;

        if (slotItem.GetSlotType() == typeof(WeaponKata) && _type != typeof(WeaponKata))
            itemEquiped = (_slotItem.equiped as WeaponKata).Weapon;
        else
            itemEquiped = _slotItem.equiped;


        originalButton.ShowInUIE();

        if (_slotItem.GetSlotType() == typeof(WeaponKata) && _type == typeof(MeleeWeapon))
            originalButton.style.backgroundImage = new StyleBackground(GetImage((_slotItem.equiped as WeaponKata).Weapon, typeof(MeleeWeapon)));
        else
            originalButton.style.backgroundImage = new StyleBackground(GetImage(_slotItem));
    }

    protected void ShowDetails(string nameDisplay, string details, Sprite Image)
    {
        if (nameDisplay == "" && details == "" && Image == null && !containerDW.ClassListContains("opacityHidden"))
        {
            containerDW.AddToClassList("opacityHidden");
            return;
        }

        containerDW.RemoveFromClassList("opacityHidden");
        if (nameDisplay == "")
            titleDW.HideInUIE();
        else
        {
            titleDW.ShowInUIE();
            titleDW.text = nameDisplay;
        }

        if (details == "")
            descriptionDW.HideInUIE();
        else
        {
            descriptionDW.ShowInUIE();
            descriptionDW.text = details;
        }

        if (Image == null)
            imageDW.HideInUIE();
        else
        {
            imageDW.ShowInUIE();
            imageDW.style.backgroundImage = new StyleBackground(Image);
        }
    }

    void CancelChange(ClickEvent _clevent)
    {
        auxAction.Invoke(originalItemIndex);
        HideWeapon();
        manager.BackLastMenu();
    }

    void SetOriginalButton()
    {
        if(slotItem.GetSlotType() == typeof(WeaponKata))
        {
            UIE_KataButton _kata = new UIE_KataButton();
            _kata.Init(slotItem as SlotItem<WeaponKata>, null, null);
            _kata.Disable();
            originalItemContainer.Add(_kata);
            _kata.FreezzeButton();

            _kata.AddEnterMouseEvent(() =>
            {
                if (itemEquiped != null)
                    ShowDetails(itemEquiped.nameDisplay, itemEquiped.GetDetails().ToString("\n") + "\n" + GetDamageDetails(itemEquiped, slotItem), itemEquiped.image);
                else
                    ShowDetails("No tienes nada equipado", "", null);
            });

            return;
        }

        UIE_SlotButton _aux = new UIE_SlotButton();

        if (slotItem.GetSlotType() == typeof(AbilityExtCast))
            _aux.Init<AbilityExtCast>(slotItem, null);
        else
            _aux.Init<MeleeWeapon>(slotItem, null);

        _aux.AddEnterMouseEvent(()=>
        {
            if (itemEquiped != null)
                ShowDetails(itemEquiped.nameDisplay, itemEquiped.GetDetails().ToString("\n") + "\n" + GetDamageDetails(itemEquiped, slotItem), itemEquiped.image);
            else
                ShowDetails("No tienes nada equipado", "", null);
        });
        _aux.Disable();
        _aux.FreezzeButton();
        originalItemContainer.Add(_aux);
    }

    void SetChangeButton(Sprite _sprite, int _itemIndex)
    {
        changeButton.ShowInUIE();
        changeButton.style.backgroundImage = new StyleBackground(_sprite);
        itemToChangeIndex = _itemIndex;
    }

    Sprite GetImage()
    {
        return GetImage(itemEquiped, filterType);
    }

    string GetText()
    {
        return GetText(itemEquiped, filterType);
    }

    void CreateListItems()
    {
        List<int> buffer = new List<int>();


        //Filtrar inventario y setear botón equipado
        for (int i = 0; i < character.inventory.Count; i++)
        {
            var index = i;
            var item = character.inventory[index];

            if (filterType != null && !filterType.IsAssignableFrom(character.inventory[i].GetType()))
                continue;

            if (item is Ability && !((Ability)item).visible)
                continue;
            
            if (itemEquiped is Ability)
            {
                if (itemEquiped.nameDisplay == item.nameDisplay)
                {
                    SetStaticItem(index);
                    originalItemIndex = index;
                }
            }
            else if (item.Equals(itemEquiped))
            {
                SetStaticItem(index);
                originalItemIndex = index;
            }

            buffer.Add(index);
        }

        if (slotItem.defaultItem != null && slotItem.defaultItem.nameDisplay == itemEquiped.nameDisplay && !(itemEquiped is MeleeWeapon))
        {
            equipedItemButton.Set(GetImage(slotItem.defaultItem, filterType), GetText(slotItem.defaultItem, filterType), "Item por defecto", "", () => { });
            equipedItemButton.SetEquipText("");

            equipedItemButton.SetHoverAction(() =>
            {
                ShowDetails(itemEquiped.nameDisplay, "Item por defecto, selecciona un item para equipartelo en su lugar", GetImage());
                SetChangeButton(GetImage(), -1);
            });
        }


        //Crear lista de botones
        foreach (var index in buffer)
        {
            var item = character.inventory[index];

            UIE_ListButton button = new UIE_ListButton();
            listContainer.Add(button);
            buttonsList.Add(index, button);

            button.Init(item.image, item.nameDisplay, item.GetType().ToString(), "-", () =>
            {
                EquipOtherItem(itemToChangeIndex);
            });

            button.SetHoverAction(() =>
            {
                ShowDetails(item.nameDisplay, item.GetDetails().ToString("\n") + "\n" + GetDamageDetails(item, slotItem), item.image);
                SetChangeButton(item.image, index);
            });

            button.SetEquipText("Equipar");
        }

        if (buttonsList.Count == 0)
            ShowDetails("No tienes nada para equipar", "", null);
        else
            SetSelectedButton();
    }
    void ClearSelectedButton()
    {
        if (equipedItemIndex < 0)
            return;

        buttonsList[equipedItemIndex].Enable();
    }
    void SetSelectedButton()
    {
        if (equipedItemIndex < 0)
            return;

        buttonsList[equipedItemIndex].Disable();
    }

    void EquipOtherItem(int _index)
    {
        ClearSelectedButton();
        PreviousAnimAction();
        
        auxAction.Invoke(_index);
        equipedItemIndex = _index;

        PostAnimAction();
        SetSelectedButton();
        SetStaticItem(_index);
        
    }

    void PreviousAnimAction()
    {
        if (filterType == typeof(MeleeWeapon))
        {
            HideWeapon();
            /*
            if (slotItem.GetSlotType() == typeof(WeaponKata))
            {
                var weapon = (slotItem as SlotItem<WeaponKata>).equiped.Weapon;
                if(weapon != null)
                    HideWeapon();
                
            }
            else
            {
                HideWeapon();
            }
            */
        }
        else if (filterType == typeof(WeaponKata))
            StopAnimation();
    }
    void PostAnimAction()
    {
        if (filterType == typeof(MeleeWeapon) && equipedItemIndex >= 0)
        {
            if (slotItem.GetSlotType() == typeof(WeaponKata))
            {
                var weapon = (slotItem as SlotItem<WeaponKata>).equiped.Weapon;
                if (weapon != null)
                    ShowWeapon(weapon);
            }
            else
            {
                ShowWeapon();
            }
        }
        else if (filterType == typeof(WeaponKata) && equipedItemIndex >= 0)
            ShowAnimationLoop((character.inventory[equipedItemIndex] as WeaponKata).itemBase.animations.animClips["Cast"]);
            
    }

    void SetStaticItem(int _index)
    {
        var item = _index >= 0 ? character.inventory[_index] : null;
        equipedItemIndex = _index;

        if (item != null)
        {
            equipedItemButton.Set(GetImage(item as ItemEquipable, filterType), GetText(item as ItemEquipable, filterType), "", "", () => 
            {
                if(slotItem.defaultItem != null)
                {
                    EquipOtherItem(-1);
                    changeButton.HideInUIE();
                    ShowDetails(slotItem.defaultItem.nameDisplay, slotItem.defaultItem.GetDetails().ToString("\n") + "\n" + GetDamageDetails(item, slotItem), slotItem.defaultItem.image);
                }
                else
                {
                    EquipOtherItem(-1);
                    changeButton.HideInUIE();
                    ShowDetails(GetText(null, filterType), "", null);
                }
            });
            equipedItemButton.SetEquipText("Desequipar");
            equipedItemButton.SetHoverAction(() =>
            {
                ShowDetails(item.nameDisplay, item.GetDetails().ToString("\n") + "\n" + GetDamageDetails(item, slotItem), item.image);
                SetChangeButton(GetImage(item as ItemEquipable, filterType), -1);
            });
        }
        else
        {
            if(slotItem.defaultItem!=null)
            {
                equipedItemButton.Set(GetImage(slotItem.defaultItem, filterType), GetText(slotItem.defaultItem, filterType), "", "", () => {});
                equipedItemButton.SetEquipText("");
                equipedItemButton.SetHoverAction(() =>
                {
                    changeButton.HideInUIE();
                    ShowDetails(slotItem.defaultItem.nameDisplay, slotItem.defaultItem.GetDetails().ToString("\n") + "\n" + GetDamageDetails(item, slotItem), slotItem.defaultItem.image);
                });
            }
            else
            {
                equipedItemButton.Set(GetImage(null, filterType), GetText(null, filterType), "", "", () => { });
                equipedItemButton.SetEquipText("");
                equipedItemButton.SetHoverAction(() =>
                {
                    changeButton.HideInUIE();
                    ShowDetails("No tienes nada equipado", "", null);
                });
            }
        }

        if(slotItem.defaultItem != null)
            originalButton.style.backgroundImage = new StyleBackground(slotItem.defaultItem.image);
        else
            originalButton.style.backgroundImage = new StyleBackground(GetImage(item as ItemEquipable, filterType));
    }


    void SetSelectedItem2(int _index)
    {
        //Debug.Log("SET SELECTED ITEM----------------------------------------------------------------");
        //definitiveItemToChangeIndex = _index;
        var item = _index >= 0? character.inventory[_index] : null;
        
        if(buttonsList.Count > 0)
            ClearSelectedButton();

        equipedItemIndex = _index;
        if (item != null)
        {
            if(filterType == typeof(MeleeWeapon))
                character.GetInContainer<ModularEquipViewEntityComponent>().DeSpawnWeapon(character.caster.actualWeapon.Weapon.itemBase.weaponModel);
            
            auxAction.Invoke(_index);

            equipedItemButton.Set(GetImage(item as ItemEquipable, filterType), GetText(item as ItemEquipable, filterType), "", "", () => SetStaticItem(-1));
            equipedItemButton.SetEquipText("Desequipar");
            equipedItemButton.SetHoverAction(() =>
            {
                ShowDetails(item.nameDisplay, item.GetDetails().ToString("\n") + "\n" + GetDamageDetails(item, slotItem), item.image);
                SetChangeButton(item.image, -1);
            });

            if (filterType == typeof(MeleeWeapon))
                character.GetInContainer<ModularEquipViewEntityComponent>().SpawnWeapon(character.caster.actualWeapon.Weapon.itemBase.weaponModel);
        }
        else
        {
            equipedItemButton.Set(GetImage(null, filterType), GetText(null, filterType), "", "", ()=> { });
            equipedItemButton.SetEquipText("");
            equipedItemButton.SetHoverAction(() =>
            {
                ShowDetails("No tienes nada equipado", "Selecciona un item para equipar", null);
                SetChangeButton(GetImage(), -1);
            });
            character.GetInContainer<ModularEquipViewEntityComponent>().DeSpawnWeapon(character.caster.actualWeapon.Weapon.itemBase.weaponModel);

            auxAction.Invoke(_index);
        }

        if (buttonsList.Count > 0)
            SetSelectedButton();

        originalButton.style.backgroundImage = new StyleBackground(GetImage(itemEquiped, filterType));
    }

    string GetDamageDetails(Item item, SlotItem slotItem)
    {
        string damages = "";

        if (item is MeleeWeapon)
        {
            if (slotItem is SlotItem<WeaponKata>)
                damages = KataWeaponDamages((MeleeWeapon)item, (WeaponKata)slotItem.equiped);
            else
                damages = BaseWeaponDamages((MeleeWeapon)item);
        }
        else if (item is WeaponKata)
            damages = KataDamages((WeaponKata)item);
        else if (item is AbilityExtCast)
            damages = AbilityDamages((AbilityExtCast)item);

        return damages;
    }
    string BaseWeaponDamages(MeleeWeapon _weapon)
    {
        string mainText = "Daños detallados\n".RichText("color", "#f6f1c2");

        var weaponDmgs = _weapon.damages;
        var characterDmgs = character.caster.additiveDamage.content.SortBy(weaponDmgs, 0).ToArray();
        var kataDmgs = _weapon.defaultKata.multiplyDamage.content.SortBy(characterDmgs, 1).ToArray();

        var titulos = new CustomColumns("Arma", "Jugador", "Kata (Por defecto)", "Final/Resultado");
        mainText += titulos.ToString().RichText("color", "#ddacb4");

        var resultDmgs = Damage.CalcDamage(_weapon, character.caster, _weapon.defaultKata).SortBy(kataDmgs, 0).ToArray();

        //mainText += "\nCharacter and weapon combined damages:\n";
        var charAndWeapResult = new CustomColumns(weaponDmgs.ToString(": ", "\n"), characterDmgs.ToString(": +", "\n"), kataDmgs.ToString(": x", "\n"), resultDmgs.ToString(": ", "\n"));
        mainText += charAndWeapResult.ToString();

        if (slotItem?.equiped != null)
        {
            var equipedWeapon = ((MeleeWeapon)slotItem.equiped);

            var resultequipedDmgs = Damage.CalcDamage(equipedWeapon, character.caster, equipedWeapon.defaultKata).ToArray();

            mainText += "Comparación con el arma equipada:\n".RichText("color", "#f6f1c2");
            mainText += new CustomColumns("Daño de arma equipada".RichText("color", "#d4aaa9"), "Daño de arma actual".RichText("color", "#d4aaa9")).ToString();
            mainText += new CustomColumns(resultequipedDmgs.ToString(": ", "\n"), resultDmgs.ToString(": ", "\n")).ToString();
        }

        return mainText;
    }
    string AbilityDamages(AbilityExtCast _ability)
    {
        string mainText = "Daños detallados\n".RichText("color", "#f6f1c2");

        if (!(_ability.castingAction.GetCastActionBase() is CastingDamageBase))
            return "";

        CastingDamageBase castAction = (CastingDamageBase)_ability.castingAction.GetCastActionBase();

        var castDmgs = castAction.damages;
        var abilityDmgs = _ability.multiplyDamage.content.ToArray();
        var characterDmgs = character.caster.additiveDamage.content.ToArray();

        CustomColumns titulos = new CustomColumns("Casteo", "Jugador", "Habilidad", "Final/Resultado");
        mainText += titulos.ToString().RichText("color", "#ddacb4");

        var resultDmgs = Damage.CalcDamage(castAction.damages, character.caster.additiveDamage.content, _ability.multiplyDamage.content).ToArray();
        CustomColumns test1 = new CustomColumns(castDmgs.ToString(": ", "\n"), characterDmgs.ToString(": +", "\n"), abilityDmgs.ToString(": x", "\n"), resultDmgs.ToString(": ", "\n"));

        mainText += test1.ToString();

        if (slotItem?.equiped != null)
        {
            var aux = ((AbilityExtCast)slotItem.equiped).castingAction.GetCastActionBase();
            if (!(aux is CastingDamageBase))
                return "La habilidad equipada no produce daño";

            var resultequipedDmgs = Damage.CalcDamage(((CastingDamageBase)aux).damages, character.caster.additiveDamage.content, ((AbilityExtCast)slotItem.equiped).multiplyDamage.content).ToArray();

            mainText += "Comparación con la habilidad equipada:\n".RichText("color", "#f6f1c2");
            mainText += new CustomColumns("Daño de habilidad equipada".RichText("color", "#d4aaa9"), "Daño de habilidad actual".RichText("color", "#d4aaa9")).ToString();
            mainText += new CustomColumns(resultequipedDmgs.ToString(": ", "\n"), resultDmgs.ToString(": ", "\n")).ToString();
        }

        return mainText;
    }

    string KataDamages(WeaponKata _kata)
    {
        string mainText = "Daños detallados\n".RichText("color", "#f6f1c2");

        var characterDmgs = character.caster.additiveDamage.content.ToArray();
        var kataDmgs = _kata.multiplyDamage.content.SortBy(characterDmgs, 1).ToArray();

        var titulos = new CustomColumns("Jugador", "Kata");
        mainText += titulos.ToString().RichText("color", "#ddacb4");

        mainText += new CustomColumns(characterDmgs.ToString(": +", "\n"), kataDmgs.ToString(": x", "\n")).ToString();

        if (slotItem?.equiped != null)
        {
            var resultequipedDmgs = ((WeaponKata)slotItem.equiped).multiplyDamage.content.SortBy(kataDmgs, 0).ToArray();

            mainText += "Comparación con la kata equipada:\n".RichText("color", "#f6f1c2");
            mainText += new CustomColumns("Multiplicador de kata equipada".RichText("color", "#d4aaa9"), "Multiplicador de kata actual".RichText("color", "#d4aaa9")).ToString();
            mainText += new CustomColumns(resultequipedDmgs.ToString(": x", "\n"), kataDmgs.SortBy(resultequipedDmgs, 0).ToArray().ToString(": x", "\n")).ToString();
        }

        return mainText;
    }

    string KataWeaponDamages(MeleeWeapon _weaponKata, WeaponKata _kata)
    {
        string mainText = "Daños detallados\n".RichText("color", "#f6f1c2");

        var weaponDmgs = _weaponKata.itemBase.damages;
        var characterDmgs = character.caster.additiveDamage.content.SortBy(weaponDmgs, 0).ToArray();
        var kataDmgs = _kata.multiplyDamage.content.SortBy(characterDmgs, 1).ToArray();

        var titulos = new CustomColumns("Arma", "Jugador", "Kata", "Final/Resultado");
        mainText += titulos.ToString().RichText("color", "#ddacb4");

        var resultDmgs = Damage.CalcDamage(_weaponKata, character.caster, _kata).SortBy(kataDmgs, 1).ToArray();

        mainText += new CustomColumns(weaponDmgs.ToString(": ", "\n"), characterDmgs.ToString(": +", "\n"), kataDmgs.ToString(": x", "\n"), resultDmgs.ToString(": ", "\n")).ToString();

        if (_kata.Weapon != null)
        {
            var equipedWeapon = _kata.Weapon;

            var resultequipedDmgs = Damage.CalcDamage(equipedWeapon, character.caster, _kata).SortBy(resultDmgs, 0).ToArray();

            mainText += "Comparación con el arma equipada en la Kata:\n".RichText("color", "#f6f1c2");
            mainText += new CustomColumns($"Daño de arma equipada ({equipedWeapon.nameDisplay})".RichText("color", "#d4aaa9"), $"Daño de {_weaponKata.nameDisplay}".RichText("color", "#d4aaa9")).ToString();
            mainText += new CustomColumns(resultequipedDmgs.ToString(": ", "\n"), resultDmgs.SortBy(resultequipedDmgs, 0).ToArray().ToString(": ", "\n")).ToString();
        }

        return mainText;
    }

}
