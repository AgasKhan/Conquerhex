using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Version serializable de una lista ordenada
/// </summary>
/// <typeparam name="T"></typeparam>
[System.Serializable]
public class OrderedList<T> : List<T>, ISerializationCallbackReceiver where T : IComparable<T>
{
    [SerializeReference, Tooltip("Version de la lista serializada\nSolo existe para la serializacion, no es la lista real")]
    List<T> ts = new List<T>();

    Comparer comparer = new Comparer();

    /// <summary>
    /// Aniade a la lista ordenada un elemento respetando el orden <br/>
    /// es mas lento que una lista a la hora de aniadir
    /// </summary>
    /// <param name="item"></param>
    new public int Add(T item)
    {
        int index = BinarySearch(item);

        if (index < 0)
            index = ~index;
        /*
        else
        {
            index++;
        }
        */
        Insert(index, item);

        return index;
    }

    /// <summary>
    /// Verifica en base a una busqueda binaria si se encuentra en la lista ordenada
    /// </summary>
    /// <param name="item">Elemento a buscar</param>
    /// <returns>Retorna verdadero en caso que se encuentre</returns>
    new public bool Contains(T item)
    {
        return BinarySearch(item) >= 0;
    }

    /// <summary>
    /// Verifica en base a una busqueda binaria si se encuentra en la lista ordenada
    /// </summary>
    /// <param name="item">Elemento a buscar</param>
    /// <param name="index">retorna el indice del elemento</param>
    /// <returns>Retorna verdadero en caso que se encuentre</returns>
    public bool Contains<T2>(T2 item, out int index) where T2 : IComparable<T>
    {
        if (Contains(item, out index, out var end))
        {
            for (; index <= end; index++)
            {
                if (item.Equals(this[index]))
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Verifica en base a una busqueda binaria si se encuentra en la lista ordenada
    /// </summary>
    /// <param name="item">Elemento a buscar</param>
    /// <param name="start">retorna el comienzo del indice en el rango de los elementos iguales</param>
    /// <param name="end">retorna el fin del rango de los indices de los elementos iguales</param>
    /// <returns>Retorna verdadero en caso que se encuentre</returns>
    public bool Contains<T2>(T2 item, out int start, out int end) where T2 : IComparable<T>
    {
        int bn = BinarySearch(item);
        start = bn;
        end = start;

        if (bn < 0)
            return false;

        for (int i = bn; i >= 0; i--)
        {
            if (item.CompareTo(this[i]) == 0)
                start = i;
            else
                break;
        }

        for (int i = bn; i < Count; i++)
        {
            if (item.CompareTo(this[i]) == 0)
                end = i;
            else
                break;
        }

        return true;
    }

    /// <summary>
    /// Aniade todos los elementos del rango, de forma ordenada <br/>
    /// utiliza el add de forma interna para lograrlo
    /// </summary>
    /// <param name="range"></param>
    new public void AddRange(IEnumerable<T> range)
    {
        foreach (var item in range)
        {
            Add(item);
        }
    }

    /// <summary>
    /// Remueve el item de la lista, buscandolo por medio de busqueda binaria<br/>
    /// al removerlo se reacomoda la lista
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new public int Remove(T item)
    {
        if (Contains(item, out int start, out int end))
        {
            for (int i = start; i <= end; i++)
            {
                if(item.Equals(this[i]))
                {
                    RemoveAt(i);
                    return i;
                }   
            }
        }

        return -1;
    }

    public int BinarySearch<T2>(T2 item) where T2 : IComparable<T>
    {
        comparer.comparable = item;

        int left = 0;
        int right = Count - 1;

        while (left <= right)
        {
            int mid = left + (right - left) / 2;

            //No me importa el segundo elemento por que por eso ya trabajo con el comparador
            int comparisonResult = comparer.Compare(this[mid], default(T));

            if (comparisonResult == 0)
            {
                // Se encontró el elemento en la posición 'mid'
                return mid;
            }
            else if (comparisonResult < 0)
            {
                // El elemento está en el lado derecho del medio
                left = mid + 1;
            }
            else
            {
                // El elemento está en el lado izquierdo del medio
                right = mid - 1;
            }
        }

        // El elemento no se encontró, devuelve el complemento del índice donde se insertaría
        return ~left;
    }

    public void OnAfterDeserialize()
    {
        Clear();
        AddRange(ts);
        ts.TrimExcess();
        TrimExcess();
    }

    public void OnBeforeSerialize()
    {
        ts.Clear();
        ts.AddRange(this);
    }

    class Comparer : IComparer<T>
    {
        public IComparable<T> comparable;

        public int Compare(T x, T y)
        {
            return comparable.CompareTo(x) * -1;
        }
    }
}