using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class Hexagone : MonoBehaviour
{
    [SerializeField]
    public Random.State seedTerrain;

    public int id;

    public int level;

    public Hexagone[] ladosArray = new Hexagone[6];//Lo uso para definir A DONDE me voy a teletransportar

    //pareja de coordenadas
    public float[,] ladosPuntos = new float[6, 2];//Lo uso para guardar la coordenadas de cada lado

    public float velocityTransfer;

    public Biomes biomes;

    public Tilemap map;

    public bool manualTiles = false;

    public bool manualProps = false;

    public bool manualSetEdge = false;

    public int lenght = 42;

    public HashSet<Entity> childsEntities { get; private set; } = new HashSet<Entity>();

    [SerializeField]
    [Tooltip("en caso de tener en true el manual Props, evaluara esta condicion para spawnear entidades")]
    bool manualSpawnSpawner = false;

    public IEnumerable<Entity> AllChildEntities => childsEntities.Concat(ladosArray.SelectMany((hex)=>hex.childsEntities));

    public void ExitEntity(Entity entity)
    {
        childsEntities.Remove(entity);
    }

    public void EnterEntity(Entity entity)
    {
        childsEntities.Add(entity);
    }

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

        int x = Vector3Int.RoundToInt(transform.position).x - (lenght / 2);
        int y = Vector3Int.RoundToInt(transform.position).y - (lenght / 2);

        int xFin = x + lenght;
        int yFin = y + lenght;

        for (int i = x; i < xFin; i++)
        {
            for (int ii = y; ii < yFin; ii++)
            {
                map.SetTile(new Vector3Int(i, ii, 0), biomes.tile[Random.Range(0, biomes.tile.Length)]);
            }
        }

        return this;
    }

    public void SetEdgePoint(int i)
    {
        this.ladosPuntos[i, 0] = transform.position.x + HexagonsManager.localApotema[i, 0];
        this.ladosPuntos[i, 1] = transform.position.y + HexagonsManager.localApotema[i, 1];
    }
   
    public void FillPropsPos(bool spawn, bool centro = false)
    {
        Vector3 center = transform.position;
        int x = Vector3Int.RoundToInt(center).x - (lenght / 2);
        int y = Vector3Int.RoundToInt(center).y - (lenght / 2);

        int xFin = x + lenght;
        int yFin = y + lenght;

        for (int i = x; i < xFin; i += biomes.inversaDensidad+1)
        {

            for (int ii = y; ii < yFin; ii += biomes.inversaDensidad+1)
            {
                
                var newDistPos=new Vector3(i, ii, center.z) - transform.position;

                //var newDistPos = (new Vector3(i, ii, transform.position.z) - transform.position);

                if (newDistPos.sqrMagnitude < Mathf.Pow(HexagonsManager.apotema * 0.85f, 2) && (newDistPos.sqrMagnitude > Mathf.Pow(biomes.inversaDensidad,2) || !centro))
                {
                    if (Random.Range(0, 100) > biomes.chanceEmptyOrEnemy)
                    {
                        float rng1 = Random.Range(1, biomes.inversaDensidad * 10 + 1);
                        float rng2 = Random.Range(1, biomes.inversaDensidad * 10 + 1);

                        GameObject prop = Instantiate(biomes.props.RandomPic(level), new Vector3((i - biomes.inversaDensidad/2f) + rng1 / 10f, (ii - biomes.inversaDensidad / 2f) + rng2 / 10f, center.z), Quaternion.identity);

                        prop.transform.SetParent(transform);
                    }
                    else if (spawn)
                    {
                        //SpawnEnemy(hex.transform, new Vector3(i, ii, center.z));
                        if(biomes.spawner!=null)
                            Instantiate(biomes.spawner, new Vector3(i, ii, center.z), Quaternion.identity).transform.SetParent(transform);
                    }
                }
            }
        }
    }

    public Hexagone SetProyections(Transform original, IEnumerable<Transform> components, bool setParent = false)
    {
        int i = 0;

        foreach (var component in components)
        {
            component.transform.position = HexagonsManager.AbsSidePosHex(ladosArray[i].transform.position, HexagonsManager.LadoOpuesto(i), component.transform.position.z, 2) + (original.position - transform.position).Vect3Copy_Z(0);

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
            yield return HexagonsManager.AbsSidePosHex(ladosArray[i].transform.position, HexagonsManager.LadoOpuesto(i), pos.z, 2) + (pos - transform.position);
        }
    }

    public int AristaMasCercana(Transform obj)
    {
        float sqrDistance = float.PositiveInfinity;
        int resultado = -1;

        for (int i = 0; i < 6; i++)
        {

            Vector3 dist = (new Vector3(HexagonsManager.localRadio[i, 0], HexagonsManager.localRadio[i, 1]) + transform.position) - obj.position;

            //Debug.DrawLine(transform.position,dist + obj.position, Color.red, 5);

            if(dist.sqrMagnitude < sqrDistance)
            {
                resultado = i;
                sqrDistance = dist.sqrMagnitude;
            }
        }

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
