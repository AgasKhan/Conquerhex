using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class OrderedList<T> : List<T>, ISerializationCallbackReceiver where T : IComparable
{
    [SerializeField]
    List<T> ts = new List<T>();

    new public void Add(T item)
    {
        int index = BinarySearch(item);

        if (index < 0)
            index = ~index;
        else
        {
            index++;
        }

        Insert(index, item);
    }

    new public bool Contains(T item)
    {
        return BinarySearch(item) >= 0;
    }

    public bool Contains(T item, out int index)
    {
        index = BinarySearch(item);
        return index >= 0;
    }

    new public void AddRange(IEnumerable<T> range)
    {
        foreach (var item in range)
        {
            Add(item);
        }
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
}