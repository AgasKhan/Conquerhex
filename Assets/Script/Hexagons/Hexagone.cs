using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Hexagone : MonoBehaviour
{
    public int id;

    public int level;

    public Hexagone[] ladosArray = new Hexagone[6];//Lo uso para definir A DONDE me voy a teletransportar

    //pareja de coordenadas
    public float[,] ladosPuntos = new float[6, 2];//Lo uso para guardar la coordenadas de cada lado

    public float velocityTransfer;

    public Biomes biomes;

    public Tilemap map;

    public bool manualTiles=false;

    public bool manualProps=false;

    public bool manualSetEdge = false;

    public int lenght = 42;

    public System.Action<int>[] OnSectionView = new System.Action<int>[6];

    [SerializeField]
    [Tooltip("en caso de tener en true el manual Props, evaluara esta condicion para spawnear entidades")]
    bool manualSpawnSpawner = false;

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

    public Hexagone FillTilePos()
    {
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
        this.ladosPuntos[i, 0] = transform.position.x + HexagonsManager.auxCalc[i, 0];
        this.ladosPuntos[i, 1] = transform.position.y + HexagonsManager.auxCalc[i, 1];
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
                if ((new Vector3(i, ii, center.z) - transform.position).sqrMagnitude < Mathf.Pow(HexagonsManager.apotema * 0.85f, 2) && ((new Vector3(i, ii, center.z) - transform.position).sqrMagnitude > Mathf.Pow(biomes.inversaDensidad,2) || !centro))
                {
                    if (Random.Range(0, 100) > biomes.chanceEmptyOrEnemy)
                    {
                        float rng1 = Random.Range(1, biomes.inversaDensidad * 10 + 1);
                        float rng2 = Random.Range(1, biomes.inversaDensidad * 10 + 1);

                        GameObject prop = Instantiate(biomes.props.RandomPic(level), new Vector3((i - biomes.inversaDensidad/2f) + rng1 / 10f, (ii - biomes.inversaDensidad / 2f) + rng2 / 10f, center.z), Quaternion.identity);

                        var aux = prop.GetComponentInChildren<SpriteRenderer>();

                        aux.flipX = (Random.Range(0, 2) == 0);

                        //prop.transform.localScale = new Vector3(prop.transform.localScale.x * (Random.Range(0, 2) == 0 ? -1 : 1), prop.transform.localScale.y, prop.transform.localScale.z);

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

    /// <summary>
    /// setea los renders en base a este hexagono
    /// </summary>
    /// <param name="lado"></param>
    public void SetRenders(int lado = -1)
    {
        var activeHex = HexagonsManager.activeHex;

        if (lado >= 0)
        {
            MainCamera.instance.transform.position = new Vector3(
                 ladosPuntos[HexagonsManager.LadoOpuesto(lado), 0] - (ladosPuntos[lado, 0] - Camera.main.transform.position.x),
                 ladosPuntos[HexagonsManager.LadoOpuesto(lado), 1] - (ladosPuntos[lado, 1] - Camera.main.transform.position.y),
                 MainCamera.instance.transform.position.z);

            LoadMap.instance.cameras[HexagonsManager.LadoOpuesto(lado)].gameObject.SetActive(false);

            LoadMap.instance.cameras[lado].gameObject.SetActive(true);
        }
            
        for (int i = 0; i < LoadMap.instance.renders.Length; i++)
        {
            ladosArray[i].gameObject.SetActive(true);

            activeHex.Add(ladosArray[i].id, ladosArray[i]);

            LoadMap.instance.renders[i].SetRender(ladosArray[i], i);

            //LoadMap.instance.renders[i].transform.position = HexagonsManager.AbsSidePosHex(transform.position, i, LoadMap.instance.renders[i].transform.position.z, 2);

            /*
            LoadMap.instance.cameras[i].position = new Vector3(
                ladosArray[i].transform.position.x,
                ladosArray[i].transform.position.y,
                LoadMap.instance.cameras[i].position.z
                );
            */
        }


        for (int i = activeHex.Count - 1; i >= 0; i--)
        {
            bool off = true;

            for (int l = 0; l < 6; l++)
            {
                if (id == activeHex[i].id || ladosArray[l].id == HexagonsManager.activeHex[i].id)
                {
                    off = false;
                    break;
                }
            }

            if (off)
            {
                activeHex[i].gameObject.SetActive(false);//desactivo todo el resto de hexagonos, para que no consuman cpu
                activeHex.RemoveAt(i);
            }
        }
    }

    public Hexagone SetProyections(Transform original, Component[] components, bool setParent = false)
    {
        for (int i = 0; i < components.Length && i< ladosArray.Length; i++)
        {
            components[i].transform.position = HexagonsManager.AbsSidePosHex(ladosArray[i].transform.position, HexagonsManager.LadoOpuesto(i), components[i].transform.position.z, 2) + (original.position - transform.position);

            if (setParent)
                components[i].transform.SetParent(ladosArray[i].transform, true);
        }

        return this;
    }

    public Hexagone SuscribeOnSection(int lado,System.Action<int> callback)
    {
        OnSectionView[lado] += callback;

        return this;
    }


    private void Start()
    {
        if (!manualTiles)
            FillTilePos();

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
