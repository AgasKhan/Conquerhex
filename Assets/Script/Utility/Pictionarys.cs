using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Internal;


/*
 Implementar sort
 */
[Serializable]
public class Pictionarys<K, V> : IEnumerable<Pictionary<K, V>>
{
    [SerializeField]
    List<Pictionary<K, V>> pictionaries;

    public int Count
    {
        get;
        private set;
    }

    public K[] keys
    {
        get
        {
            List<K> keys = new List<K>();
            foreach (var item in pictionaries)
            {
                keys.Add(item.key);
            }

            return keys.ToArray();
        }
    }

    public V[] values
    {
        get
        {
            List<V> values = new List<V>();
            foreach (var item in pictionaries)
            {
                values.Add(item.value);
            }

            return values.ToArray();
        }
    }

    /// <summary>
    /// busca por el orden de la lista
    /// </summary>
    /// <param name="k"></param>
    /// <returns></returns>
    public V this[int k]
    {
        get
        {
            return pictionaries[k].value;
        }

        set
        {
            pictionaries[k].value = value;
        }
    }

    /// <summary>
    /// busca por el nombre del enum, si coincide con el nombre del key
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public V this[Enum e]
    {
        get
        {
            return pictionaries[StringIndex(e)].value;
        }
        set
        {
            pictionaries[StringIndex(e)].value = value;
        }
    }

    /// <summary>
    /// Busca por el key ingresado
    /// </summary>
    /// <param name="k"></param>
    /// <returns></returns>
    public V this[K k]
    {
        get
        {
            var index = SearchIndex(k);
            if (index < 0)
                Debug.Log("se busco " + k.ToString() + " y no se encontro");
            return pictionaries[index].value;
        }
        set
        {
            pictionaries[SearchIndex(k)].value = value;
        }
    }

    public static Pictionarys<K, V> operator +(Pictionarys<K, V> original, Pictionarys<K, V> sumado)
    {
        original.AddRange(sumado);

        return original;
    }

    public static Pictionarys<K, V> operator +(Pictionarys<K, V> original, Dictionary<K, V> sumado)
    {
        foreach (var item in sumado)
        {
            original.Add(item.Key, item.Value);
        }
        return original;
    }

    public static Dictionary<K, V> operator +(Dictionary<K, V> original, Pictionarys<K, V> sumado)
    {
        foreach (var item in sumado)
        {
            original.Add(item.key, item.value);
        }
        return original;
    }

    public override string ToString()
    {
        return ToString("=");
    }

    public string ToString(string glue, string entreKeys = "\n\n")
    {
        string salida = "";

        foreach (var item in pictionaries)
        {
            if (!item.value.Equals(default(V)) && ((item.value as string) != ""))
                salida += item.key.ToString().RichText("u") + glue + item.value + entreKeys;
        }

        /*
        foreach (var item in pictionaries)
        {
            salida += item.key + glue + item.value + entreKeys;
        }
        */

        return salida;
    }


    /// <summary>
    /// devuelve el index en caso de encontrar similitud con el nombre del key
    /// </summary>
    /// <param name="s"></param>
    /// <returns>devuelve la poscion en un entero</returns>
    public int StringIndex(Enum s)
    {
        return StringIndex(s.ToString());
    }

    /// <summary>
    /// devuelve el index en caso de encontrar similitud con el nombre del key
    /// </summary>
    /// <param name="s"></param>
    /// <returns>devuelve la poscion en un entero</returns>
    public int StringIndex(string s)
    {
        for (int i = 0; i < pictionaries.Count; i++)
        {
            if (pictionaries[i].key.ToString() == s)
            {
                return i;
            }
        }
        return -1;
    }

    public void Sort(IComparer<Pictionary<K, V>> comparer)
    {
        pictionaries.Sort(comparer);
    }

    public Pictionary<K, V> GetPic(int index)
    {
        return pictionaries[index];
    }

    public bool TryGetPic(K key, out Pictionary<K,V> pic)
    {
        bool ret = ContainsKey(key, out int index);

        if (ret)
            pic = pictionaries[index];
        else
            pic = default;

        return ret;
    }

    public bool TryGetValue(K key, out V value)
    {
        bool ret = ContainsKey(key, out int index);

        if (ret)
            value = pictionaries[index].value;
        else
            value = default;

        return ret;
    }

    public bool ContainsKey(K key, out int index)
    {
        if ((index = SearchIndex(key)) > -1)
        {
            return true;
        }
        return false;
    }

    public bool ContainsKey(K key)
    {
        if (SearchIndex(key) > -1)
            return true;
        return false;
    }

    public void AddRange(IEnumerable<Pictionary<K, V>> pic)
    {
        pictionaries.AddRange(pic);
        int aux = 0;
        foreach (var item in pic)
        {
            aux++;
        }

        Count += aux;
    }

    public Pictionary<K, V> Add(K key, V value)
    {
        if (ContainsKey(key))
            return default;

        var aux = new Pictionary<K, V>(key, value);

        pictionaries.Add(aux);
        Count++;

        return aux;
    }

    public void Remove(K key)
    {
        for (int i = 0; i < pictionaries.Count; i++)
        {
            if (pictionaries[i].key.Equals(key))
            {
                pictionaries.RemoveAt(i);

                Count--;

                return;
            }
        }
    }

    public void RemoveAt(int i)
    {
        pictionaries.RemoveAt(i);
        Count--;
    }

    public void Clear()
    {
        pictionaries.Clear();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<Pictionary<K, V>> GetEnumerator()
    {
        return pictionaries.GetEnumerator();
    }


    public T SearchOrCreate<T>(K key) where T : V, new()
    {
        if (ContainsKey(key, out int index))
            return (T)pictionaries[index].value;

        var newAux = new T();

        //Debug.Log("se creo a partir de un new: " + newAux);

        Add(key, newAux);

        return newAux;
    }

    public void CreateOrSave(K key, V value)
    {
        if (ContainsKey(key, out int index))
        {
            pictionaries[index].value = value;
            return;
        }

        Add(key, value);
        //string completeNameClass = key.GetType().Namespace + "." + key.GetType().Name;
    }

    public V SearchOrDefault(K key, V defoult)
    {
        if (ContainsKey(key, out int index))
        {
            return pictionaries[index].value;
        }

        return defoult;
    }
    
    int SearchIndex(K key)
    {
        for (int i = 0; i < pictionaries.Count; i++)
        {
            if (pictionaries[i].key.Equals(key))
            {
                return i;
            }
        }

        return -1;
    }

    public Pictionarys()
    {
        pictionaries = new List<Pictionary<K, V>>();
    }
}



namespace Internal
{ 

    [System.Serializable]
    public class Pictionary<K, V> : IComparable<Pictionary<K, V>>
    {
        
        public K key;
        public V value;

        public int CompareTo(Pictionary<K, V> other)
        {
            return String.Compare(this.key.ToString(), other.key.ToString());
        }
        public Pictionary(K k, V v)
        {
            key = k;
            value = v;
        }
    }
}