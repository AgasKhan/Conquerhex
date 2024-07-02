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
        public Transform parent;

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
                pool[i] = new ObjectRefence(prefab, parent, utilityRefence);
            }
        }
    }

    [System.Serializable]
    public class ObjectRefence
    {
        public GameObject Obj;
        public Object[] auxiliarReference;

        public ObjectRefence(GameObject prefab, Transform parent, Object[] utilityRefence)
        {
            Obj = Instantiate(prefab, parent);

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

    static public int categoriesCount => instance.categoriesOfPool.Length;

    static public int[] poolObjectsCount;

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


    /// <summary>
    /// devuelve el indice de la categoria dentro del pool
    /// </summary>
    /// <param name="index">indice de la categoria</param>
    /// <param name="powerObject">nombre del prefab del objeto</param>
    /// <returns></returns>
    static public void SrchInCategory(Vector2Int indexsFind, out string category, out GameObject powerObject)
    {
        category = instance.categoriesOfPool[indexsFind.x].name;

        powerObject = instance.categoriesOfPool[indexsFind.x].objectPool[indexsFind.y].prefab;
    }

    #endregion

    #region "Spawn" pool objects

    static public Transform SpawnPoolObject(Vector2Int indexs, Vector3? pos = null, Quaternion? angles = null, Transform padre = null)
    {

        var poolObject = InternalSpawnPoolObject(indexs);

        var transformObject = poolObject.SpawnPoolObj();

        SetTransform(transformObject, poolObject.prefab.transform, pos, angles, padre);

        return transformObject;
    }

    static public Transform SpawnPoolObject<T>(Vector2Int indexs, out T reference, Vector3? pos = null, Quaternion? angles = null, Transform padre = null, bool active=true) where T : Object
    {
        var poolObject = InternalSpawnPoolObject(indexs);

        var transformObject = poolObject.SpawnPoolObj(out reference);

        SetTransform(transformObject, poolObject.prefab.transform, pos, angles, padre, active);

        return transformObject;
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

    static void SetTransform(Transform transform, Transform original, Vector3? pos = null, Quaternion? angles = null, Transform padre = null, bool active = true)
    {
        transform.SetActiveGameObject(false);

        if (padre != null)
        {
            transform.SetParent(null, true);

            transform.localScale = original.localScale;
        }
        else
        {
            var aux = transform.parent;

            transform.SetParent(null, true);

            transform.localScale = original.localScale;

            transform.SetParent(aux, true);
        }

        if(padre != null)
        {
            transform.SetParent(padre, true);
        }

        if (pos != null)
            transform.localPosition = (Vector3)pos;

        if (angles != null)
            transform.localRotation = (Quaternion)angles;

        transform.gameObject.SetActive(active);
    }
    #endregion

    private void OnValidate()
    {
        instance = this;
        poolObjectsCount = new int[categoriesCount];

        for (int i = 0; i < categoriesCount; i++)
        {
            poolObjectsCount[i] = categoriesOfPool[i].objectPool.Length;
        }
    }

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