using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;

public class UIE_EquipMenu : UIE_BaseMenu
{
    public VisualElement listContainer;

    //Details Window
    VisualElement containerDW;
    Label titleDW;
    Label descriptionDW;
    VisualElement imageDW;

    //Change Buttons
    VisualElement originalButton;
    VisualElement changeButton;

    Action<SlotItem, int> auxAction;
    SlotItem slotItem;
    Type filterType;

    List<UIE_ListButton> buttonsList = new List<UIE_ListButton>();

    protected override void Config()
    {
        base.Config();

        MyAwakes += myAwake;
    }

    private void myAwake()
    {
        onEnableMenu += myOnEnable;
        onDisableMenu += myOnDisable;

        listContainer = ui.Q<VisualElement>("listContainer");
        titleDW = ui.Q<Label>("titleDW");
        descriptionDW = ui.Q<Label>("descriptionDW");
        imageDW = ui.Q<VisualElement>("imageDW");
        originalButton = ui.Q<VisualElement>("originalButton");
        changeButton = ui.Q<VisualElement>("changeButton");
        containerDW = ui.Q<VisualElement>("containerDW");

        onClose += () => manager.BackLastMenu();
    }

    private void myOnEnable()
    {
        listContainer.Clear();
        CreateListItems();
    }
    private void myOnDisable()
    {
        auxAction = null;
        slotItem = null;
        filterType = null;

        SetChangeButton(null);

        ShowItemDetails("", "", null);
        containerDW.AddToClassList("opacityHidden");

        originalButton.style.display = DisplayStyle.None;
        originalButton.style.backgroundImage = default;
    }

    public void SetEquipMenu<T>(SlotItem _slotItem, Type _type, Action<SlotItem, int> _action) where T : Item
    {
        auxAction = _action;
        slotItem = _slotItem;
        filterType = _type;

        if(_slotItem.equiped != null)
        {
            originalButton.style.display = DisplayStyle.Flex;
            originalButton.style.backgroundImage = new StyleBackground(_slotItem.equiped.image);
        }
    }
    void SetChangeButton(Sprite _sprite)
    {
        if(_sprite!=null)
            changeButton.style.display = DisplayStyle.Flex;
        else
            changeButton.style.display = DisplayStyle.None;

        changeButton.style.backgroundImage = new StyleBackground(_sprite);
    }

    void CreateListItems()
    {
        for (int i = 0; i < character.inventory.Count; i++)
        {
            if (filterType != null && !filterType.IsAssignableFrom(character.inventory[i].GetType()))
                continue;

            if (character.inventory[i] is Ability && !((Ability)character.inventory[i]).visible)
                continue;

            var index = i;

            var item = character.inventory[index];

            var changeText = "Equipar";

            System.Action<ClickEvent> action =
               (clEvent) =>
               {
                   ShowItemDetails(item.nameDisplay, item.GetDetails().ToString("\n") + "\n" + GetDamageDetails(item, slotItem), item.image);
                   SetChangeButton(item.image);
               };

            System.Action<ClickEvent> changeAction = null;
            if (slotItem != null)
            {
                if (slotItem.equiped != default && item.GetItemBase().nameDisplay == slotItem.equiped.GetItemBase().nameDisplay)
                {
                    changeText = "Desequipar";
                    changeAction = (clEvent) =>
                    {
                        slotItem.indexEquipedItem = -1;
                        TriggerOnClose(default);
                    };
                }
                else
                {
                    changeText = "Equipar";
                    changeAction = (clEvent) => { auxAction.Invoke(slotItem, index); TriggerOnClose(default); };
                }
            }

            UIE_ListButton button = new UIE_ListButton();

            //Debug.Log("List container is null = " + (listContainer == null));
            listContainer.Add(button);
            button.Init(item.image.texture, item.nameDisplay, item.GetType().ToString(), "Cortante", true, action, changeAction, changeText);

            buttonsList.Add(button);
        }

        if (buttonsList.Count == 0)
            ShowItemDetails("No tienes nada para equipar", "", null);

        filterType = null;
    }

    void ShowItemDetails(string nameDisplay, string details, Sprite Image)
    {
        containerDW.RemoveFromClassList("opacityHidden");
        titleDW.text = nameDisplay;
        descriptionDW.text = details;
        imageDW.style.backgroundImage = new StyleBackground(Image);
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

        var titulos = new CustomColumns("Arma", "Jugador", "Kata (default)", "Final/Resultado");
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

        if (slotItem?.equiped != null)
        {
            var equipedWeapon = ((WeaponKata)slotItem.equiped).Weapon;

            var resultequipedDmgs = Damage.CalcDamage(equipedWeapon, character.caster, _kata).SortBy(resultDmgs, 0).ToArray();

            mainText += "Comparación con el arma equipada en la Kata:\n".RichText("color", "#f6f1c2");
            mainText += new CustomColumns($"Daño de arma equipada ({equipedWeapon.nameDisplay})".RichText("color", "#d4aaa9"), $"Daño de {_weaponKata.nameDisplay}".RichText("color", "#d4aaa9")).ToString();
            mainText += new CustomColumns(resultequipedDmgs.ToString(": ", "\n"), resultDmgs.SortBy(resultequipedDmgs, 0).ToArray().ToString(": ", "\n")).ToString();
        }

        return mainText;
    }

}
