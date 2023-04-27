using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LoadMap : SingletonMono<LoadMap>
{
    public Transform[] carlitos;

    public Camera[] cameras;

    public GameObject[] renders;

    public int rng;

    public int loadPerFrame;

    public Biomes[] biomes;

    public Tilemap map;

    public MoveAbstract playerPublic;

    public GameObject spawner;

    public GameObject victoria;

    public Vector3 basePos= new Vector3 {x=2, y=20, z=10 };

    float tiempoCarga = 0;

    Teleport[] arrHexCreados => HexagonsManager.arrHexCreados;

    Pictionarys<int, Teleport> activeHex => HexagonsManager.activeHex;

    GameObject hexagono => HexagonsManager.hexagono;

    int[][,] hexagonos => HexagonsManager.hexagonos;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        LoadSystem.AddPostLoadCorutine(CargaHexagonos);
    }

    public Vector3 CalculateHexagonoPos(int index)
    {
        return new Vector3(instance.basePos.x * (index + 1), instance.basePos.y, instance.basePos.z);
    }

    IEnumerator LoadHex(System.Action<bool> end, System.Action<string> msg2)
    {
        HexagonsManager.SetArrayHexagons(rng);

        Teleport arrHexTeleport;//Despues lo voy a usar para guardar el getcomponent

        //TextMeshProUGUI[] textMesh; //para setear los numeros de las torres

        float scala = hexagono.transform.localScale.x;

        float lado = scala / 2; //+ correccionScala;

        float apotema = Mathf.Sqrt(Mathf.Pow(lado, 2) - Mathf.Pow(lado / 2, 2));

        basePos.x = lado * basePos.x * basePos.x;

        Tile[][] terreno = new Tile[biomes.Length][];

        for (int i = 0; i < terreno.GetLength(0); i++)
        {
            terreno[i] = biomes[i].tile;
        }

        //reveer
        Object[][] props = new Object[terreno.GetLength(0)][];

        DebugPrint.Log("Informacion de seteo");

        DebugPrint.Log("lado" + lado);

        DebugPrint.Log("apotema " + apotema);

        for (int i = 0; i < terreno.GetLength(0); i++)
        {
            string path = "Props/" + biomes[i].nameDisplay + "/";

            props[i] = LoadSystem.LoadAsset(path);
        }

        //StartCoroutine(VincularHexagonos());

        msg2("Ejecutando algoritmo de vinculacion de hexagonos");

        yield return new WaitForCorutines(this, HexagonsManager.instance.VincularHexagonos, (s)=> msg2(s));

        HexagonsManager.LocalSidePosHex(apotema);

        for (int i = 0; i < hexagonos.GetLength(0); i++)
        {
            //espera para la carga
            if (i % loadPerFrame == 0)
            {
                tiempoCarga += Time.unscaledDeltaTime;

                float persentage = ((i + 1f) / hexagonos.GetLength(0) )*100;

                string msg = Mathf.RoundToInt(persentage) + "%" +
                    "\n<size=20>" +
                    i + " de " + hexagonos.GetLength(0) + "\n" +
                    "<size=25>Escape para cancelar";

                msg2(msg);

                DebugPrint.Log("Carga del nivel: " + persentage + "%");
                DebugPrint.Log("Tiempo entre frames: " + (Time.unscaledDeltaTime) + " milisegundos");
                DebugPrint.Log("Tiempo total de carga: " + tiempoCarga);
                yield return null;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopAllCoroutines();
                LoadSystem.instance.Load("Menu");

            }

            //spawn hexagonos

            arrHexCreados[i] = Instantiate(hexagono).GetComponent<Teleport>();

            arrHexCreados[i].gameObject.SetActive(false);//desactivo todo el resto de hexagonos, para que no consuman cpu

            //textMesh = arrHexCreados[i].GetComponentsInChildren<TextMeshProUGUI>();

            arrHexTeleport = arrHexCreados[i];

            arrHexCreados[i].transform.position = CalculateHexagonoPos(i);

            //arrHexCreados[i].GetComponent<Renderer>().material.SetColor("_Color", new Color(Random.Range(1, 11)/10f, Random.Range(1, 11) / 10f, Random.Range(1, 11) / 10f, 1f));

            arrHexTeleport.id = i;

            arrHexTeleport.name = "Hexagono " + i;

            arrHexTeleport.ladosArray = new int[6, 2];//Lo uso para definir A DONDE me voy a teletransportar
            arrHexTeleport.ladosPuntos = new float[6, 2];//Lo uso para guardar la coordenadas de cada lado

            rng = Random.Range(0, terreno.GetLength(0));

            FillTilePos(terreno[rng], arrHexCreados[i].transform.position, 42);

            FillPropsPos(props[rng], arrHexCreados[i].gameObject, 42, apotema, i != 0, i == 0 || i == hexagonos.GetLength(0) - 1);

            arrHexTeleport.SetTeleportEdge(hexagonos[i]);
        }
        end(true);
    }

    void FillTilePos(Tile[] tiles, Vector3 center, int distance)
    {
        int x = Vector3Int.RoundToInt(center).x - (distance / 2);
        int y = Vector3Int.RoundToInt(center).y - (distance / 2);

        int xFin = x + distance;
        int yFin = y + distance;

        for (int i = x; i < xFin; i++)
        {
            for (int ii = y; ii < yFin; ii++)
            {
                map.SetTile(new Vector3Int(i, ii, 20), tiles[Random.Range(0, tiles.Length)]);
            }
        }
    }
    IEnumerator CargaHexagonos(System.Action<bool> end, System.Action<string> msg)
    {   //el primero es la cantidad y el indice de hexagonos, le sumo uno de mas para tener el primero central, y desp el anterior vinculado
        //el segundo los lados, coloco uno mas, ya que voy a usar el lado 0, para preguntar si esta completo
        //el tercero es que id de hexagono [0] esta relacionado y el segundo [1] es a cual lado esta relacionado

        //cantidad = GameObject.FindGameObjectsWithTag("enemigo").Length;

        Teleport arrHexTeleport;

        Time.timeScale = 0;

        rng = (rng > 0) ? rng : PlayerPrefs.GetInt("Hex", 0);

        rng = (rng > 0) ? rng : Random.Range(3, 21);

        DebugPrint.Log("Cantidad de memoria: " + SystemInfo.systemMemorySize);

        while (SystemInfo.systemMemorySize < 14 * rng * 4 / 1000)
        {
            msg("Has colocado mas hexagonos que lo que dispones de RAM" +
                "\n" +
                "Pulsa escape para volver");

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopAllCoroutines();
                LoadSystem.instance.Load("Menu");

            }
            yield return null;
        }

        if(playerPublic!= null)
        {
            GameManager.instance.player = playerPublic.gameObject;
            carlitos = playerPublic.carlitos;
        }
            

        cameras = new Camera[Camera.allCamerasCount - 1];

        renders = new GameObject[6];


        //recorro todas las camaras y guardo las que quiero en orden
        foreach (var item in Camera.allCameras)
        {
            //print(item.name);
            if (item.name != "Main Camera")
            {
                cameras[int.Parse(item.name.Substring(item.name.IndexOf("(") + 1, 1))] = item;
            }

        }
        //print("La cantidad de camaras es de: " + cameras.Length);

        //audioListener.enabled = true;

        
        yield return new WaitForCorutines(this, LoadHex, (s) => msg(s));

        //prototipo de us

        //audioListener.enabled = false;

        //configuro las camaras y los carlitos para el primer hexagono

        arrHexTeleport = arrHexCreados[0];

        arrHexCreados[0].gameObject.SetActive(true);

        activeHex.Add(0, arrHexCreados[0]);

        for (int i = 0; i < cameras.Length; i++)
        {
            arrHexCreados[arrHexTeleport.ladosArray[i, 0]].gameObject.SetActive(true);

            activeHex.Add(arrHexCreados[arrHexTeleport.ladosArray[i, 0]].id, arrHexCreados[arrHexTeleport.ladosArray[i, 0]]);

            cameras[i].gameObject.transform.position = new Vector3(
                arrHexCreados[arrHexTeleport.ladosArray[i, 0]].transform.position.x,
                arrHexCreados[arrHexTeleport.ladosArray[i, 0]].transform.position.y,
                cameras[i].gameObject.transform.position.z
            );

            //cameras[i].orthographicSize = correcionZoom;

            renders[i] = GameObject.Find("render (" + i + ")");

            renders[i].transform.position = HexagonsManager.AbsSidePosHex(arrHexCreados[0].transform.position, i, renders[i].transform.position.z, 2);

            //renders[i].transform.localScale = Vector3.one * scala;

            if(playerPublic != null)
                carlitos[i].transform.position = HexagonsManager.AbsSidePosHex(arrHexCreados[arrHexTeleport.ladosArray[i, 0]].transform.position, ((i - 3) >= 0) ? (i - 3) : (i + 3), carlitos[i].transform.position.z, 2);
        }

        if (arrHexCreados.GetLength(0) > 1)
        {
            victoria = Instantiate(victoria);
            victoria.transform.position = arrHexCreados[arrHexCreados.GetLength(0) - 1].transform.position;
        }        

        tiempoCarga += Time.unscaledDeltaTime;
        DebugPrint.Log("Tiempo entre frames: " + (Time.unscaledDeltaTime) + " milisegundos");
        DebugPrint.Log("Tiempo total de carga: " + tiempoCarga);
        DebugPrint.Log("Carga finalizada");

        if(playerPublic != null)
        {
            playerPublic.transform.parent = arrHexCreados[0].transform;
            playerPublic.transform.localPosition = new Vector3(0, 0, 0);
        }
        

        //AudioManager.instance.Play("ambiente").source.loop = true;
        end(true);
    }


    void FillPropsPos(Object[] props, GameObject hex, int distance, float apotema, bool spawn, bool centro = false)
    {
        Vector3 center = hex.transform.position;
        int x = Vector3Int.RoundToInt(center).x - (distance / 2);
        int y = Vector3Int.RoundToInt(center).y - (distance / 2);

        int xFin = x + distance;
        int yFin = y + distance;

        for (int i = x; i < xFin; i += 4)
        {
            for (int ii = y; ii < yFin; ii += 4)
            {
                if ((new Vector3(i, ii, center.z) - hex.transform.position).sqrMagnitude < Mathf.Pow(apotema * 0.85f, 2) && ((new Vector3(i, ii, center.z) - hex.transform.position).sqrMagnitude > 9 || !centro))
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        float rng1 = Random.Range(1, 31);
                        float rng2 = Random.Range(1, 31);
                        GameObject prop = Object.Instantiate(props[Random.Range(0, props.Length)]) as GameObject;

                        prop.transform.position = new Vector3((i - 1.5f) + rng1 / 10f, (ii - 1.5f) + rng2 / 10f, center.z);

                        var aux = prop.GetComponentInChildren<SpriteRenderer>();

                        aux.sortingOrder = Mathf.RoundToInt(prop.transform.position.y * -100);

                        aux.flipX = (Random.Range(0, 2) == 0);

                        //prop.transform.localScale = new Vector3(prop.transform.localScale.x * (Random.Range(0, 2) == 0 ? -1 : 1), prop.transform.localScale.y, prop.transform.localScale.z);



                        prop.transform.SetParent(hex.transform);
                    }
                    else if (Random.Range(0, 3) == 0 && spawn)
                    {
                        //SpawnEnemy(hex.transform, new Vector3(i, ii, center.z));
                        Instantiate(spawner, new Vector3(i, ii, center.z), Quaternion.identity).transform.SetParent(hex.transform);


                    }
                }


            }
        }
    }

   

    

   
}

