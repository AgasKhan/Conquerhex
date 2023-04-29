using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [System.Serializable]
    public class Category
    {
        [Header("Nombre de la clase")]
        public string name;

        [SerializeField]
        public PoolObjects[] objectPool;
    }

    [System.Serializable]
    public class PoolObjects
    {
        [Header("configuracion")]

        public GameObject prefab;
        public Object[] utilityRefence;
        public int amount;

        [Header("Interna")]
        int _index = 0;

        [SerializeReference]
        ObjectRefence[] pool;

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

        public Transform SpawnPoolObj<T>(out T go) where T : Object
        {
            var aux = pool[index];
            go = default;

            foreach (var item in aux.auxiliarReference)
            {
                if (item is T)
                {
                    go = (T)item;
                    break;
                }
            }
            return aux.Obj.transform;
        }

        public Transform SpawnPoolObj()
        {
            return pool[index].Obj.transform;
        }

        public void Init()
        {
            pool = new ObjectRefence[amount];

            for (int i = 0; i < pool.Length; i++)
            {
                pool[i] = new ObjectRefence(prefab, utilityRefence);
            }
        }
    }

    [System.Serializable]
    public class ObjectRefence
    {
        public GameObject Obj;
        public Object[] auxiliarReference;

        public ObjectRefence(GameObject prefab, Object[] utilityRefence)
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


    static PoolManager instance;

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

        for (int ii = 0; ii < instance.categoriesOfPool[index].objectPool.Length; ii++)
        {
            if (instance.categoriesOfPool[index].objectPool[ii].prefab.name == powerObject)
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

    static public Transform SpawnPoolObject(Vector2Int indexs, Vector3 pos = new Vector3(), Quaternion angles = new Quaternion(), Transform padre = null)
    {

        var poolObject = InternalSpawnPoolObject(indexs);

        var transformObject = poolObject.SpawnPoolObj();

        SetTransform(transformObject, poolObject.prefab.transform, pos, angles, padre);

        return transformObject;
    }

    static public Transform SpawnPoolObject<T>(Vector2Int indexs, out T reference, Vector3 pos = new Vector3(), Quaternion angles = new Quaternion(), Transform padre = null) where T : Object
    {
        var poolObject = InternalSpawnPoolObject(indexs);

        var transform = poolObject.SpawnPoolObj(out reference);

        SetTransform(transform, poolObject.prefab.transform, pos, angles, padre);

        return transform;
    }

    static PoolObjects InternalSpawnPoolObject(Vector2Int indexs)
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

        return instance.categoriesOfPool[indexs.x].objectPool[indexs.y];
    }

    static void SetTransform(Transform transform, Transform original,Vector3 pos = new Vector3(), Quaternion angles = new Quaternion(), Transform padre = null)
    {
        transform.parent = null;
        transform.localPosition = pos;
        transform.localRotation = angles;

        transform.localScale = original.localScale;

        transform.parent = padre;

        transform.gameObject.SetActive(true);
    }
    #endregion

    void Awake()
    {
        instance = this;

        if (!eneabled)
            return;

        LoadSystem.AddPostLoadCorutine(GeneratePool);
    }

    IEnumerator GeneratePool(System.Action<bool> end, System.Action<string> msg)
    {
        msg("Starting pool");

        foreach (var item in categoriesOfPool)
        {
            foreach (var subitem in item.objectPool)
            {
                msg("Loading pool item: " + subitem.prefab.name);
                subitem.Init();
                yield return null;
            }
        }

        end(true);
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