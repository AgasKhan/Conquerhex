using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class HybridArray<V>
{
    class SetWrapper
    {
        public bool set = true;

        public V value;

        public override bool Equals(object obj)
        {
            return value.Equals(obj);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public SetWrapper(V value)
        {
            this.value = value;
        }
    }

    Func<V, V, bool> fusion;

    Func<V[]> staticArray;

    List<SetWrapper> dynamicArray = new List<SetWrapper>();

    int _count = 0;

    public IEnumerable<V> content
    {
        get
        {
            var list = dynamicArray.Where(wrapper => wrapper.set).Select(wrapper => wrapper.value).Concat(staticArray()).ToList();

            for (int i = list.Count - 1; i >= 0; i--)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    if (fusion(list[i], list[j]))
                        list.RemoveAt(j);
                }

                yield return list[i];
            }
        }
    }

    public int Count => staticArray().Length + _count;

    public V this[int index]
    {
        get
        {
            if (index >= staticArray().Length)
            {
                while (!dynamicArray[index].set && index < dynamicArray.Count)
                {
                    index++;
                }

                return dynamicArray[index].value;
            }

            return staticArray()[index];
        }
    }



    public void Add(V item)
    {
        _count++;

        for (int i = 0; i < dynamicArray.Count; i++)
        {
            if (!dynamicArray[i].set)
            {
                dynamicArray[i].value = item;
                dynamicArray[i].set = true;
                return;
            }
        }

        dynamicArray.Add(new SetWrapper(item));
    }

    public void Remove(V item)
    {
        _count--;

        for (int i = 0; i < dynamicArray.Count; i++)
        {
            if (dynamicArray[i].Equals(item))
                dynamicArray[i].set = false;
        }
    }

    public HybridArray(Func<V[]> staticArray)
    {
        this.staticArray = staticArray;
    }
}