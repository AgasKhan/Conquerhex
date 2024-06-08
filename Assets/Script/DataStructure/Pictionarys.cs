using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Internal;
using Unity.Jobs;
using Unity.Collections;

[Serializable]
public class Pictionarys<K, V> : IList<Pictionary<K, V>>
{
    [SerializeField]
    List<Pictionary<K, V>> pictionaries;

    Stack<Pictionary<K, V>> auxiliarObjects;

    public int Count
    {
        get => pictionaries.Count;
    }

    public K[] keys
    {
        get
        {
            K[] keys = new K[Count];
            for (int i = 0; i < Count; i++)
            {
                keys[i] = pictionaries[i].key;
            }

            return keys;
        }
    }

    public V[] values
    {
        get
        {
            V[] values = new V[Count];

            for (int i = 0; i < Count; i++)
            {
                values[i] = pictionaries[i].value;
            }

            return values;
        }
    }

    public bool IsReadOnly => throw new NotImplementedException();

    Pictionary<K, V> IList<Pictionary<K, V>>.this[int index] { get => pictionaries[index]; set => pictionaries[index] = value; }

    /// <summary>
    /// busca por el orden de la lista
    /// </summary>
    /// <param name="k"></param>
    /// <returns></returns>
    public V this[int k]
    {
        get
        {
            return GetByIndex(k);
        }

        set
        {
            SetByIndex(k, value);
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
            return GetByKey(k);
        }
        set
        {
            SetByKey(k, value);
        }
    }


    public V GetByIndex(int index)
    {
        return pictionaries[index].value;
    }

    public void SetByIndex(int index, V value)
    {
        pictionaries[index].value = value;
    }

    public V GetByKey(K k)
    {
        var index = SearchIndex(k);
        if (index < 0)
            Debug.Log("se busco " + k.ToString() + " y no se encontro");
        return pictionaries[index].value;
    }

