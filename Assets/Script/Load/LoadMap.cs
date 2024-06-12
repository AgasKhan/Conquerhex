using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LoadMap : SingletonMono<LoadMap>
{
    public bool execute = true;

    public int rng;

    public Pictionarys<Biomes, int> biomes;

    public Tilemap map;

    //public MoveAbstract playerPublic;

    public GameObject victoria;

    public Vector3 basePos= new Vector3 {x=2, y=20, z=10 };

    [SerializeField]
    int spawnCountLimitPerFrame = 500;

    float tiempoCarga = 0;

    Hexagone[] arrHexCreados => HexagonsManager.arrHexCreados;

    Hexagone hexagono => HexagonsManager.hexagonoPrefab;

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

        //StartCoroutine(VincularHexagonos());

        msg2("Ejecutando algoritmo de vinculacion de hexagonos");

        yield return new WaitForCorutinesForLoad(this, HexagonsManager.instance.VincularHexagonos, (s)=> msg2(s));

        string msg;

        for (int i = 0; i < hexagonos.GetLength(0); i++)
        {
            float persentage = ((i + 1f) / hexagonos.GetLength(0)) * 100;
            msg = $"<size=50>{Mathf.RoundToInt(persentage)}%\n<size=20> {i} de {hexagonos.GetLength(0)}";

            //espera para la carga
            if (GameManager.SlowFrameRate)
            {
                //LoadSystem.stopwatch.Restart();

                tiempoCarga += Time.unscaledDeltaTime;

                //Se borro el mensaje de escape para cancelar

                msg2(msg+ "\n0/4096\n");
                /*
                DebugPrint.Log("Carga del nivel: " + persentage + "%");
                DebugPrint.Log("Tiempo entre frames: " + (Time.unscaledDeltaTime) + " milisegundos");
                DebugPrint.Log("Tiempo total de carga: " + tiempoCarga);
                */
                yield return null;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopAllCoroutines();
                //LoadSystem.instance.Load("MainMenu");
            }

            //spawn hexagonos

            arrHexTeleport = Instantiate(hexagono);

            //arrHexTeleport.gameObject.SetActive(false);//desactivo todo el resto de hexagonos, para que no consuman cpu

            //textMesh = arrHexCreados[i].GetComponentsInChildren<TextMeshProUGUI>();

            //arrHexCreados[i].GetComponent<Renderer>().material.SetColor("_Color", new Color(Random.Range(1, 11)/10f, Random.Range(1, 11) / 10f, Random.Range(1, 11) / 10f, 1f));
            arrHexTeleport.transform.position = CalculateHexagonoPos(i);

            yield return arrHexTeleport
                .SetID(i)
                .SetTileMap(map)
                .SetBiome(biomes.RandomPic())
                .SetTeleportEdge(hexagonos[i])
                .SetTerrain()
                .FillPropsPosRoutine((str)=>msg2($"{msg}\n{str}\n") , i != 0, i == 0 || i == HexagonsManager.idMaxLevel,spawnCountLimitPerFrame);

            yield return arrHexTeleport.Off();

            arrHexTeleport.name = "Hexagono " + i;
        }
        //end(true);
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

                //LoadSystem.instance.Load("MainMenu");
            }
            yield return null;
        }
        
        yield return new WaitForCorutinesForLoad(this, LoadHex, (s) => msg(s));    

        if (arrHexCreados.GetLength(0) > 1 && victoria!=null)
        {
            victoria = Instantiate(victoria);
            victoria.transform.position = arrHexCreados[HexagonsManager.idMaxLevel].transform.position;
        }        

        tiempoCarga += Time.unscaledDeltaTime;
        DebugPrint.Log("Tiempo entre frames: " + (Time.unscaledDeltaTime) + " milisegundos");
        DebugPrint.Log("Tiempo total de carga: " + tiempoCarga);
        DebugPrint.Log("Carga finalizada");

        if(GameManager.instance.playerCharacter != null)
        {
            GameManager.instance.playerCharacter.transform.parent = arrHexCreados[0].transform;
            GameManager.instance.playerCharacter.transform.localPosition = new Vector3(0, 0, 0);
            arrHexCreados[0].EnterEntity(GameManager.instance.playerCharacter);
        }
        
        //end(true);
    }
}

