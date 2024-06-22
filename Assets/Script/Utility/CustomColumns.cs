using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public static CustomColumns operator +(CustomColumns este, CustomColumns otro)
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

        este.maxRows += otro.maxRows;
        return este;
    }
    public static CustomColumns operator +(CustomColumns este, IEnumerable<string> otro)
    {
        return AddRow(otro, este, (lista, item) => { lista.Add(item); });
    }

    public static CustomColumns operator +(IEnumerable<string> otro, CustomColumns este)
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

    public CustomColumns AddLeft(CustomColumns newColumns)
    {
        int index = 0;
        foreach (var item in newColumns.columns)
        {
            this.columns.Insert(index, item);
            maxLengths.Insert(index, newColumns.maxLengths[index]);
            index++;
        }
        maxRows = Mathf.Max(maxRows, newColumns.maxRows);

        return this;
    }
    public CustomColumns AddRight(CustomColumns newColumns)
    {
        int index = 0;
        foreach (var item in newColumns.columns)
        {
            this.columns.Add(item);
            maxLengths.Add(newColumns.maxLengths[index]);
            index++;
        }
        maxRows = Mathf.Max(maxRows, newColumns.maxRows);

        return this;
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
            return texto + new string('_', (longitudMaxima - largo));
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