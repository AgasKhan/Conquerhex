using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Hexagone : MonoBehaviour
{
    public int id;

    public int level;

    //hacia donde se teletransporta, el primer indice es el lado del propio hexagono, y el segundo es el destino (0 para la id y 1 para el lado)
    //public int[,] ladosArray;

    public Hexagone[] ladosArray = new Hexagone[6];//Lo uso para definir A DONDE me voy a teletransportar

    //pareja de coordenadas
    public float[,] ladosPuntos = new float[6, 2];//Lo uso para guardar la coordenadas de cada lado

    public float velocityTransfer;

    public Vector2 anguloDefecto;

    public Biomes biomes;

    public Tilemap map;

    public bool manualTiles=false;

    public bool manualProps=false;

    public bool manualSetEdge = false;

    public int lenght = 42;

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

        for (int i = x; i < xFin; i += 4)
        {
            for (int ii = y; ii < yFin; ii += 4)
            {
                if ((new Vector3(i, ii, center.z) - transform.position).sqrMagnitude < Mathf.Pow(HexagonsManager.apotema * 0.85f, 2) && ((new Vector3(i, ii, center.z) - transform.position).sqrMagnitude > 9 || !centro))
                {
                    if (Random.Range(0, 100) > biomes.chanceEmptyOrEnemy)
                    {
                        float rng1 = Random.Range(1, 31);
                        float rng2 = Random.Range(1, 31);

                        GameObject prop = Instantiate(biomes.props.RandomPic(), new Vector3((i - 1.5f) + rng1 / 10f, (ii - 1.5f) + rng2 / 10f, center.z), Quaternion.identity);

                        var aux = prop.GetComponentInChildren<SpriteRenderer>();

                        aux.flipX = (Random.Range(0, 2) == 0);

                        //prop.transform.localScale = new Vector3(prop.transform.localScale.x * (Random.Range(0, 2) == 0 ? -1 : 1), prop.transform.localScale.y, prop.transform.localScale.z);

                        prop.transform.SetParent(transform);
                    }
                    else if (spawn)
                    {
                        //SpawnEnemy(hex.transform, new Vector3(i, ii, center.z));
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

            LoadMap.instance.renders[i].transform.position = HexagonsManager.AbsSidePosHex(transform.position, i, LoadMap.instance.renders[i].transform.position.z, 2);

            //LoadMap.instance.cameras[i].gameObject.SetActive(true);

            LoadMap.instance.cameras[i].position = new Vector3(
                ladosArray[i].transform.position.x,
                ladosArray[i].transform.position.y,
                LoadMap.instance.cameras[i].position.z
                );
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


    private void Awake()
    {
        //vector que da el angulo por defecto para calcular el hexagono (esta a 120 grados)
        anguloDefecto = new Vector2(Mathf.Cos((2f / 3f) * Mathf.PI), Mathf.Sin((2f / 3f) * Mathf.PI));
    }

    private void Start()
    {
        if (!manualTiles)
            FillTilePos();

        if (manualSetEdge)
            for (int ii = 0; ii < ladosArray.Length; ii++)
            {
                SetEdgePoint(ii);
            }

        HexagonsManager.arrHexCreados[id] = this;
    }


}
