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

                       var test1 = new CustomColumns(titulos);
                       Debug.Log(test1.ToString());

                       mainText += FormatColumns(titulos);

                       mainText += FormatColumns(MaxLengths(titulos), characterDmgs, "+", weaponDmgs, "x", kataDmgs);

                       var totalDamage = Damage.Combine(Damage.AdditiveFusion, auxKata.weaponEnabled.itemBase.damages, character.caster.additiveDamage.content);
                       var resultDmgs = totalDamage.ToArray().ToString(": ", "\n");

                       var test2 = new CustomColumns(totalDamage.ToArray().ToString(": ", "\n"));
                       Debug.Log(test2.ToString());

                       //var test3 = test1 + test2;
                       Debug.Log((totalDamage.ToArray().ToString(": ", "\n")).ToString() + test1);
                       Debug.Log((test1 + test2).ToString());
                       //Debug.Log((test2 + test1).ToString());
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

public class CustomColumns
{
    int maxRows;
    List<int> maxLengths;
    List<List<string>> columns = new List<List<string>>();

    public override string ToString()
    {
        string result = "";

        for (int i = 0; i < maxRows; i++)
        {
            for (int j = 0; j < columns.Count; j++)
            {
                string linea = i < columns[j].Count ? columns[j][i] : "";
                result += FormatString(linea, maxLengths[j]) + "\t";
            }
            result += "\n";
        }

        return result;
    }

    public static CustomColumns operator + (CustomColumns este, CustomColumns otro)
    {
        int index = 0;

        foreach (var item in otro.columns)
        {
            if (index >= este.columns.Count)
            {
                este.columns.Add(new List<string>());
                este.maxLengths.Add(0);
            }

            este.columns[index].AddRange(otro.columns[index]);
            este.maxLengths[index] = Mathf.Max(este.maxLengths[index], otro.maxLengths[index]);
            index++;
        }
        /*
        for (int i = 0; i < Mathf.Max(este.columns.Count, otro.columns.Count) ; i++)
        {
            Debug.Log("Este: " + (este.columns[i]==null) + " Otro: " + (otro.columns[i] == null) + " Index: " + i);
            Debug.Log(Mathf.Max(este.columns.Count, otro.columns.Count));

            if (i >= este.columns.Count)
            {
                este.columns.Add(new List<string>());
                este.maxLengths.Add(0);
            }


            este.columns[i].AddRange(otro.columns[i]);
            este.maxLengths[i] = Mathf.Max(este.maxLengths[i], otro.columns[i].Count);
        }
        */
        este.maxRows += otro.maxRows;
        return este;
    }
    public static CustomColumns operator + (CustomColumns este, IEnumerable<string> otro)
    {
        return AddRow(otro, este, (lista, item) => { lista.Add(item); });
    }

    public static CustomColumns operator + (IEnumerable<string> otro, CustomColumns este)
    {
        return AddRow(otro, este, (lista, item) => { lista.Insert(0, item); });
    }

    public static CustomColumns AddRow(IEnumerable<string> otro, CustomColumns este, System.Action<List<string>, string> action)
    {
        int index = 0;
        foreach (var item in otro)
        {
            if (index >= este.columns.Count)
            {
                este.columns.Add(new List<string>());
                este.maxLengths.Add(0);
            }

            action?.Invoke(este.columns[index], item);
            este.maxLengths[index] = Mathf.Max(este.maxLengths[index], item.Length);

            index++;
        }
        este.maxRows++;

        return este;
    }

    public void AddFirst(string texto, int columnIndex)
    {
        if (columnIndex >= 0 && columnIndex < columns.Count)
        {
            columns[columnIndex].Insert(0, texto);
            EqualizeRows();
        }
        else
            Debug.LogError("Indice de columna fuera de rango");
    }


    public void AddLast(string texto, int columnIndex)
    {
        if (columnIndex >= 0 && columnIndex < columns.Count)
        {
            for (int i = 0; i < columns[columnIndex].Count; i++)
            {
                if (columns[columnIndex][i].Replace(" ", "") == "")
                {
                    columns[columnIndex][i] = FormatString(texto, maxLengths[columnIndex]);
                    return;
                }
            }

            columns[columnIndex].Add(texto);

            if (columns[columnIndex].Count > maxRows)
                EqualizeRows();
        }
        else
        {
            Debug.LogError("Indice de columna fuera de rango");
        }
    }

    private void EqualizeRows()
    {
        for (int i = 0; i < columns.Count; i++)
        {
            if (columns[i].Count < maxRows)
            {
                columns[i].Add(FormatString("", maxLengths[i]));
            }
        }
    }

    List<int> MaxLengths()
    {
        maxRows = 0;
        maxLengths = new List<int>();

        for (int i = 0; i < columns.Count; i++)
        {
            maxRows = Mathf.Max(maxRows, columns[i].Count);
            Debug.Log("Max Lengths: " + maxLengths.Count + " Columns: " + columns.Count + " Index: "+i);
            maxLengths.Add(GetMaxLength(columns[i]));
        }

        return maxLengths;
    }

    int GetMaxLength(IEnumerable<string> lineas)
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

    public CustomColumns(params string[] columnas)
    {
        for (int j = 0; j < columnas.Length; j++)
        {
            columns.Add(new List<string>());
            columns[j].AddRange(columnas[j].Split('\n'));
        }

        MaxLengths();
    }

}
