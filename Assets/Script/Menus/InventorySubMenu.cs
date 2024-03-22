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

        subMenu.AddNavBarButton("All", ButtonAct).AddNavBarButton("Equipment", () => { ButtonAct(ResourceType.Equipment.ToString()); })
                    .AddNavBarButton("Mineral", () => { ButtonAct(ResourceType.Mineral.ToString()); }).AddNavBarButton("Gemstone", () => { ButtonAct(ResourceType.Gemstone.ToString()); })
                    .AddNavBarButton("Other", () => { ButtonAct(ResourceType.Other.ToString()); });

        subMenu.CreateTitle("Inventory");

        CreateBody();
    }

    void CreateBody()
    {
        subMenu.ClearBody();

        subMenu.CreateSection(0, 3);
        subMenu.CreateChildrenSection<ScrollRect>();

        CreateButtons();

        subMenu.CreateSection(3, 6);
        //subMenu.CreateChildrenSection<ScrollRect>();
        myDetailsW = subMenu.AddComponent<DetailsWindow>();
    }

    public void CreateButtons()
    {
        buttonsList.Clear();

        for (int i = 0; i < character.inventory.inventory.Count; i++)
        {
            ButtonA button = subMenu.AddComponent<ButtonA>();

            var item = character.inventory.inventory[i];

            var index = i;

            UnityEngine.Events.UnityAction action =
               () =>
               {
                   ShowItemDetails(item.nameDisplay, item.GetDetails().ToString("\n"), item.image);
                   DestroyButtonsActions();
                   CreateButtonsActions(item, item.GetItemBase().buttonsAcctions);

                   if(item.GetItemBase() is WeaponKataBase)
                   {
                       WeaponKata auxKata = (WeaponKata)item;

                       string mainText = "------------------------------------------------------------------\n";

                       var kataDmgs = auxKata.multiplyDamage.content.ToArray().ToString(": x", "\n");
                       var characterDmgs = character.caster.additiveDamage.content.ToArray().ToString(": ", "\n");
                       var weaponDmgs = auxKata.weaponEnabled.itemBase.damages.ToString(": ", "\n");

                       mainText += "Kata Selected: " + item.nameDisplay + "\n";

                       var titulos = new string[] { "Character damages", "operacion", "Weapon equiped damages", "operacion", "Kata Selected" };

                       mainText += FormatColumns(titulos);

                       mainText += FormatColumns(MaxLengths(titulos), characterDmgs, "+", weaponDmgs, "x", kataDmgs);

                       var totalDamage = Damage.Combine(Damage.AdditiveFusion, auxKata.weaponEnabled.itemBase.damages, character.caster.additiveDamage.content);
                       var resultDmgs = totalDamage.ToArray().ToString(": ", "\n");
                       //mainText += resultDmgs;

                       mainText += "\nCharacter and weapon combined damages:\n";
                       var charAndWeapResult = FormatColumns(characterDmgs, "+\n+\n+", weaponDmgs, "=\n=\n=", resultDmgs);
                       mainText += charAndWeapResult;

                       totalDamage = Damage.Combine(Damage.MultiplicativeFusion, totalDamage, auxKata.multiplyDamage.content);

                       mainText += "\nCharacter/Weapon and Kata combined damages:\n";
                       var resultAndKata = totalDamage.ToArray().ToString(": ", "\n");
                       mainText += FormatColumns(resultDmgs, "x\nx\nx", kataDmgs, "=\n=\n=", resultAndKata);


                       mainText += "\nAll damages operations:\n";
                       mainText += FormatColumns(charAndWeapResult, "x\nx\nx", kataDmgs, "=\n=\n=", resultAndKata);

                       mainText += "\n------------------------------------------------------------------";

                       Debug.Log(mainText);

                   }
               };

            buttonsList.Add(button.SetButtonA(item.nameDisplay, item.image, SetTextforItem(item), action).SetType(item.itemType.ToString()));
        }
    }
    /*
     
    objeto => columnas

    columnas addFirst(titulo)

    columnas calcule el ancho mas conveniente()

    ToString()
     
     */

    string FormatColumns(int[] maxLengths, params string[] columnas)
    {
        string resultado = "";
        int numLineas = Mathf.Max(columnas.Select(c => c.Split('\n').Length).ToArray());
        for (int i = 0; i < numLineas; i++)
        {
            for (int j = 0; j < columnas.Length; j++)
            {
                string[] lineas = columnas[j].Split('\n');
                string linea = i < lineas.Length ? lineas[i] : "";
                resultado += FormatString(linea, maxLengths[j]) + "\t";
            }
            resultado += "\n";
        }

        return resultado;
    }

    string FormatColumns(params string[] columnas)
    {
        return FormatColumns(MaxLengths(columnas), columnas);
    }

    int[] MaxLengths(params string[] columnas)
    {
        int[] maxLengths = new int[columnas.Length];

        for (int i = 0; i < columnas.Length; i++)
        {
            string[] lineas = columnas[i].Split('\n');
            maxLengths[i] = GetMaxLength(lineas);
        }

        return maxLengths;
    }

    int GetMaxLength(string[] lineas)
    {
        int maxLength = 0;
        foreach (string linea in lineas)
        {
            maxLength = Mathf.Max(maxLength, linea.FixedLength());
        }
        return maxLength;
    }

    string FormatString(string texto, int longitudMaxima)
    {
        int largo = texto.FixedLength();

        if (largo <= longitudMaxima)
            return texto + new string(' ', (longitudMaxima - largo));

        return texto.Substring(0, longitudMaxima - 3) + "...";
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

}