    public void SetByKey(K k, V value)
    {
        pictionaries[SearchIndex(k)].value = value;
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


    public void Sort(IComparer<Pictionary<K, V>> comparer)
    {
        pictionaries.Sort(comparer);
    }

    public Pictionary<K, V> GetPic(int index)
    {
        return pictionaries[index];
    }

    public void SetPic(int index, Pictionary<K, V> pic)
    {
        pictionaries[index] = pic;
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

    public bool Contains(Pictionary<K, V> item)
    {
        return ContainsKey(item.key);
    }

    public int IndexOf(Pictionary<K, V> item)
    {
        int i = -1;

        ContainsKey(item.key, out i);

        return i;
    }

    public void Insert(int index, K key,V value)
    {
        Pictionary<K, V> aux;

        if (auxiliarObjects.Count > 0)
        {
            aux = auxiliarObjects.Pop();

            aux.key = key;
            aux.value = value;
        }
        else
        {
            aux = new Pictionary<K, V>(key, value);
        }

        pictionaries.Insert(index, aux);
    }

    public void Insert(int index, Pictionary<K, V> item)
    {
        pictionaries.Insert(index, item);
    }

    public void Add(Pictionary<K, V> item)
    {
        pictionaries.Add(item);
    }

    public Pictionary<K, V> Add(K key, V value)
    {
        if (ContainsKey(key))
            return default;

        Pictionary<K, V> aux;

        if (auxiliarObjects.Count>0)
        {
            aux = auxiliarObjects.Pop();

            aux.key = key;
            aux.value = value;
        }
        else
        {
            aux = new Pictionary<K, V>(key, value);
        }        

        pictionaries.Add(aux);

        return aux;
    }

    public void AddRange(IEnumerable<Pictionary<K, V>> pic)
    {
        pictionaries.AddRange(pic);
    }

    public void Remove(K key)
    {
        for (int i = 0; i < pictionaries.Count; i++)
        {
            if (pictionaries[i].key.Equals(key))
            {
                RemoveAt(i);

                return;
            }
        }
    }

    public void RemoveAt(int i)
    {
        var aux = pictionaries[i];

        pictionaries.RemoveAt(i);

        auxiliarObjects.Push(aux);
    }

    public bool Remove(Pictionary<K, V> item)
    {
        return pictionaries.Remove(item);
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
        if (!ContainsKey(key, out int index) || pictionaries[index].value == null)
        {
            var newAux = new T();

            if (index < 0)
                Add(key, newAux);
            else
                pictionaries[index].value = newAux;

            return newAux;
        }

        return (T)pictionaries[index].value;
    }

    public void CreateOrSave(K key, V value)
    {
        if (ContainsKey(key, out int index))
        {
            pictionaries[index].value = value;
            return;
        }

        Add(key, value);
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

    public void CopyTo(Pictionary<K, V>[] array, int arrayIndex)
    {
        pictionaries.CopyTo(array, arrayIndex);
    }

    public Pictionarys()
    {
        pictionaries = new List<Pictionary<K, V>>();

        auxiliarObjects = new Stack<Pictionary<K, V>>();
    }
}



namespace Internal
{
    public class RandomDataPic<T>
    {
        [Unity.Burst.BurstCompile]
        public struct MyParallelJob : IJobParallelFor
        {

            [Unity.Collections.LowLevel.Unsafe.NativeDisableContainerSafetyRestriction] 

            public NativeArray<Unity.Mathematics.Random> randomNumberGenerators;

            [Unity.Collections.LowLevel.Unsafe.NativeSetThreadIndex] 
            int threadId;
            /*
            [ReadOnly]
            public NativeArray<float> randomNumberGenerators;
            */
            [ReadOnly]
            public NativeArray<int> weights;

            [WriteOnly]
            public NativeArray<int> result;

            public float acumTotal;

            public void Execute(int index)
            {
                var random = randomNumberGenerators[threadId];

                float rng = random.NextFloat();

                randomNumberGenerators[threadId] = random;

                float acumPercentage = 0;

                int lastItem = -1;

                for (int i = 0; i < weights.Length; i++)
                {
                    var item = weights[i];

                    acumPercentage += item / acumTotal;

                    if (rng <= acumPercentage)
                    {
                        result[index] = i;
                        return;
                    }

                    lastItem = i;
                }

                result[index] = lastItem;
            }
        }

        NativeArray<Unity.Mathematics.Random> randomNumberGenerators;

        //NativeArray<float> randomNumberGenerators;

        Pictionarys<T, int> pictionary;

        float acumTotal = 0;
        
        [BurstCompatible]
        public T[] ParallelRandom(int lenght)
        {
            T[] result = new T[lenght];
            /*

            randomNumberGenerators = new NativeArray<float>(lenght, Allocator.Persistent);

            for (int i = 0; i < randomNumberGenerators.Length; i++)
            {
                randomNumberGenerators[i] = UnityEngine.Random.Range(0f,1f);
            }
            */
            NativeArray<int> weights = new NativeArray<int>(pictionary.values, Allocator.TempJob);
            NativeArray<int> indexResult = new NativeArray<int>(lenght, Allocator.TempJob);

            MyParallelJob job = new MyParallelJob();

            job.randomNumberGenerators = randomNumberGenerators;

            job.weights = weights;

            job.result = indexResult;

            job.acumTotal = acumTotal;

            JobHandle handle = job.Schedule(indexResult.Length, 32);

            handle.Complete();

            for (int i = 0; i < indexResult.Length; i++)
            {
                result[i] = pictionary.GetPic(indexResult[i]).key;
            }

            weights.Dispose();
            indexResult.Dispose();
            randomNumberGenerators.Dispose();

            return result;
        }

        public T Random()
        {
            float acumPercentage = 0;

            float rng = UnityEngine.Random.Range(0, 1f);

            T lastItem = default;

            foreach (var item in pictionary)
            {
                acumPercentage += item.value / acumTotal;

                if (rng <= acumPercentage)
                {
                    return item.key;
                }

                lastItem = item.key;
            }

            return lastItem;
        }



        ~RandomDataPic()
        {
            randomNumberGenerators.Dispose();
        }


        public RandomDataPic(Pictionarys<T, int> pictionary)
        {
      
            randomNumberGenerators = new NativeArray<Unity.Mathematics.Random>(Unity.Jobs.LowLevel.Unsafe.JobsUtility.MaxJobThreadCount, Allocator.Persistent);

            for (int i = 0; i < randomNumberGenerators.Length; i++)
            {
                randomNumberGenerators[i] = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, int.MaxValue));
            }

            this.pictionary = pictionary;

            foreach (var item in pictionary)
            {
                acumTotal += item.value;
            }
        }
    }

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


/*
 
    public class RandomDataPic<T>
    {
        public struct MyParallelJob : IJobParallelFor
        {



[ReadOnly]
public NativeArray<float> randomNumberGenerators;

[ReadOnly]
public NativeArray<int> weights;

[WriteOnly]
public NativeArray<int> result;

public float acumTotal;

public void Execute(int index)
{
    float rng = randomNumberGenerators[index];

    float acumPercentage = 0;

    int lastItem = -1;

    for (int i = 0; i < weights.Length; i++)
    {
        var item = weights[i];

        acumPercentage += item / acumTotal;

        if (rng <= acumPercentage)
        {
            result[index] = i;
            return;
        }

        lastItem = i;
    }

    result[index] = lastItem;
}
        }

        //NativeArray<Unity.Mathematics.Random> randomNumberGenerators;

        NativeArray<float> randomNumberGenerators;

Pictionarys<T, int> pictionary;

float acumTotal = 0;

[BurstCompatible]
public T[] ParallelRandom(int lenght)
{
    T[] result = new T[lenght];

    randomNumberGenerators = new NativeArray<float>(lenght, Allocator.Persistent);

    for (int i = 0; i < randomNumberGenerators.Length; i++)
    {
        randomNumberGenerators[i] = UnityEngine.Random.Range(0f, 1f);
    }

    NativeArray<int> weights = new NativeArray<int>(pictionary.values, Allocator.TempJob);
    NativeArray<int> indexResult = new NativeArray<int>(lenght, Allocator.TempJob);

    MyParallelJob job = new MyParallelJob();

    job.randomNumberGenerators = randomNumberGenerators;

    job.weights = weights;

    job.result = indexResult;

    job.acumTotal = acumTotal;

    JobHandle handle = job.Schedule(indexResult.Length, 32);

    handle.Complete();

    for (int i = 0; i < indexResult.Length; i++)
    {
        result[i] = pictionary.GetPic(indexResult[i]).key;
    }

    weights.Dispose();
    indexResult.Dispose();
    randomNumberGenerators.Dispose();

    return result;
}

public T Random()
{
    float acumPercentage = 0;

    float rng = UnityEngine.Random.Range(0, 1f);

    T lastItem = default;

    foreach (var item in pictionary)
    {
        acumPercentage += item.value / acumTotal;

        if (rng <= acumPercentage)
        {
            return item.key;
        }

        lastItem = item.key;
    }

    return lastItem;
}



public RandomDataPic(Pictionarys<T, int> pictionary)
{




    this.pictionary = pictionary;

    foreach (var item in pictionary)
    {
        acumTotal += item.value;
    }
}
    }
 */