using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class Hexagone : MonoBehaviour
{
    [Header("bools para mentir")]

    public bool tileGeneration = true;

    public bool gridGeneration = true;

    [Header("Informacion")]

    [SerializeField]
    public Random.State seedTerrain;

    public int id;

    public int level;

    public Hexagone[] ladosArray = new Hexagone[6];//Lo uso para definir A DONDE me voy a teletransportar

    //pareja de coordenadas
    public float[,] ladosPuntos = new float[6, 2];//Lo uso para guardar la coordenadas de cada lado

    public Biomes biomes;

    public Tilemap map;

    [Header("Configuracion")]

    public TerrainManager mapCopado;

    public float velocityTransfer;

    public bool manualTiles = false;

    public bool manualProps = false;

    public bool manualSetEdge = false;

    public int lenght = 42;

    int[,] detailsMap => mapCopado.detailsMap;

    public HashSet<Entity> childsEntities { get; private set; } = new HashSet<Entity>();

    public Dictionary<Transform, Entity> childsEntitiesTr { get; private set; } = new Dictionary<Transform, Entity>();

    [SerializeField]
    [Tooltip("en caso de tener en true el manual Props, evaluara esta condicion para spawnear entidades")]
    bool manualSpawnSpawner = false;

    public bool bussy;

    public IEnumerable<Entity> AllChildEntities => childsEntities.Concat(ladosArray.SelectMany((hex)=>hex.childsEntities));

    public void ExitEntity(Entity entity)
    {
        entity.hexagoneParent = null;
        /*
        if (bussy)
            GameManager.eventQueueRoutine.Enqueue(Routine(() => childsEntities.Remove(entity)));
        else*/
        childsEntities.Remove(entity);
        childsEntitiesTr.Remove(entity.transform);
    }

    public void EnterEntity(Entity entity)
    {
        entity.hexagoneParent?.ExitEntity(entity);
        entity.hexagoneParent = this;
        //entity.transform.parent = null;
        /*
        if (bussy)
            GameManager.eventQueueRoutine.Enqueue(Routine(() => childsEntities.Add(entity, entity.gameObject.activeSelf)));
        else*/
        childsEntities.Add(entity);
        childsEntitiesTr.Add(entity.transform, entity);
    }

    public IEnumerator On()
    {
        bussy = true;

        gameObject.SetActive(true);

        for (int i = 0; i < transform.childCount; i++)
        {
            var item = transform.GetChild(i);

            if (!childsEntitiesTr.TryGetValue(item, out var e))
                item.SetActiveGameObject(true);
            else if (!e.health.IsDeath)
                item.SetActiveGameObject(true);

            if ((i % (transform.childCount /100)) == 0 && GameManager.MediumFrameRate)
                yield return null;
        }

        bussy = false;
    }

    public IEnumerator Off()
    {
        bussy = true;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).SetActiveGameObject(false);

            if ((i % (transform.childCount/100)) == 0 && GameManager.MediumFrameRate)
                yield return null;
        }

        gameObject.SetActive(false);

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
                    map.SetTile(new Vector3Int(i, ii, 0), biomes.tile[Random.Range(0, biomes.tile.Length)]);
                }
            }
        }

        return this;
    }

    public void SetEdgePoint(int i)
    {
        this.ladosPuntos[i, 0] = transform.position.x + HexagonsManager.localApotema[i, 0];
        this.ladosPuntos[i, 1] = transform.position.z + HexagonsManager.localApotema[i, 1];
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

                                prop.transform.SetParent(transform);
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

                        prop.transform.SetParent(transform);

                        spawnCount++;
                    }
                }
                else if (spawn && biomes.layersOfProps[indexLayerProp].spawner != null)
                {
                    Instantiate(biomes.layersOfProps[indexLayerProp].spawner, pos, Quaternion.identity).transform.SetParent(transform);
                    spawnCount++;
                }

                if (spawnCount % spawnCountLimitPerFrame == 0)
                {
                    msg($"{index}/{xFin * zFin}");
                    yield return null;
                }
            }
        }

    }

    public Hexagone SetProyections(Transform original, IEnumerable<Transform> components, bool setParent = false)
    {
        int i = 0;

        foreach (var component in components)
        {
            component.transform.position = HexagonsManager.AbsSidePosHex(ladosArray[i].transform.position, HexagonsManager.LadoOpuesto(i), component.transform.position.y, 2) + (original.position - transform.position).Vect3Copy_Y(0);

            if (setParent)
                component.transform.SetParent(ladosArray[i].transform, true);

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
