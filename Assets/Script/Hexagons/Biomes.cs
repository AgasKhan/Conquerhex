using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[CreateAssetMenu(menuName = "BaseData/Biomes", fileName = "Biomes")]
public class Biomes : ShowDetails
{
    [System.Serializable]
    public struct LayersOfProps
    {
        [System.Serializable]
        public struct SpawnPerLevel
        {
            //public int level;
            public Pictionarys<Spawner, int> spawners;

            public Pictionarys<GameObject, int> center;
        }

        public int chanceEmptyOrEnemy;
        [Tooltip("Representa cuantas casillas se salta para colocar objetos, en caso de ser 0 no se saltara ninguna")]
        public int inversaDensidad;

        public SpawnPerLevel[] spawners;

        public Spawner spawner;
        public Pictionarys<GameObject, int> props;
    }

    public enum LayersNames
    {
        Trees,
        Enemies,
        Paths
    }

    public TerrainData terrainBiome;
    public Tile[] tile;
    public Color generalColor = new Color(1,1,1,1);

    [Header("Orden de las layers:\n\tPrimero: Trees\n\tSegundo: Enemies\n\tTercero: Paths\n\nPara el sistema viejo toma: \n\tchanceEmptyOrEnemy: Enemies\n\tinversaDensidad: trees\n\tspawner: Enemies")]
    [Space]
    public LayersOfProps[] layersOfProps = new LayersOfProps[3];

    [Tooltip("Quedara obsoleto cuando se traspase de sistema, en su lugar trabajar con LayersOfProps")]
    public Pictionarys<GameObject, int> props
    {
        get
        {
            if(_props==null)
                _props = layersOfProps.SelectMany((lOfProps) => lOfProps.props).ToPictionarys();
            return layersOfProps.SelectMany((lOfProps) => lOfProps.props).ToPictionarys();
        }
    }

    Pictionarys<GameObject, int> _props;

    [ContextMenu("Reset colors")]
    void ResetColor()
    {
        foreach (var item in tile)
        {
            item.color = Color.white;
        }
    }

    [ContextMenu("Cargar assets de la carpeta")]
    void LoadAssets()
    {
        string path = BaseData.pathProps + "Common" + "/";

        for (int i = 0; i < layersOfProps.Length; i++)
        {
            string pathFolder = path + System.Enum.GetNames(typeof(LayersNames))[i] + "/";

            Debug.Log(pathFolder);

            foreach (var item in LoadSystem.LoadAssets<GameObject>(pathFolder))
            {
                if (!layersOfProps[i].props.ContainsKey(item))
                    layersOfProps[i].props.Add(item, 10);
            }
        }

        path = BaseData.pathProps + nameDisplay + "/";

        for (int i = 0; i < layersOfProps.Length; i++)
        {
            string pathFolder = path + System.Enum.GetNames(typeof(LayersNames))[i] + "/";

            Debug.Log(pathFolder);

            foreach (var item in LoadSystem.LoadAssets<GameObject>(pathFolder))
            {
                if (!layersOfProps[i].props.ContainsKey(item))
                    layersOfProps[i].props.Add(item, 10);
            }
        }
    }

    private void OnValidate()
    {
        if (generalColor == Color.white)
            return;

        foreach (var item in tile)
        {
            item.color = generalColor;
        }
    }
}

[System.Serializable]
public class GameObjectSpawnProperty : ISerializationCallbackReceiver
{
    public GameObject prefabToSpawn;

    public bool spawnInLocal;

    [Space, Header("Offset position")]
    [Range(0, 10)]
    public float offsetPositionX;
    [Range(0, 10)]
    public float offsetPositionY;
    [Range(0, 10)]
    public float offsetPositionZ;

    [Space, Header("Offset rotation")]
    [Range(-180, 180)]
    public float offsetRotationX;
    [Range(-180, 180)]
    public float offsetRotationY;
    [Range(-180, 180)]
    public float offsetRotationZ;


    [Space, Header("Random position")]
    [Range(0, 10)]
    public float randomPositionX;
    [Range(0, 10)]
    public float randomPositionY;
    [Range(0, 10)]
    public float randomPositionZ;

    [Space, Header("Random rotation")]
    [Range(0, 360)]
    public float randomRotationX;
    [Range(0, 360)]
    public float randomRotationY;
    [Range(0, 360)]
    public float randomRotationZ;

    [Space]
    public float radius;

    [HideInInspector, SerializeField]
    protected Vector3 offsetPosition;

    [HideInInspector, SerializeField]
    protected Quaternion offsetRotation;

