using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.Events;

public class Hexagone : MonoBehaviour, IUpdate
{
    public struct WeightTransform: System.IComparable<WeightTransform>
    {
        public Transform transform;

        public int CompareTo(WeightTransform other)
        {
            return transform.childCount.CompareTo(other.transform.childCount);
        }
    }

    [Header("bools para mentir")]

    public bool tileGeneration = true;

    public bool gridGeneration = true;

    [Header("Informacion")]

    [SerializeField]
    public Random.State seedTerrain;

    public int id;

    public int level;

    public int ladoToBase = -1;

    public Hexagone[] ladosArray = new Hexagone[6];//Lo uso para definir A DONDE me voy a teletransportar

    //pareja de coordenadas
    public Vector3[] ladosPuntos = new Vector3[6];//Lo uso para guardar la coordenadas de cada lado

    public Vector3[] aristasPuntos = new Vector3[6];

    public Biomes biomes;

    public Tilemap map;

    [Header("Configuracion")]

    public TerrainManager mapCopado;

    public SpriteRenderer[] effect;

    public float velocityTransfer;

    public bool manualTiles = false;

    public bool manualProps = false;

    public bool manualSetEdge = false;

    public int lenght = 42;

    //[SerializeField]
    int objectsPerChunk => HexagonsManager.instance.objectsPerChunk;

    int[,] detailsMap => mapCopado.detailsMap;

    public TerrainManager.Paths Paths => mapCopado.paths[0];

    public HashSet<Entity> childsEntities { get; private set; } = new HashSet<Entity>();

    PriorityQueue<WeightTransform> chunks = new PriorityQueue<WeightTransform>();

    [SerializeField]
    [Tooltip("en caso de tener en true el manual Props, evaluara esta condicion para spawnear entidades")]
    bool manualSpawnSpawner = false;

    public bool bussy;

    public IEnumerable<Entity> AllChildEntities => childsEntities.Concat(ladosArray.SelectMany((hex)=>hex.childsEntities));

    bool on;

    public event UnityAction MyUpdates;
    public event UnityAction MyFixedUpdates;
    public void SetPortalColor(Color color)
    {
        for (int i = 0; i < effect.Length; i++)
        {
            effect[i].color = color;
        }
    }
    public void ExitEntity(Entity entity)
    {
        entity.HexagoneParent = null;
        /*
        if (bussy)
            GameManager.eventQueueRoutine.Enqueue(Routine(() => childsEntities.Remove(entity)));
        else*/
        childsEntities.Remove(entity);
        ExitChunk(entity.transform);
    }

    public void EnterEntity(Entity entity)
    {
        entity.HexagoneParent?.ExitEntity(entity);
        entity.HexagoneParent = this;
        //entity.transform.parent = null;
        /*
        if (bussy)
            GameManager.eventQueueRoutine.Enqueue(Routine(() => childsEntities.Add(entity, entity.gameObject.activeSelf)));
        else*/
        childsEntities.Add(entity);

        EnterChunk(entity.transform);
    }

    public void EnterChunk(Transform tr)
    {
        if(chunks.IsEmpty)
        {
            Transform newParent = new GameObject("chunk " + transform.childCount).transform;

            newParent.transform.position = transform.position;
            newParent.parent = transform;

            newParent.SetActiveGameObject(gameObject.activeSelf);

            tr.SetParent(newParent);

            chunks.Enqueue(new WeightTransform() { transform = newParent });
            return;
        }

        var parent = chunks.Dequeue();

        if (parent.transform.childCount >= objectsPerChunk)
        {
            Transform newParent = new GameObject("chunk "+ transform.childCount).transform;

            newParent.transform.position = transform.position;
            newParent.parent = transform;

            newParent.SetActiveGameObject(gameObject.activeSelf);

            tr.SetParent(newParent,true);

            chunks.Enqueue(new WeightTransform() { transform = newParent });

            chunks.Enqueue(parent);
        }
        else
        {
            tr.SetParent(parent.transform);

            chunks.Enqueue(parent);
        }
    }

    public void ExitChunk(Transform tr)
    {
        foreach (var item in chunks)
        {
            if(item.transform == tr.parent)
            {
                tr.SetParent(null, true);
                chunks.UpdateElement(item);
                break;
            }
        }
    }

    void OptimiceChunks()
    {
        List<Transform> childs = new List<Transform>();
        while(!chunks.IsEmpty)
        {
            var parent =chunks.Dequeue();
            if (parent.transform.childCount < objectsPerChunk)
            {
                for (int i = parent.transform.childCount-1; i >= 0; i--)
                {
                    childs.Add(parent.transform.GetChild(i));
                    parent.transform.GetChild(i).parent = transform;
                }

                Destroy(parent.transform.gameObject);
            }
            else
            {
                chunks.Enqueue(parent);
                break;
            }
        }

        for (int i = 0; i < childs.Count; i++)
        {
            EnterChunk(childs[i]);
        }
    }

