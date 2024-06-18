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

    LisNavBarModule myListNavBar;

    DetailsWindow myDetailsW;

    System.Type filterType;

    public SlotItem slotItem = null;

    System.Action<SlotItem, int> action = null;

    public void SetEquipMenu<T>(SlotItem _slotItem, System.Type _type, System.Action<SlotItem, int> _action) where T : Item
    {
        action = _action;
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

    void CreateStatistics()
    {
        subMenu.TriggerOnClose();
    }

    private void InventoryOnClose()
    {
        slotItem = null;
        subMenu.OnClose -= InventoryOnClose;
    }

    void CreateBody()
    {
        /*
        subMenu.ClearBody();

        subMenu.CreateSection(0, 3);
        subMenu.CreateChildrenSection<ScrollRect>();

        CreateButtons();

        subMenu.CreateSection(3, 6);
        subMenu.CreateChildrenSection<ScrollRect>();
        myDetailsW = subMenu.AddComponent<DetailsWindow>();

        if(slotItem != null)
            subMenu.navbar.DestroyAll();

        if (buttonsList.Count <= 0)
            ShowItemDetails("", "No tienes nada que equipar", null);
        */

        subMenu.ClearBody();

        subMenu.CreateSection(0, 3);
        //subMenu.CreateChildrenSection<ScrollRect>();
        myListNavBar = subMenu.AddComponent<LisNavBarModule>();
        myListNavBar.SetTitle("Inventario");
        //myListNavBar.SetTags(new string[] { "Nombre", "Peso", "Peso Total", "Cantidad" });
        CreateButtons();

        subMenu.CreateSection(3, 6);
        subMenu.CreateChildrenSection<ScrollRect>();
        myDetailsW = subMenu.AddComponent<DetailsWindow>();

        if (slotItem != null)
            subMenu.navbar.DestroyAll();

        if (buttonsList.Count <= 0)
            ShowItemDetails("", "No tienes nada que equipar", null);
    }

    public void CreateButtons()
    {
        buttonsList.Clear();

        if (slotItem != null && slotItem.equiped != null)
        {
            CreateUnequipButton(slotItem);
        }

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
                   
                   if(slotItem != null)
                   {
                       CreateEquipButton(slotItem, index);
                   }

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

            var button = myListNavBar.AddButtonHor(item.image, item.nameDisplay, null, action);
            buttonsList.Add(button);
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

    void CreateEquipButton(SlotItem _slotItem, int _index)
    {
        buttonsListActions.Add(subMenu.AddComponent<EventsCall>().Set("Equipar", ()=> {action.Invoke(_slotItem, _index); } , ""));
        buttonsListActions[buttonsListActions.Count - 1].rectTransform.sizeDelta = new Vector2(300, 75);
    }

    void CreateUnequipButton(SlotItem _slotItem)
    {
        myListNavBar.ShowAuxButton("Desequipar", () =>
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

    string SetTextforItem(Item item)
    {
        string details;

        if (item is MeleeWeapon)
        {
            details = "Usos: " + ((MeleeWeapon)item).current;
        }
        else
        {
            details = item.GetCount().ToString() /*+ " / " + item.GetItemBase().maxAmount*/;
        }

        return details;
    }
}