using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LoadMap : SingletonMono<LoadMap>
{
    [System.Serializable]
    public class MapTransform
    {
        public RenderTexture[] renders;

        public Transform this [int index]
        {
            get
            {
                return renders[index].cameraRelated.transform;
            }
        }
    }

    public bool execute = true;

    public Transform[] carlitos;


    public MapTransform cameras;
   
    public RenderTexture[] renders
    {
        get => cameras.renders;
    }

    public int rng;

    public int loadPerFrame;

    public Biomes[] biomes;

    public Tilemap map;

    public MoveAbstract playerPublic;

    public GameObject victoria;

    public Vector3 basePos= new Vector3 {x=2, y=20, z=10 };

    float tiempoCarga = 0;

    Hexagone[] arrHexCreados => HexagonsManager.arrHexCreados;

    Pictionarys<int, Hexagone> activeHex => HexagonsManager.activeHex;

    GameObject hexagono => HexagonsManager.hexagono;

    int[][,] hexagonos => HexagonsManager.hexagonos;

    // Start is called before the first frame update

    

    protected override void Awake()
    {
        base.Awake();
        if(execute)
            LoadSystem.AddPostLoadCorutine(CargaHexagonos);
    }

    public Vector3 CalculateHexagonoPos(int index)
    {
        return new Vector3(instance.basePos.x * (index + 1), instance.basePos.y, instance.basePos.z);
    }

    IEnumerator LoadHex(System.Action<bool> end, System.Action<string> msg2)
    {
        HexagonsManager.SetArrayHexagons(rng);

        Hexagone arrHexTeleport;//Despues lo voy a usar para guardar el getcomponent

        //TextMeshProUGUI[] textMesh; //para setear los numeros de las torres

        basePos.x = HexagonsManager.lado * basePos.x * basePos.x;

        for (int i = 0; i < biomes.Length; i++)
        {
            string path = "Props/" + biomes[i].nameDisplay + "/";

            foreach (var item in LoadSystem.LoadAssets<GameObject>(path))
            {
                if (!biomes[i].props.ContainsKey(item))
                    biomes[i].props.Add(item,10);
            }
        }

        //StartCoroutine(VincularHexagonos());

        msg2("Ejecutando algoritmo de vinculacion de hexagonos");

        yield return new WaitForCorutines(this, HexagonsManager.instance.VincularHexagonos, (s)=> msg2(s));

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

            arrHexTeleport = Instantiate(hexagono).GetComponent<Hexagone>();

            arrHexTeleport.gameObject.SetActive(false);//desactivo todo el resto de hexagonos, para que no consuman cpu

            //textMesh = arrHexCreados[i].GetComponentsInChildren<TextMeshProUGUI>();

            //arrHexCreados[i].GetComponent<Renderer>().material.SetColor("_Color", new Color(Random.Range(1, 11)/10f, Random.Range(1, 11) / 10f, Random.Range(1, 11) / 10f, 1f));
            arrHexTeleport.transform.position = CalculateHexagonoPos(i);

            rng = Random.Range(0, biomes.Length);

            arrHexTeleport.SetID(i).SetTileMap(map).SetBiome(biomes[rng]).SetTeleportEdge(hexagonos[i]).FillTilePos().FillPropsPos(i != 0, i == 0 || i == HexagonsManager.idMaxLevel);

            arrHexTeleport.name = "Hexagono " + i;
        }
        end(true);
    }

    
    IEnumerator CargaHexagonos(System.Action<bool> end, System.Action<string> msg)
    {   //el primero es la cantidad y el indice de hexagonos, le sumo uno de mas para tener el primero central, y desp el anterior vinculado
        //el segundo los lados, coloco uno mas, ya que voy a usar el lado 0, para preguntar si esta completo
        //el tercero es que id de hexagono [0] esta relacionado y el segundo [1] es a cual lado esta relacionado

        //cantidad = GameObject.FindGameObjectsWithTag("enemigo").Length;

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
            
        /*

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

        */
        //print("La cantidad de camaras es de: " + cameras.Length);

        //audioListener.enabled = true;

        
        yield return new WaitForCorutines(this, LoadHex, (s) => msg(s));    

        if (arrHexCreados.GetLength(0) > 1 && victoria!=null)
        {
            victoria = Instantiate(victoria);
            victoria.transform.position = arrHexCreados[HexagonsManager.idMaxLevel].transform.position;
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
}

