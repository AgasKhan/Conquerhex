using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class InventorySubMenu : CreateSubMenu
{
    public Character character;

    public ScrollVertComponent itemList;

    List<ButtonA> buttonsList = new List<ButtonA>();

    List<EventsCall> buttonsListActions = new List<EventsCall>();

    DetailsWindow myDetailsW;

    public void ExchangeItems(InventoryEntityComponent playerInv, InventoryEntityComponent storageInv, Item itemToMove)
    {
        itemToMove.GetAmounts(out int actual, out int max);
        playerInv.AddOrSubstractItems(itemToMove.nameDisplay, -actual);
        storageInv.AddOrSubstractItems(itemToMove.nameDisplay, actual);
    }

    protected override void InternalCreate()
    {
        subMenu.navbar.DestroyAll();

        if(filter!= null)
        {

        }
        else
        {
            subMenu.AddNavBarButton("All", ButtonAct).AddNavBarButton("Equipment", () => { ButtonAct(ResourceType.Equipment.ToString()); })
                    .AddNavBarButton("Mineral", () => { ButtonAct(ResourceType.Mineral.ToString()); }).AddNavBarButton("Gemstone", () => { ButtonAct(ResourceType.Gemstone.ToString()); })
                    .AddNavBarButton("Other", () => { ButtonAct(ResourceType.Other.ToString()); });

            subMenu.CreateTitle("Inventory");

            CreateBody();
        }
    }

    void CreateBody(ItemBase filter = null)
    {
        subMenu.ClearBody();

        subMenu.CreateSection(0, 3);
        subMenu.CreateChildrenSection<ScrollRect>();

        CreateButtons(filter);

        subMenu.CreateSection(3, 6);
        //subMenu.CreateChildrenSection<ScrollRect>();
        myDetailsW = subMenu.AddComponent<DetailsWindow>();
    }

    public void CreateButtons(ItemBase filter = null)
    {
        buttonsList.Clear();

        for (int i = 0; i < character.inventory.inventory.Count; i++)
        {
            if (filter != null && character.inventory.inventory[i].GetItemBase() != filter)
                continue;


            ButtonA button = subMenu.AddComponent<ButtonA>();

            var item = character.inventory.inventory[i];

            var index = i;

            UnityEngine.Events.UnityAction action =
               () =>
               {
                   ShowItemDetails(item.nameDisplay, item.GetDetails().ToString("\n"), item.image);
                   DestroyButtonsActions();
                   //CreateButtonsActions(item, item.GetItemBase().buttonsAcctions);
                   CreateButtonEquip(()=>equipAction(indexWeapon, (MeleeWeapon)item));

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
               };

            buttonsList.Add(button.SetButtonA(item.nameDisplay, item.image, SetTextforItem(item), action).SetType(item.itemType.ToString()));
        }
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

    void CreateButtonEquip(System.Action _action)
    {

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

    void ButtonAct(string type)
    {
        foreach (var item in buttonsList)
        {
            if (item.type != type && type != "")
                item.SetActiveGameObject(false);
            else
                item.SetActiveGameObject(true);
        }

        myDetailsW.SetActiveGameObject(false);
        DestroyButtonsActions();
    }

    void ButtonAct()
    {
        ButtonAct("");
    }

    string SetTextforItem(Item item)
    {
        string details = "";

        if (item is MeleeWeapon)
        {
            details = "Uses: " + ((MeleeWeapon)item).current;
        }
        else
        {
            item.GetAmounts(out int actual, out int max);
            details = actual + " / " + max;
        }

        return details;
    }

    ItemBase filter;
    System.Action<int, MeleeWeapon> equipAction;
    int indexWeapon;
    public void SetFilter(ItemBase _filter)
    {
        filter = _filter;
    }
    public void SetEquipAct(System.Action<int, MeleeWeapon> _action, int _index)
    {
        equipAction = _action;
        indexWeapon = _index;
    }

}