    public virtual Transform Spawn(Vector3? position = null, Quaternion? rotation = null, Transform parent = null)
    {
        if (rotation == null)
        {
            rotation = offsetRotation;
        }
        else
        {
            rotation *= offsetRotation;
        }

        rotation *= Quaternion.Euler(Random.Range(-randomRotationX / 2, randomRotationX / 2), Random.Range(-randomRotationY / 2, randomRotationY / 2), Random.Range(-randomRotationZ / 2, randomRotationZ / 2));

        if (position == null)
        {
            position = offsetPosition;
        }
        else
        {
            position += offsetPosition;
        }

        position += new Vector3(Random.Range(-randomPositionX / 2, randomPositionX / 2), Random.Range(-randomPositionY / 2, randomPositionY / 2), Random.Range(-randomPositionZ / 2, randomPositionZ / 2));

        return Object.Instantiate(prefabToSpawn, ((Vector3)position), (Quaternion)rotation, parent).transform;
    }

    public virtual Transform Spawn<T>(out T reference, Vector3? position = null, Quaternion? rotation = null, Transform parent = null) where T : Object
    {
        var aux = Spawn(position, rotation, parent);

        reference = aux.gameObject.GetComponent<T>();

        return aux;
    }

    public virtual void OnAfterDeserialize()
    {
    }

    public virtual void OnBeforeSerialize()
    {
        offsetPosition = new Vector3 { x = offsetPositionX, y = offsetPositionY, z = offsetPositionZ };

        offsetRotation = Quaternion.Euler(offsetRotationX, offsetRotationY, offsetRotationZ);
    }
}


[System.Serializable]
//[UnityEditor.CustomPropertyDrawer(typeof(PoolGameObjectSpawnProperty))]
public class PoolGameObjectSpawnProperty : GameObjectSpawnProperty
{
    [SerializeField, Tooltip("si posee valores negativos, sera ignorado")]
    public Vector2Int index;

    public string category;

    public override void OnBeforeSerialize()
    {
        if (!PoolManager.instanced)
            return;

        if (index.x>=0 && index.y>=0)
        {
            if (index.x >= PoolManager.categoriesCount)
                index.x = PoolManager.categoriesCount - 1;

            if(index.y>= PoolManager.poolObjectsCount[index.x])
            {
                index.y = PoolManager.poolObjectsCount[index.x] - 1;
            }

            PoolManager.SrchInCategory(index, out category, out prefabToSpawn);
        }       
        else
        {
            category = "NoSelection";
            prefabToSpawn = null;
        }

        base.OnBeforeSerialize();
    }

    public override Transform Spawn(Vector3? position = null, Quaternion? rotation = null, Transform parent=null)
    {
        if (rotation == null)
        {
            rotation = offsetRotation;
        }
        else
        {
            rotation *= offsetRotation;
        }

        rotation *= Quaternion.Euler(Random.Range(-randomRotationX / 2, randomRotationX / 2), Random.Range(-randomRotationY / 2, randomRotationY / 2), Random.Range(-randomRotationZ / 2, randomRotationZ / 2));

        if (position == null)
        {
            position = offsetPosition;
        }
        else
        {
            position += offsetPosition;
        }

        position += new Vector3(Random.Range(-randomPositionX / 2, randomPositionX / 2), Random.Range(-randomPositionY / 2, randomPositionY / 2), Random.Range(-randomPositionZ / 2, randomPositionZ / 2));

        return PoolManager.SpawnPoolObject(index, position, rotation, spawnInLocal ? parent : null);
    }

    public override Transform Spawn<T>(out T reference, Vector3? position = null, Quaternion? rotation = null, Transform parent = null)
    {
        if(rotation == null)
        {
            rotation = offsetRotation;
        }
        else
        {
            rotation *= offsetRotation;
        }

        rotation *= Quaternion.Euler(Random.Range(-randomRotationX / 2, randomRotationX / 2), Random.Range(-randomRotationY / 2, randomRotationY / 2), Random.Range(-randomRotationZ / 2, randomRotationZ / 2));

        if(position==null)
        {
            position = offsetPosition;
        }
        else
        {
            position += offsetPosition;
        }

        position += new Vector3(Random.Range(-randomPositionX / 2, randomPositionX / 2), Random.Range(-randomPositionY / 2, randomPositionY / 2), Random.Range(-randomPositionZ / 2, randomPositionZ / 2));

        return PoolManager.SpawnPoolObject(index, out reference , position, rotation, spawnInLocal ? parent : null);
    }
}