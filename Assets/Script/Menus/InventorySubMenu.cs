using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class InventorySubMenu : CreateSubMenu
{
    Character character;

    public ScrollVertComponent itemList;

    List<ButtonHor> buttonsList = new List<ButtonHor>();

    List<EventsCall> buttonsListActions = new List<EventsCall>();

    ListNavBarModule myListNavBar;

    DetailsWindow myDetailsW;

    System.Type filterType;

    public SlotItem slotItem = null;

    System.Action<SlotItem, int> auxAction = null;

    public void SetEquipMenu<T>(SlotItem _slotItem, System.Type _type, System.Action<SlotItem, int> _action) where T : Item
    {
        auxAction = _action;
        slotItem = _slotItem;
        filterType = _type;
    }

    public override void Create(Character _character)
    {
        character = _character;
        base.Create(_character);
    }
    protected override void InternalCreate()
    {
        //subMenu.navbar.DestroyAll();

        //subMenu.AddNavBarButton("Inventario", () => Create(character)).AddNavBarButton("Equipamiento", () => CreateStatistics());

        /*
        subMenu.AddNavBarButton("All", () => { FilterItems(""); }).AddNavBarButton("Equipment", () => { FilterItems("MeleeWeapon"); })
                    .AddNavBarButton("Resources", () => { FilterItems("Resources_Item"); }).AddNavBarButton("Katas", () => { FilterItems("WeaponKata"); })
                    .AddNavBarButton("Abilities", () => { FilterItems("AbilityExtCast"); });
        */
        subMenu.CreateTitle("Inventario");

        CreateBody();

        subMenu.OnClose += InventoryOnClose;
    }

    private void InventoryOnClose()
    {
        slotItem = null;
        subMenu.OnClose -= InventoryOnClose;
    }
   
    void CreateBody()
    {
        subMenu.ClearBody();

        subMenu.CreateSection(0, 3);
        myListNavBar = subMenu.AddComponent<ListNavBarModule>();
        myListNavBar.SetTitle("Inventario");
        //myListNavBar.SetTags(new ItemTags("Peso", "Peso Total", "Tipo", "Cantidad"));
        CreateButtons();

        subMenu.CreateSection(3, 6);
        subMenu.CreateChildrenSection<ScrollRect>();
        myDetailsW = subMenu.AddComponent<DetailsWindow>();

        if (slotItem != null)
            subMenu.navbar.DestroyAll();

        if (buttonsList.Count <= 0 && slotItem != null)
            ShowItemDetails("", "No tienes nada que equipar", null);
    }

    public void CreateButtons()
    {
        buttonsList.Clear();
        /*
        if (slotItem != null && slotItem.equiped != null)
        {
            CreateUnequipButton(slotItem);
        }
        */
        for (int i = 0; i < character.inventory.Count; i++)
        {
            if (filterType != null && !filterType.IsAssignableFrom(character.inventory[i].GetType()))
                continue;
            
            if (character.inventory[i] is Ability && !((Ability)character.inventory[i]).visible)
                continue;
            
            var index = i;

            var item = character.inventory[index];

            UnityEngine.Events.UnityAction action =
               () =>
               {
                   //Debug.Log("El indice de " + item.nameDisplay + " es: " + index);

                   ShowItemDetails(item.nameDisplay, item.GetDetails().ToString("\n"), item.image);

                   DestroyButtonsActions();

                   CreateDetailsButton(item);


                   /*
                   if (item.GetItemBase() is WeaponKataBase)
                   {
                       WeaponKata auxKata = (WeaponKata)item;

                       string mainText = "------------------------------------------------------------------\n";

                       var kataDmgs = auxKata.multiplyDamage.content.ToArray().ToString(": x", "\n");
                       var characterDmgs = character.caster.additiveDamage.content.ToArray().ToString(": ", "\n");
                       var weaponDmgs = auxKata.WeaponEnabled.itemBase.damages.ToString(": ", "\n");

                       mainText += "Kata Selected: " + item.nameDisplay + "\n";

                       var titulos = new CustomColumns ("Character damages", "operacion", "Weapon equiped damages", "operacion", "Kata Selected" );

                       var test1 = new CustomColumns(characterDmgs, "+", weaponDmgs, "x", kataDmgs);

                       mainText += (titulos + test1).ToString();

                       var totalDamage = Damage.Combine(Damage.AdditiveFusion, auxKata.WeaponEnabled.itemBase.damages, character.caster.additiveDamage.content);
                       var resultDmgs = totalDamage.ToArray().ToString(": ", "\n");

                       mainText += "\nCharacter and weapon combined damages:\n";
                       var charAndWeapResult = new CustomColumns(characterDmgs, "+\n+\n+", weaponDmgs, "=\n=\n=", resultDmgs);
                       mainText += charAndWeapResult.ToString();

                       totalDamage = Damage.Combine(Damage.MultiplicativeFusion, totalDamage, auxKata.multiplyDamage.content);

                       mainText += "\nCharacter/Weapon and Kata combined damages:\n";
                       var resultAndKata = totalDamage.ToArray().ToString(": ", "\n");

                       mainText += new CustomColumns(resultDmgs, "x\nx\nx", kataDmgs, "=\n=\n=", resultAndKata).ToString();

                       mainText += "\nAll damages operations:\n";
                       mainText += (new CustomColumns("x\nx\nx", kataDmgs, "=\n=\n=", resultAndKata).AddLeft(charAndWeapResult)).ToString();

                       mainText += "\n------------------------------------------------------------------";

                       Debug.Log(mainText);

                   }
                   */
               };

            var button = myListNavBar.AddButtonHor(item.nameDisplay, item.image, item.GetItemTags(), action);
            buttonsList.Add(button);

            if (slotItem != null)
            {
                if (slotItem.equiped != default && item.GetItemBase().nameDisplay == slotItem.equiped.GetItemBase().nameDisplay)
                {
                    button.SetAuxButton("Desequipar", () =>
                    {
                        slotItem.indexEquipedItem = -1;
                        MenuManager.instance.modulesMenu.ObtainMenu<SubMenus>().TriggerOnClose();
                    }, "");
                }
                else
                    button.SetAuxButton("Equipar", () => { auxAction.Invoke(slotItem, index); }, "");
            }
            //buttonsList.Add(button.SetButtonA(item.nameDisplay, item.image, SetTextforItem(item), action).SetType(item.itemType.ToString()));
        }
        
        filterType = null;
    }

    void DestroyButtonsActions()
    {
        foreach (var item in buttonsListActions)
        {
            if (item != null)
                Object.Destroy(item.gameObject);
        }

        buttonsListActions.Clear();
    }

    void CreateDetailsButton(Item item)
    {
        buttonsListActions.Add(subMenu.AddComponent<EventsCall>().Set("Más detalles", () => 
        {
            myDetailsW.SetTexts(item.nameDisplay, GetDamageDetails(item, slotItem));
        }, ""));
        buttonsListActions[buttonsListActions.Count - 1].rectTransform.sizeDelta = new Vector2(200, 65);
    }


    string GetDamageDetails(Item item, SlotItem slotItem)
    {
        string damages = "";
        
        if (item is MeleeWeapon)
        {
            if(slotItem is SlotItem<MeleeWeapon>)
                damages = BaseWeaponDamages((MeleeWeapon)item);
            else
                damages = KataWeaponDamages((MeleeWeapon)item, (WeaponKata)slotItem.equiped);
        } 
        else if (item is WeaponKata)
            damages = KataDamages((WeaponKata)item);
        else if (item is AbilityExtCast)
            damages = AbilityDamages((AbilityExtCast)item);
        
        return damages;
    }

    string BaseWeaponDamages(MeleeWeapon _weapon)
    {
        string mainText = "Daños detallados\n".RichText("color", "#832b28");

        var weaponDmgs = _weapon.damages.ToString(": ", "\n");
        var characterDmgs = character.caster.additiveDamage.content.ToArray().ToString(": +", "\n");

        var titulos = new CustomColumns("Daño del arma", "Daño del jugador", "Daño final");
        mainText += titulos.ToString();

        var totalDamage = Damage.Combine(Damage.AdditiveFusion, _weapon.itemBase.damages, character.caster.additiveDamage.content);
        var resultDmgs = totalDamage.ToArray().ToString(": ", "\n");

        //mainText += "\nCharacter and weapon combined damages:\n";
        var charAndWeapResult = new CustomColumns(weaponDmgs, characterDmgs, resultDmgs);
        mainText += charAndWeapResult.ToString();

        return mainText;
    }
    string AbilityDamages(AbilityExtCast _ability)
    {
        string mainText = "Daños detallados\n".RichText("color", "#832b28");

        if (!(_ability.castingAction.GetCastActionBase() is CastingDamageBase))
            return "";

        var castAction = (CastingDamageBase)_ability.castingAction.GetCastActionBase();

        var abilityDmgs = _ability.multiplyDamage.content.ToArray().ToString(": x", "\n");
        var characterDmgs = character.caster.additiveDamage.content.ToArray().ToString(": +", "\n");
        var castDmgs = castAction.damages.ToString(": ", "\n");

        var titulos = new CustomColumns("Daño del casteo", "Daño del jugador", "Daño de la habilidad", "Resultado");

        mainText += titulos.ToString();

        var totalDamage = Damage.Combine(Damage.AdditiveFusion, castAction.damages, character.caster.additiveDamage.content);
        totalDamage = Damage.Combine(Damage.MultiplicativeFusion, totalDamage, _ability.multiplyDamage.content);
        //var resultDmgs = totalDamage.ToArray().ToString(": ", "\n");
        var test1 = new CustomColumns(castDmgs, characterDmgs, abilityDmgs, (totalDamage.ToArray().ToString(": ", "\n")));

        mainText += test1.ToString();

        return mainText;
    }
    string KataDamages(WeaponKata _kata)
    {
        string mainText = "Daños detallados\n".RichText("color", "#832b28");

        var characterDmgs = character.caster.additiveDamage.content.ToArray().ToString(": +", "\n");
        var kataDmgs = _kata.multiplyDamage.content.ToArray().ToString(": x", "\n");

        var titulos = new CustomColumns("Daño del jugador", "Daño de la Kata");

        mainText += titulos.ToString();
        mainText += new CustomColumns(characterDmgs, kataDmgs).ToString();

        return mainText;
    }
    string KataWeaponDamages(MeleeWeapon _weaponKata, WeaponKata _kata)
    {
        string mainText = "Daños detallados\n".RichText("color", "#832b28");

        var weaponDmgs = _weaponKata.itemBase.damages.ToString(": ", "\n");
        var characterDmgs = character.caster.additiveDamage.content.ToArray().ToString(": +", "\n");
        var kataDmgs = _kata.multiplyDamage.content.ToArray().ToString(": x", "\n");

        var titulos = new CustomColumns("Daño del arma", "Daño del jugador", "Daño de la Kata", "Daño final");

        mainText += titulos.ToString();

        var totalDamage = Damage.Combine(Damage.AdditiveFusion, _weaponKata.itemBase.damages, character.caster.additiveDamage.content);
        totalDamage = Damage.Combine(Damage.MultiplicativeFusion, totalDamage, _kata.multiplyDamage.content);

        mainText += new CustomColumns(weaponDmgs, characterDmgs, kataDmgs, kataDmgs, totalDamage.ToArray().ToString(": ", "\n")).ToString();

        return mainText;
    }
    

    void CreateEquipButton(SlotItem _slotItem, int _index)
    {
        buttonsListActions.Add(subMenu.AddComponent<EventsCall>().Set("Equipar", ()=> {auxAction.Invoke(_slotItem, _index); } , ""));
        buttonsListActions[buttonsListActions.Count - 1].rectTransform.sizeDelta = new Vector2(180, 65);
    }

    void CreateUnequipButton(SlotItem _slotItem)
    {
        myListNavBar.SetLeftAuxButton("Desequipar", () =>
        {
            _slotItem.indexEquipedItem = -1;
            MenuManager.instance.modulesMenu.ObtainMenu<SubMenus>().TriggerOnClose();
        }, "");
    }

    void CreateButtonsActions(Item myItem, Dictionary<string, System.Action<Character, Item>> dic)
    {
        foreach (var item in dic)
        {
            buttonsListActions.Add(subMenu.AddComponent<EventsCall>().Set(item.Key, 
                () => {
                    item.Value(character, myItem);
                    CreateBody();
                }, ""));

            buttonsListActions[buttonsListActions.Count - 1].rectTransform.sizeDelta = new Vector2(300, 75);

            //subMenu.AddComponent<EventsCall>().Set(item.Key, () => item.Value(character), "");
        }
    }

    void ShowItemDetails(string nameDisplay, string details, Sprite Image)
    {       
        myDetailsW.SetTexts(nameDisplay,details).SetImage(Image);
        myDetailsW.SetActiveGameObject(true);
    }

    void FilterItems(string type)
    {
        /*
        foreach (var item in buttonsList)
        {
            if (item.type != type && type != "")
                item.SetActiveGameObject(false);
            else
                item.SetActiveGameObject(true);
        }
        */
        myDetailsW.SetActiveGameObject(false);
        DestroyButtonsActions();
    }
}