using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DickLite<Key, Value>
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

        return value == null;
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
}

public class SuperDickLite<Value> : DickLite<System.Type, Value>
{
    public SuperDickLite()
    {
        if(keyHashed==null)
        {
            var types = System.AppDomain.CurrentDomain.GetAssemblies().SelectMany((assemply) => assemply.GetTypes()).Where(type => type.IsSubclassOf(typeof(Value))).ToArray();

            SetKeys(types);
        }

        Init();
    }
}