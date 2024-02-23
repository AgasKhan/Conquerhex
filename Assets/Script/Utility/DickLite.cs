using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DickLite<Key, Value> : IEnumerable<Value>
{
    protected static int[] keyHashed;

    protected Value[] values;

    public Value this[Key key]
    {
        get
        {
            return values[GetIndex(key)];
        }
        set
        {
            values[GetIndex(key)] = value;
        }
    }

    public static void SetKeys(Key[] keys)
    {
        keyHashed = keys.Select((key) => key.GetHashCode()).OrderBy((key)=>key).ToArray();
    }

    public bool TryGetValue(Key key, out Value value)
    {
        value = this[key];

        return value != null;
    }


    protected int GetIndex(Key key)
    {
        return System.Array.BinarySearch(keyHashed, key.GetHashCode());
    }


    public void Init()
    {
        if (keyHashed == null)
            throw new System.TypeLoadException("No has seteado las Keys antes de crear el objeto");

        values = new Value[keyHashed.Length];
    }

    public IEnumerator<Value> GetEnumerator()
    {

        foreach (var item in values)
        {
            if(item!=null)
                yield return item;
        }

        //return ((IEnumerable<Value>)values).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return values.GetEnumerator();
    }
}

public class SuperDickLite<Value> : DickLite<System.Type, Value>
{
    public SuperDickLite()
    {
        if(keyHashed==null)
        {
            var types = System.AppDomain.CurrentDomain.GetAssemblies().SelectMany((assemply) => assemply.GetTypes()).Where(type => typeof(Value).IsAssignableFrom(type)).ToArray();

            SetKeys(types);

            string pantalla = string.Empty;

            foreach (var item in keyHashed)
            {
                pantalla += " " + item;
            }

            string dato;

            if(typeof(Value).IsGenericType)
            {
                 dato = typeof(Value).Name + " con el/los generico/s: " + typeof(Value).GetGenericArguments().Select(type => type.Name).Aggregate((str, str2) => str + '-' + str2);
            }
            else
            {
                dato = typeof(Value).Name;
            }

            Debug.Log($"Se seteo el keyHashed de {dato} con un total de keys: {keyHashed.Length}\nKeys: {pantalla}");
        }

        Init();
    }
}