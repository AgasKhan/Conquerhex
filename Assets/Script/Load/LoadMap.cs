using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LoadMap : MonoBehaviour
{
    public static LoadMap instance;

    static public GameObject[] arrHexCreados;

    static private float[,] auxCalc = new float[6, 2];

    public GameObject[] carlitos;

    public Camera[] cameras;

    public GameObject[] renders;

    public int rng;

    public LoadScreen loadScreen;

    public GameObject hexagono;

    public Tile[] pastito;

    public Tile[] nieve;

    public Tilemap map;

    public GameObject playerPublic;

    public GameObject enemigos;

    public GameObject victoria;

    int[,,] hexagonos;

    float[] randomList;

    float tiempoCarga = 0;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        StartCoroutine(CargaHexagonos());
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator LoadHex(System.Action<bool> end, System.Action<string> msg2)
    {
        hexagonos = new int[rng, 7, 2];

        Teleport arrHexTeleport;//Despues lo voy a usar para guardar el getcomponent

        //TextMeshProUGUI[] textMesh; //para setear los numeros de las torres

        float scala = hexagono.transform.localScale.x;

        float lado = scala / 2; //+ correccionScala;

        float apotema = Mathf.Sqrt(Mathf.Pow(lado, 2) - Mathf.Pow(lado / 2, 2));

        float x = lado * 2 * 2;
        float y = 20;
        float z = 10;

        Tile[][] terreno = { pastito, nieve };

        //reveer
        Object[][] props = new Object[terreno.GetLength(0)][];

        arrHexCreados = new GameObject[hexagonos.GetLength(0)];

        DebugPrint.Log("Informacion de seteo");

        DebugPrint.Log("lado" + lado);

        DebugPrint.Log("apotema " + apotema);

        for (int i = 0; i < terreno.GetLength(0); i++)
        {
            string path = "Props/terreno_" + i + "/";

            props[i] = Euler.LoadAsset(path);
        }

        StartCoroutine(VincularHexagonos());

        //yield return new WaitForCorutines(this, VincularHexagonos, (s)=> { });

        while (hexagonos[hexagonos.GetLength(0) - 1, hexagonos.GetLength(1) - 1, hexagonos.GetLength(2) - 1] == 0)
            yield return null;

        auxCalc = Euler.LocalSidePosHex(auxCalc, apotema);

        for (int i = 0; i < hexagonos.GetLength(0); i++)
        {
            //espera para la carga
            if (i % 45 == 0)
            {
                tiempoCarga += Time.unscaledDeltaTime;

                float persentage = ((i + 1f) / hexagonos.GetLength(0) )*100;

                string msg = Mathf.RoundToInt(persentage) + "%" +
                    "\n<size=20>" +
                    i + " de " + hexagonos.GetLength(0) + "\n" +
                    "<size=25>Escape para cancelar";

                loadScreen.Progress(persentage , msg);

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

            arrHexCreados[i] = Instantiate(hexagono);

            arrHexCreados[i].SetActive(false);//desactivo todo el resto de hexagonos, para que no consuman cpu

            //textMesh = arrHexCreados[i].GetComponentsInChildren<TextMeshProUGUI>();

            arrHexTeleport = arrHexCreados[i].GetComponent<Teleport>();

            arrHexCreados[i].transform.position = new Vector3(x * (i + 1), y, z);

            //arrHexCreados[i].GetComponent<Renderer>().material.SetColor("_Color", new Color(Random.Range(1, 11)/10f, Random.Range(1, 11) / 10f, Random.Range(1, 11) / 10f, 1f));

            arrHexTeleport.id = i;

            arrHexTeleport.name = "Hexagono " + i;

            arrHexTeleport.ladosArray = new int[6, 2];//Lo uso para definir A DONDE me voy a teletransportar
            arrHexTeleport.ladosPuntos = new float[6, 2];//Lo uso para guardar la coordenadas de cada lado

            rng = Random.Range(0, terreno.GetLength(0));

            FillTilePos(terreno[rng], arrHexCreados[i].transform.position, 42);

            FillPropsPos(props[rng], arrHexCreados[i], 42, apotema, i != 0, i == 0 || i == hexagonos.GetLength(0) - 1);

            for (int ii = 0; ii < hexagonos.GetLength(1) - 1; ii++)
            {
                arrHexTeleport.ladosArray[ii, 0] = hexagonos[i, ii + 1, 0];
                arrHexTeleport.ladosArray[ii, 1] = hexagonos[i, ii + 1, 1] - 1; //Le resto para tener el indice en 0
                //para X e Y
                arrHexTeleport.ladosPuntos[ii, 0] = (x * (i + 1)) + auxCalc[ii, 0];
                arrHexTeleport.ladosPuntos[ii, 1] = y + auxCalc[ii, 1];

                //Debug.DrawRay(arrHexCreados[i].transform.position, new Vector2(auxCalc[ii, 0], auxCalc[ii, 1]) , Color.cyan, 60);

                //Debug.DrawRay(arrHexCreados[i].transform.position, new Vector2(Mathf.Cos((1f / 2f)*Mathf.PI), Mathf.Sin((1f / 2f) * Mathf.PI)), Color.red, 60);

                //textMesh[ii].text = hexagonos[i, ii + 1, 0].ToString();
                //nTorre++;
            }
            /*
            textMesh[0].text = "<color=#FF0000>" + hexagonos[i, 1, 0] + "\n\n<color=#0000FF>" + hexagonos[i, 6, 0];
            textMesh[1].text = "<color=#FF0000>" + hexagonos[i, 1, 0] + "\n\n<color=#00FF00>" + hexagonos[i, 2, 0];

            textMesh[2].text = "<color=#0000FF>" + hexagonos[i, 3, 0] + "\n\n<color=#00FF00>" + hexagonos[i, 2, 0];
            textMesh[3].text = "<color=#0000FF>" + hexagonos[i, 3, 0] + "\n\n<color=#FF0000>" + hexagonos[i, 4, 0];

            textMesh[4].text = "<color=#00FF00>" + hexagonos[i, 5, 0] + "\n\n<color=#FF0000>" + hexagonos[i, 4, 0];
            textMesh[5].text = "<color=#00FF00>" + hexagonos[i, 5, 0] + "\n\n<color=#0000FF>" + hexagonos[i, 6, 0];
            */
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
    IEnumerator CargaHexagonos()
    {   //el primero es la cantidad y el indice de hexagonos, le sumo uno de mas para tener el primero central, y desp el anterior vinculado
        //el segundo los lados, coloco uno mas, ya que voy a usar el lado 0, para preguntar si esta completo
        //el tercero es que id de hexagono [0] esta relacionado y el segundo [1] es a cual lado esta relacionado

        //cantidad = GameObject.FindGameObjectsWithTag("enemigo").Length;

        Teleport arrHexTeleport;

        int min = PlayerPrefs.GetInt("nivelMin", 0);
        int max = PlayerPrefs.GetInt("nivelMax", 5);

        Time.timeScale = 0;

        randomList = GenRandomList(max - min);

        rng = (rng > 0) ? rng : PlayerPrefs.GetInt("Hex", 0);

        rng = (rng > 0) ? rng : Random.Range(3, 21);

        DebugPrint.Log("Cantidad de memoria: " + SystemInfo.systemMemorySize);

        while (SystemInfo.systemMemorySize < 14 * rng * 4 / 1000)
        {
            loadScreen.Progress(0, "Has colocado mas hexagonos que lo que dispones de RAM" +
                "\n" +
                "Pulsa escape para volver");

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopAllCoroutines();
                LoadSystem.instance.Load("Menu");

            }
            yield return null;
        }

        GameManager.instance.player = playerPublic;

        cameras = new Camera[Camera.allCamerasCount - 1];

        renders = new GameObject[6];

        carlitos = new GameObject[6];

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

        loadScreen.Progress(0, "Ejecutando algoritmo de vinculacion de hexagonos");

        string msg2;

        //perdida del flujo
        yield return new WaitForCorutines(this, LoadHex, (s) => msg2 = s );

        //prototipo de us

        //audioListener.enabled = false;

        //configuro las camaras y los carlitos para el primer hexagono
        for (int i = 0; i < cameras.Length; i++)
        {
            arrHexTeleport = arrHexCreados[0].GetComponent<Teleport>();

            arrHexCreados[0].SetActive(true);

            arrHexCreados[arrHexTeleport.ladosArray[i, 0]].SetActive(true);

            cameras[i].gameObject.transform.position = new Vector3(
                arrHexCreados[arrHexTeleport.ladosArray[i, 0]].transform.position.x,
                arrHexCreados[arrHexTeleport.ladosArray[i, 0]].transform.position.y,
                cameras[i].gameObject.transform.position.z
            );

            //cameras[i].orthographicSize = correcionZoom;

            renders[i] = GameObject.Find("render (" + i + ")");

            renders[i].transform.position = AbsSidePosHex(arrHexCreados[0].transform.position, i, renders[i].transform.position.z, 2);

            //renders[i].transform.localScale = Vector3.one * scala;

            carlitos[i] = GameObject.Find("Carlitos (" + i + ")");

            carlitos[i].transform.position = AbsSidePosHex(arrHexCreados[arrHexTeleport.ladosArray[i, 0]].transform.position, ((i - 3) >= 0) ? (i - 3) : (i + 3), carlitos[i].transform.position.z, 2);

        }

        if (arrHexCreados.GetLength(0) > 1)
        {
            victoria = Instantiate(victoria);
            victoria.transform.position = arrHexCreados[arrHexCreados.GetLength(0) - 1].transform.position;
        }


        yield return null;

        
        tiempoCarga += Time.unscaledDeltaTime;
        DebugPrint.Log("Tiempo entre frames: " + (Time.unscaledDeltaTime) + " milisegundos");
        DebugPrint.Log("Tiempo total de carga: " + tiempoCarga);
        DebugPrint.Log("Carga finalizada");

        loadScreen.Progress(100, "<size=50>Carga finalizada</size>" +
            "\n<size=20> Presione <color=green>espacio</color> para continuar </size>");

        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }

        playerPublic.transform.parent = arrHexCreados[0].transform;
        playerPublic.transform.localPosition = new Vector3(0, 0, 0);

        loadScreen.gameObject.SetActive(false);

        yield return null;
        Time.timeScale = 1;
        //AudioManager.instance.Play("ambiente").source.loop = true;

    }

    static public Vector3 AbsSidePosHex(Vector3 posicionInicial, int lado, float z, float multiplicador = 1)
    {
        //accede a la variable global que contiene las coordenadas relativas y las guarda en un vector3, asi mismo permite multiplicarlas por un escalar
        return new Vector3(posicionInicial.x + auxCalc[lado, 0] * multiplicador, posicionInicial.y + auxCalc[lado, 1] * multiplicador, z);
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

                        prop.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(prop.transform.position.y * -100);

                        prop.transform.localScale = new Vector3(prop.transform.localScale.x * (Random.Range(0, 2) == 0 ? -1 : 1), prop.transform.localScale.y, prop.transform.localScale.z);



                        prop.transform.SetParent(hex.transform);
                    }
                    else if (Random.Range(0, 3) == 0 && spawn)
                    {
                        SpawnEnemy(hex.transform, new Vector3(i, ii, center.z));
                    }
                }


            }
        }
    }

    void SpawnEnemy(Transform padre, Vector3 pos)
    {
        float aux = NivelGen();

        GameObject enemigo = Instantiate(enemigos);

        float vida = enemigo.GetComponent<Vida>().maxHp;

        ControlEnemigo enemigoController = enemigo.GetComponent<ControlEnemigo>();

        enemigo.transform.position = pos;

        enemigo.GetComponent<Vida>().SetHp(Random.Range(5, 21));

        enemigoController.nivel = aux;

        enemigoController.enfriamientoDisparoConst = Random.Range(1, 4) / (3f);

        string rango;

        switch (enemigoController.enfriamientoDisparoConst)
        {
            case 0.5f:
                rango = "<color=red>";
                break;
            case (1 / 3f):
                rango = "<color=blue>";
                break;
            default:
                rango = "<color=green>";
                break;
        }

        enemigo.name = rango + "Enemigo Nv: " + enemigoController.nivel + "</color>";

        enemigoController.velocidad = (aux / 10f) + 10;

        enemigoController.velocidadProyectil = Random.Range(5, 10);

        enemigoController.danio = (aux + 10) * 0.1f * enemigoController.enfriamientoDisparoConst;

        enemigoController.deteccion = enemigoController.enfriamientoDisparoConst * 30 + aux / 100f;

        enemigo.GetComponentInChildren<SpriteRenderer>().color -= new Color(1 - vida / 20, 1 - vida / 20, 0, 0);

        enemigo.transform.SetParent(padre);
    }

    float[] GenRandomList(int cantidad)
    {
        int valor = cantidad + 1;

        int extra = 2;//si necesitas comprobar el algoritmo base, cambia esto por 0

        valor += extra; //debido a q el algoritmo base deja el ultimo valor en 0, le sumo 2 para asi compensar este detalle

        float casilla = 100f / valor; //cuanto vale cada celda si todos tuviesen el mismo porcentaje

        int fix = casilla % 2 == 0 ? 0 : 1;//en caso de q mis casillas sean impares le sumare uno al array para compensar

        float cuenta;

        float[] array = new float[valor]; //relleno el array con todos esos porcentajes

        for (int i = 0; i < array.Length; i++)
        {
            array[i] = casilla;
        }

        casilla = 2 * casilla / 100; //saco el porcentaje de lo q representan 2 filas de todos los numeros generados

        for (int i = 0; i < array.Length / 2 + fix; i++)
        {
            cuenta = (array[i] - (array[i] * casilla * i)) * (1 - casilla * i);//Le resto a mi casilla el porcentaje de las casillas recorridas, y lo multiplico por este mismo, para asi ir decrementando el numero
            array[i] += cuenta;
            array[array.Length - 1 - i] -= cuenta;
        }

        //array[0]+= array[array.length-extra];//le sumo la ante ultima linea

        //array = array.map((val) => val + array[array.length - extra] / (valor - extra));//correcion lineal, le sumo esa diferencia a todos los elementos

        for (int i = 0; i < array.Length / 2 + fix; i++)
        {
            array[i] += array[array.Length - extra] / (valor - extra);
        }

        for (int i = 1; i < array.Length; i++)
        {
            array[i] += array[i - 1];
        }

        return array;
    }

    int NivelGen()
    {
        int min = PlayerPrefs.GetInt("nivelMin", 0);

        float rngF = Random.Range(1f, 100f);

        int nivel = min;

        /*DebugPrint.Log("Array nivel \n------------------");
        for (int i = 0; i < randomList.Length; i++)
        {
            DebugPrint.Log("Array " + i + " %" + randomList[i]);
        }
        */
        for (int i = 0; i < randomList.Length; i++)
        {
            if (rngF < randomList[i])
            {
                //DebugPrint.Log("Nivel elegido: " + (nivel+i));
                //DebugPrint.Log("Fin \n------------------");
                return (nivel + i);
            }
        }

        DebugPrint.Warning("no eligio nada de la lista de niveles");
        return nivel;
    }

    bool ImprimirHexagonos(int[,,] hexagonos)
    {
        /*
            Ademas de mostrar en consola el valor del array 
            muestra errores de relacion, en caso de que existan
        */
        bool reload = false;

        DebugPrint.Log("Mapeado de hexagonos");

        for (int h = 0; h < hexagonos.GetLength(0); h++)
        {
            string pantalla = "\n\n<color=green>hexagono con ID: " + h + "</color>\n";

            for (int l = 1; l < hexagonos.GetLength(1); l++)
                pantalla += "|\tLado \t ? \tid \tlado\t|";

            pantalla += "\n";

            for (int l = 1; l < hexagonos.GetLength(1); l++)
            {
                pantalla += "|\t" + l + "\t?\t" + hexagonos[h, l, 0] + "\t" + hexagonos[h, l, 1] + "\t|";
                //print("| Lado " + l + " teleport hexagono: " + hexagonos[h, l, 0] + " al lado: " + hexagonos[h, l, 1] + " |");
                if (h != hexagonos[hexagonos[h, l, 0], hexagonos[h, l, 1], 0])
                {
                    DebugPrint.Error("Error en Lado " + l + " de la id: " + h + " resultado no esperado: l " + hexagonos[hexagonos[h, l, 0], hexagonos[h, l, 1], 1] + " ,h " + hexagonos[hexagonos[h, l, 0], hexagonos[h, l, 1], 0]);

                    DebugPrint.Error(h + " " + hexagonos[hexagonos[h, l, 0], hexagonos[h, l, 1], 0]);

                    reload = true;
                }
            }

            DebugPrint.Log(pantalla);

        }



        return reload;

    }

    IEnumerator VincularHexagonos(bool comprobacion = false)
    {
        /*
        La funcion toma un array de 3 dimensiones y crea la informacion asi mismo ordena el array para el sistema de hexagonos
        procedural
         */

        //el array de listas, contiene 6 listas, una por cada lado,
        //en este caso siendo equivalente al lado 0 de las listas al lado 1 de los arrays,
        //dentro voy a guardar cada uno de los lados disponibles,
        //invirtiendo el orden en comparacion del array de hexagonos,
        //poniendo primero el lado y desp el indice del array
        List<int>[] disponibles = new List<int>[6];

        int[,,] hex = hexagonos;

        bool aislado = false;

        if (comprobacion)
            for (int h = 0; h < hex.GetLength(0); h++)
            {
                for (int l = 0; l < hex.GetLength(1); l++)
                {
                    for (int i = 0; i < hex.GetLength(2); i++)
                    {
                        hex[h, l, i] = 0;
                    }
                }

            }

        for (int i = 0; i < disponibles.Length; i++)
        {
            disponibles[i] = new List<int>();
            for (int h = 0; h < hex.GetLength(0); h++)
            {
                if (hex[h, 0, 0] == 0)
                    disponibles[i].Add(h);
            }
        }

        for (int h = 0; h < hex.GetLength(0); h++)
        {
            if (h % 1000 == 0)
            {

                loadScreen.Progress("Ejecutando algoritmo de vinculacion de hexagonos" +
                    "\n" +
                    "<size=20>" + h + " de " + hex.GetLength(0) + "</size>" +
                    "\n" +
                    "Presiona escape para cancelar");

                yield return null;
            }


            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopAllCoroutines();
                LoadSystem.instance.Load("Menu");

            }


            if (hex[h, 0, 0] == 0) //if que se encarga de preguntar si alguno ya esta completo,
                                   //esto puede pasar en caso de que el array tenga algun hexagono seteado de antemano
            {
                hex[h, 0, 0] = 1;

                for (int l = 1; l < hex.GetLength(1); l++)
                {
                    //le resto uno a la variable de los lados para que sea compatible con el sistema de la lista
                    l--;

                    //calculo el lado opuesto (cuando quiera trabajarlo para mi array de hexagonos voy a tener que sumarle 1 para equivaler los indices)
                    int ladoOpuesto = ((l - 3) >= 0) ? (l - 3) : (l + 3);

                    //Le vuelvo a incrementar 1 para dejarlo como estaba
                    l++;

                    int auxL, auxI;
                    //Voy a guardar la cantidad maxima de indices para despues poder elegir uno al azar, dentro de la lista
                    //me encargo de no pisar un lado ya seteado
                    if (hex[h, l, 1] == 0 && ((auxL = disponibles[ladoOpuesto].Count) > 0))
                    {
                        //defino mi numero aleatorio en base a la disponibilidad (ya que los anteriores hexagonos ya los tengo seteados, no quiero pisarlos)
                        auxL = Random.Range(0, auxL);

                        //obtengo el indice de los hexagonos a partir de la lista
                        auxI = disponibles[ladoOpuesto][auxL];

                        //print("lado " + ladoOpuesto + " ID " + disponibles[ladoOpuesto][auxL]);

                        //quito tanto el elegido de forma aleatoria como el que estoy parado ahora
                        disponibles[ladoOpuesto].RemoveAt(auxL);
                        disponibles[l - 1].Remove(h);

                        //seteo mi hexagono
                        hex[h, l, 0] = auxI;
                        hex[h, l, 1] = ladoOpuesto + 1;//le sumo 1 por q el indice de los hexagonos arranca en 1 en comparacion al de las listas que arranca en 0

                        //seteo el otro hexagono
                        hex[auxI, hex[h, l, 1], 0] = h;
                        hex[auxI, hex[h, l, 1], 1] = l;
                    }
                    else if (hex[h, l, 1] == 0)
                    {
                        hex[h, l, 0] = -1;
                    }
                    //print(hexagonos[h, l, 0]);
                }
            }
        }
        //correcion si por casualidad un hexagono se vinculo totalmente asi mismo
        for (int h = 0; h < hex.GetLength(0); h++)
        {
            bool auxAislado = true;

            for (int l = 1; l < hex.GetLength(1); l++)
            {
                if (hex[h, l, 0] != h)
                {
                    auxAislado = false;
                }
            }
            if (auxAislado)
            {
                aislado = true;
            }
        }

        if (ImprimirHexagonos(hex) || (aislado && hex.GetLength(0) > 1))
        {
            DebugPrint.Warning("Generacion no grata, rearmando");
            DebugPrint.Warning("warning");
            StopCoroutine(VincularHexagonos());
            VincularHexagonos(true);
        }
        else
        {
            hexagonos = hex;
        }
            
    }

   
}
