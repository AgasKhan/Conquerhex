using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObjects : MonoBehaviour
{
    [System.Serializable]
    public class Category
    {
        [Header("Nombre de la clase")]
        public string name;

        [SerializeField]
        public BasePool[] basePools;
    }

    [System.Serializable]
    public class BasePool
    {
        [Header("configuracion")]

        public GameObject prefab;
        public Object[] utilityRefence;
        public int amount;

        [Header("Interna")]
        int _index = 0;

        [SerializeReference]
        PoolObj[] pool;

        public int index
        {
            get
            {
                int aux = _index;
                _index++;
                if (_index >= pool.Length)
                    _index = 0;

                return aux;
            }
        }

        public T SpawnPoolObj<T>(out Transform go) where T : Object
        {
            var aux = pool[index];
            go = aux.Obj.transform;

            foreach (var item in aux.auxiliarReference)
            {
                if (item is T)
                    return (T)item;
            }
            return default;
        }

        public Transform SpawnPoolObj()
        {
            return pool[index].Obj.transform;
        }

        public void Init()
        {
            pool = new PoolObj[amount];

            for (int i = 0; i < pool.Length; i++)
            {
                pool[i] = new PoolObj(prefab, utilityRefence);
            }
        }
    }

    [System.Serializable]
    public class PoolObj
    {
        public GameObject Obj;
        public Object[] auxiliarReference;

        public PoolObj(GameObject prefab, Object[] utilityRefence)
        {
            Obj = Instantiate(prefab);

            auxiliarReference = new Object[utilityRefence.Length];

            for (int i = 0; i < utilityRefence.Length; i++)
            {
                auxiliarReference[i] = Obj.GetComponent(utilityRefence[i].GetType());
            }

            Obj.SetActive(false);
        }
    }

    [Header("Activar generacion")]
    public bool eneabled;

    [SerializeField]
    Category[] categoriesOfPool;


    static PoolObjects instance;

    #region busqueda por categoria

    /// <summary>
    /// Devuelve los indices de la categoria y el objeto del pool
    /// </summary>
    /// <param name="type">nombre de la clase/categoria del objeto</param>
    /// <param name="powerObject">nombre del prefab del objeto</param>
    /// <returns></returns>
    static public Vector2Int SrchInCategory(string type, string powerObject)
    {
        return SrchInCategory(SrchInCategory(type), powerObject);   
    }

    /// <summary>
    /// devuelve el indice de la categoria dentro del pool
    /// </summary>
    /// <param name="word">nombre de la clase/categoria del objeto</param>
    /// <returns></returns>
    static public int SrchInCategory(string word)
    {
        for (int i = 0; i < instance.categoriesOfPool.Length; i++)
        {
            if (instance.categoriesOfPool[i].name == word)
            {
                return i;
            }
        }
        Debug.LogWarning("Error categoria no encontrada: " + word);
        return -1;
    }

    /// <summary>
    /// devuelve el indice de la categoria dentro del pool
    /// </summary>
    /// <param name="index">indice de la categoria</param>
    /// <param name="powerObject">nombre del prefab del objeto</param>
    /// <returns></returns>
    static public Vector2Int SrchInCategory(int index, string powerObject)
    {
        Vector2Int indexsFind = new Vector2Int(index, -1);

        for (int ii = 0; ii < instance.categoriesOfPool[index].basePools.Length; ii++)
        {
            if (instance.categoriesOfPool[index].basePools[ii].prefab.name == powerObject)
            {
                indexsFind.y = ii;
                return indexsFind;
            }
        }
        Debug.LogWarning("No se encontro el objeto: " + powerObject);
        return indexsFind;

    }

    #endregion

    #region "Spawn" pool objects

    static public T SpawnPoolObject<T>(int categoryIndex, string powerObject, Vector3 pos, Quaternion angles) where T : Object
    {
        Vector2Int indexs = SrchInCategory(categoryIndex, powerObject);

        return SpawnPoolObject<T>(indexs, pos, angles);
    }

    static public GameObject SpawnPoolObject(int categoryIndex, string powerObject, Vector3 pos, Quaternion angles)
    {
        Vector2Int indexs = SrchInCategory(categoryIndex, powerObject);

        return SpawnPoolObject(indexs, pos, angles);
    }

    static public GameObject SpawnPoolObject(string type, string powerObject, Vector3 pos, Quaternion angles)
    {
        Vector2Int indexs = SrchInCategory(type, powerObject);

        return SpawnPoolObject(indexs, pos, angles);
    }

    static public GameObject SpawnPoolObject(Vector2Int indexs, Vector3 pos, Quaternion angles, Transform padre = null)
    {

        if (indexs.x < 0)
        {
            Debug.LogWarning("categoria no encontrada");
            return null;
        }
        else if (indexs.y < 0)
        {
            Debug.LogWarning("Objeto no encontrado");
            return null;
        }

        var pool = instance.categoriesOfPool[indexs.x].basePools[indexs.y];

        Transform transformObject = pool.SpawnPoolObj();

        transformObject.parent = padre;
        transformObject.localPosition = pos;
        transformObject.localRotation = angles;
        transformObject.gameObject.SetActive(true);

        return transformObject.gameObject;
    }

    static public T SpawnPoolObject<T>(Vector2Int indexs, Vector3 pos, Quaternion angles, Transform padre = null) where T : Object
    {

        if (indexs.x < 0)
        {
            Debug.LogWarning("categoria no encontrada");
            return default;
        }
        else if (indexs.y < 0)
        {
            Debug.LogWarning("Objeto no encontrado");
            return default;
        }

        var pool = instance.categoriesOfPool[indexs.x].basePools[indexs.y];

        T obj = pool.SpawnPoolObj<T>(out Transform transform);

        transform.parent = padre;
        transform.localPosition = pos;
        transform.localRotation = angles;
        transform.gameObject.SetActive(true);

        return obj;
    }
    #endregion

    void Start()
    {
        instance = this;

        if (!eneabled)
            return;

        foreach (var item in categoriesOfPool)
        {
            foreach (var subitem in item.basePools)
            {
                subitem.Init();
            }
        }
    }

}

/*

static public GameObject SrchPowerObject(string type, string powerObject)
    {
        for (int i = 0; i < instance.powerObjects.Length; i++)
        {
            if(instance.powerObjects[i].classType==type)
            {
                for (int ii = 0; ii < instance.powerObjects[i].GO.Length; ii++)
                {
                    if (instance.powerObjects[i].GO[ii].name == powerObject)
                    {
                        return instance.powerObjects[i].GO[ii];
                    }
                }
            }
        }
        return null;
    }
    

    static public GameObject SpawnPowerObject(string type, string powerObject, Vector3 pos, Quaternion angles)
    {
        GameObject aux = SrchPowerObject(type, powerObject);

        if(aux==null)
        {
            Debug.LogWarning("Objeto no encontrado");
            return null;
        }

        return Instantiate(aux, pos, angles);
    } 

 */