    public void ChangeOnOffRoutine(bool b)
    {
        on = b;
    }

    public IEnumerator On()
    {
        bussy = true;
        on = true;

        gameObject.SetActive(true);

        for (int i = 0; i < transform.childCount; i++)
        {
            if (GameManager.MediumFrameRate)
                yield return null;

            if (!on)
            {
                yield return Off();
                yield break;
            }

            if (i >= transform.childCount)
                break;

            var item = transform.GetChild(i);

            item.SetActiveGameObject(true);
        }

        bussy = false;
    }

    public IEnumerator Off()
    {
        bussy = true;
        on = false;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (GameManager.MediumFrameRate)
                yield return null;

            if (on)
            {
                yield return On();
                yield break;
            }

            if (i >= transform.childCount)
                break;

            transform.GetChild(i).SetActiveGameObject(false);
        }

        gameObject.SetActive(false);

        OptimiceChunks();

        bussy = false;
    }

    /*
    IEnumerator Routine(System.Action action)
    {
        yield return null;
        action();
    }
    */

    public Hexagone SetID(int i)
    {
        id = i;

        HexagonsManager.arrHexCreados[i] = this;

        return this;
    }

    public Hexagone SetOwnerColor()
    {
        HexagonsManager.SetColorHexagone(this);

        return this;
    }

    public Hexagone SetTileMap(Tilemap map)
    {
        this.map = map;
        return this;
    }

    public Hexagone SetBiome(Biomes biomes)
    {
        this.biomes = biomes;
        return this;
    }

    public Hexagone SetTeleportEdge(int[,] ladosArray)
    {
        level = ladosArray[0, 0];

        LoadSystem.AddPostLoadCorutine(() => {

            for (int ii = 0; ii < ladosArray.GetLength(0) - 1; ii++)
            {
                this.ladosArray[ii] = HexagonsManager.arrHexCreados[ladosArray[ii + 1, 0]];
                //this.ladosArray[ii] = ladosArray[ii + 1, 1] - 1; //Le resto para tener el indice en 0

                //para X e Y
                SetEdgePoint(ii);
            }
        },0);

        return this;
    }

    public Hexagone SetTerrain()
    {
        seedTerrain = Random.state;

        mapCopado.copyData = biomes.terrainBiome;

        if (manualTiles)
        {
            Paths.toCenterPath = true;

            for (int i = 0; i < ladosArray.Length; i++)
            {
                if (HexagonsManager.fluentMap[HexagonsManager.hexagonos[id][i + 1, 0]] == id)
                {
                    Paths.points[i] = 1;
                    Paths.toCenterPath = false;
                }

                if (HexagonsManager.fluentMap[id] == HexagonsManager.hexagonos[id][i + 1, 0])
                {
                    Paths.points[i] = 1;
                    ladoToBase = i;
                }
            }

            if (id == 0)
                Paths.toCenterPath = true;
        }

        mapCopado.Generate();

        if (tileGeneration)
        {
            mapCopado.SetActiveGameObject(false);
            int x = Vector3Int.RoundToInt(transform.position).x - (lenght / 2);
            int y = Vector3Int.RoundToInt(transform.position).z - (lenght / 2);

            int xFin = x + lenght;
            int yFin = y + lenght;

            for (int i = x; i < xFin; i++)
            {
                for (int ii = y; ii < yFin; ii++)
                {
                    //var tile = biomes.tile[Random.Range(0, biomes.tile.Length)];
                    //var index = new Vector3Int(i, ii, 0);
                    map.SetTile(new Vector3Int(i, ii, 0), biomes.tile[Random.Range(0, biomes.tile.Length)]);
                    //map.SetTileFlags(index, TileFlags.None);
                    //map.SetColor(index, tile.color);
                    //map.RefreshTile(index);
                }
            }
        }

        return this;
    }

    public void SetEdgePoint(int i)
    {
        this.ladosPuntos[i] = new Vector3();

        this.ladosPuntos[i].x = transform.position.x + HexagonsManager.localApotema[i, 0];
        this.ladosPuntos[i].z = transform.position.z + HexagonsManager.localApotema[i, 1];

        this.aristasPuntos[i] = new Vector3();
        this.aristasPuntos[i].x = transform.position.x + HexagonsManager.localRadio[i, 0];
        this.aristasPuntos[i].z = transform.position.z + HexagonsManager.localRadio[i, 1];
    }
   
    public void FillPropsPos(bool spawn, bool centro = false)
    {
        LoadSystem.AddPostLoadCorutine((System.Action<bool> end, System.Action<string> msg) => 
        {
            return FillPropsPosRoutine((str)=> msg($"Load props in hexagone: {id}\n{str}"), spawn, centro);
        });
    }

    public IEnumerator FillPropsPosRoutine(System.Action<string> msg , bool spawn, bool centro = false, int spawnCountLimitPerFrame = 500)
    {
        Vector3 center = transform.position;
        int x;
        int z;

        int xFin;
        int zFin;

        int suma;

        if (gridGeneration)
        {
            x = Vector3Int.RoundToInt(center).x - (lenght / 2);
            z = Vector3Int.RoundToInt(center).z - (lenght / 2);

            xFin = x + lenght;
            zFin = z + lenght;

            suma = biomes.layersOfProps[(int)Biomes.LayersNames.Trees].inversaDensidad + 1;

            for (int i = x; i < xFin; i += suma)
            {
                for (int ii = z; ii < zFin; ii += suma)
                {
                    var newDistPos = new Vector3(i, center.y, ii) - transform.position;

                    //var newDistPos = (new Vector3(i, ii, transform.position.z) - transform.position);

                    if (newDistPos.sqrMagnitude < Mathf.Pow(HexagonsManager.apotema * 0.85f, 2) && (newDistPos.sqrMagnitude > Mathf.Pow(biomes.layersOfProps[(int)Biomes.LayersNames.Trees].inversaDensidad, 2) || !centro))
                    {
                        if (Random.Range(0, 100) > biomes.layersOfProps[(int)Biomes.LayersNames.Enemies].chanceEmptyOrEnemy)
                        {
                            if (biomes.props.Count > 0)
                            {
                                float rng1 = Random.Range(1, biomes.layersOfProps[(int)Biomes.LayersNames.Trees].inversaDensidad * 10 + 1);
                                float rng2 = Random.Range(1, biomes.layersOfProps[(int)Biomes.LayersNames.Trees].inversaDensidad * 10 + 1);

                                GameObject prop = Instantiate(biomes.props.RandomPic(), new Vector3((i - biomes.layersOfProps[(int)Biomes.LayersNames.Trees].inversaDensidad / 2f) + rng1 / 10f, center.y, (ii - biomes.layersOfProps[(int)Biomes.LayersNames.Trees].inversaDensidad / 2f) + rng2 / 10f), Quaternion.identity);

                                EnterChunk(prop.transform);

                                //prop.transform.SetParent(transform);
                            }
                        }
                        else if (spawn)
                        {
                            //SpawnEnemy(hex.transform, new Vector3(i, ii, center.z));
                            if (biomes.layersOfProps[(int)Biomes.LayersNames.Enemies].spawner != null)
                                Instantiate(biomes.layersOfProps[(int)Biomes.LayersNames.Enemies].spawner, new Vector3(i, center.y, ii), Quaternion.identity).transform.SetParent(transform);
                        }
                    }
                }
            }

            yield break;
        }

        x = 0;
        z = 0;

        xFin = detailsMap.GetLength(0);
        zFin = detailsMap.GetLength(1);

        suma = 1;

        
        GameObject[][] dataRandom = new GameObject[][]
        {
            biomes.layersOfProps[0].props.RandomPicData().ParallelRandom(xFin * zFin),
            biomes.layersOfProps[1].props.RandomPicData().ParallelRandom(xFin * zFin),
            biomes.layersOfProps[2].props.RandomPicData().ParallelRandom(xFin * zFin),
        };

        int spawnCount = 0;


        for (int i = x; i < xFin; i += suma)
        {
            for (int ii = z; ii < zFin; ii += suma)
            {
                if (detailsMap[i, ii] == 0)
                    continue;

                int index = ii + i * xFin;

                int indexLayerProp = detailsMap[i, ii] - 1;

                float posXMultiply = ((float)ii - (detailsMap.GetLength(0) / 2)) * HexagonsManager.radio / (detailsMap.GetLength(0) / 2);

                float posZMultiply = ((float)i - (detailsMap.GetLength(1) / 2)) * HexagonsManager.radio / (detailsMap.GetLength(1) / 2);

                Vector3 pos = new Vector3(posXMultiply + center.x, center.y, posZMultiply + center.z);

                if (Random.Range(0, 100) > biomes.layersOfProps[indexLayerProp].chanceEmptyOrEnemy)
                {
                    if (biomes.layersOfProps[indexLayerProp].props.Count > 0)
                    {
                        GameObject prop = Instantiate(dataRandom[indexLayerProp][index] /*biomes.layersOfProps[indexLayerProp].props.RandomPic()*/, pos, Quaternion.identity);

                        EnterChunk(prop.transform);
                        //prop.transform.SetParent(transform);

                        spawnCount++;
                    }
                }
                else if (spawn && biomes.layersOfProps[indexLayerProp].spawner != null)
                {
                    //Instantiate(biomes.layersOfProps[indexLayerProp].spawner, pos, Quaternion.identity).transform.SetParent(transform);

                    var arr = biomes.layersOfProps[indexLayerProp].spawners;

                    if (arr == null)
                        continue;

                    var aux = arr[Mathf.Clamp(level-1, 0, biomes.layersOfProps[indexLayerProp].spawners.Length-1)].spawners;

                    if (aux!=null)
                    {
                        Instantiate(aux.RandomPic(), pos, Quaternion.identity).transform.SetParent(transform);

                        spawnCount++;
                    }
                }

                if (spawnCount % spawnCountLimitPerFrame == 0)
                {
                    msg($"{index}/{xFin * zFin}");
                    yield return null;
                }
            }
        }

        if(!centro)
            EnterChunk(Instantiate(biomes.layersOfProps[0].spawners[Mathf.Clamp(level-1, 0, biomes.layersOfProps[0].spawners.Length-1)].center.RandomPic(), center, Quaternion.identity).transform);
    }

    public Hexagone SetProyections(Transform original, IEnumerable<Transform> components, bool setParent = false)
    {
        int i = 0;

        foreach (var component in components)
        {
            component.transform.position = HexagonsManager.AbsSidePosHex(ladosArray[i].transform.position, HexagonsManager.LadoOpuesto(i), component.transform.position.y, 2) + (original.position - transform.position).Vect3Copy_Y(0);

            if (setParent)
            {
                EnterChunk(component.transform);
                //component.transform.SetParent(ladosArray[i].transform, true);
            }

            i++;
        }

        return this;
    }

    public IEnumerable<Vector3> AllEquivalentPos(Vector3 pos)
    {
        yield return pos;

        for (int i = 0; i < ladosArray.Length; i++)
        {
            yield return HexagonsManager.AbsSidePosHex(ladosArray[i].transform.position, HexagonsManager.LadoOpuesto(i), pos.y, 2) + (pos - transform.position);
        }
    }

    public int AristaMasCercana(Transform obj)
    {
        float sqrDistance = float.PositiveInfinity;
        int resultado = -1;

        for (int i = 0; i < 6; i++)
        {

            Vector3 dist = (new Vector3(HexagonsManager.localRadio[i, 0], 0 ,HexagonsManager.localRadio[i, 1]) + transform.position) - obj.position;

            //Debug.DrawLine(transform.position,dist + obj.position, Color.red, 5);

            if(dist.sqrMagnitude < sqrDistance)
            {
                resultado = i;
                sqrDistance = dist.sqrMagnitude;
            }
        }

        Debug.DrawLine(transform.position, transform.position + new Vector3(HexagonsManager.localRadio[resultado, 0], 0, HexagonsManager.localRadio[resultado, 1]), Color.red, 5);

        return resultado;
    }

    public IEnumerable<Vector3> GetEquivalentPoints(int lado)
    {
        for (int i = 0; i < ladosArray[lado].aristasPuntos.Length; i++)
        {
            yield return (ladosArray[lado].aristasPuntos[i] - ladosArray[lado].transform.position) + HexagonsManager.apotema * 2 *(ladosPuntos[lado] - transform.position).normalized + transform.position;
        }
    }

    void MyUpdate()
    {
        MyUpdates?.Invoke();
    }
    void MyFixedUpdate()
    {
        MyFixedUpdates?.Invoke();
    }

    private void OnEnable()
    {
        GameManager.GamePlayUpdate += MyUpdate;
        GameManager.GamePlayFixedUpdate += MyFixedUpdate;
    }

    private void OnDisable()
    {
        GameManager.GamePlayUpdate -= MyUpdate;
        GameManager.GamePlayFixedUpdate -= MyFixedUpdate;
    }

    private void Start()
    {
        if (!manualTiles)
            SetTerrain();

        if(!manualProps)
        {
            FillPropsPos(manualSpawnSpawner, false);
        }

        if (manualSetEdge)
            for (int ii = 0; ii < ladosArray.Length; ii++)
            {
                SetEdgePoint(ii);
            }

        HexagonsManager.arrHexCreados[id] = this;
    }
}
