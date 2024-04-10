using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonsManager : SingletonMono<HexagonsManager>
{
    public static Hexagone[] arrHexCreados => instance._arrHexCreados;

    public static Pictionarys<int, Hexagone> activeHex => instance._activeHex;

    public static float[,] auxCalc => instance._auxCalc;

    public static GameObject hexagono => instance._hexagono;

    public static int[][,] hexagonos => instance._hexagonos;

    public static float scala;

    public static float lado;

    public static float apotema;

    public Vector2 anguloDefecto;

    public static int idMaxLevel=> instance._idMaxLevel;

    [SerializeReference]
    Hexagone[] _arrHexCreados;

    [SerializeField]
    Pictionarys<int, Hexagone> _activeHex = new Pictionarys<int, Hexagone>();

    [SerializeReference]
    float[,] _auxCalc = new float[6, 2];

    [SerializeReference]
    GameObject _hexagono;

    int[][,] _hexagonos;

    [SerializeReference]
    int _idMaxLevel;

    public static void SetArrayHexagons(int number)
    {
        instance._hexagonos = new int[number][,];

        for (int i = 0; i < number; i++)
        {
            instance._hexagonos[i] = new int[7, 2];
        }

        instance._arrHexCreados = new Hexagone[instance._hexagonos.GetLength(0)];
    }

    public static int LadoOpuesto(int lado)
    {
        return ((lado - 3) >= 0) ? (lado - 3) : (lado + 3);
    }

    /// <summary>
    /// setea los renders en base a este hexagono
    /// </summary>
    /// <param name="lado"></param>
    public static void SetRenders(Hexagone hex, int lado = -1)
    {
        MainCamera.instance.SetProyections(hex);

        if (lado >= 0)
        {
            MainCamera.instance.transform.position = new Vector3(
                hex.ladosPuntos[LadoOpuesto(lado), 0] - (hex.ladosPuntos[lado, 0] - Camera.main.transform.position.x),
                hex.ladosPuntos[LadoOpuesto(lado), 1] - (hex.ladosPuntos[lado, 1] - Camera.main.transform.position.y),
                MainCamera.instance.transform.position.z);    
        }

        for (int i = 0; i < MainCamera.instance.rendersOverlay.Length; i++)
        {
            bool add = true;

            //chequeo la duplicidad
            for (int j = 0; j < activeHex.Count; j++)
            {
                if (hex.ladosArray[i].id == activeHex[j].id)
                    add = false;
            }

            if(add)
                activeHex.Add(hex.ladosArray[i].id, hex.ladosArray[i]);
        }

        instance.StartCoroutine(SetProyectionRoutine(hex));
    }

    static IEnumerator SetProyectionRoutine(Hexagone hex)
    {
        for (int i = activeHex.Count - 1; i >= 0; i--)
        {
            bool off = true;

            for (int l = 0; l < 6; l++)
            {
                if (hex.id == activeHex[i].id || hex.ladosArray[l].id == activeHex[i].id)
                {
                    off = false;
                    break;
                }
            }

            if (off)
            {
                activeHex[i].SetActiveGameObject(false);//desactivo todo el resto de hexagonos, para que no consuman cpu
                activeHex.RemoveAt(i);
            }
            else
            {
                activeHex[i].SetActiveGameObject(true);
            }

            if(GameManager.MediumFrameRate)
                yield return null;
        }
    }

    public void LocalSidePosHex(float magnitud = 1f)
    {
        DebugPrint.Log("Calculo de posición de lados");

        //calcula las coordenadas relativas de los lados de los hexagonos y lo retorna
        for (int i = 0; i < auxCalc.GetLength(0); i++)
        {
            /*
            auxCalc[i, 0] = ((((lado/2) * Mathf.Sin((1f / 3f) * Mathf.PI)) / (Mathf.Sin((1f / 6f) )* Mathf.PI))) * Mathf.Cos((1f / 2f) * Mathf.PI + (1f / 3f * Mathf.PI) * i);
            auxCalc[i, 1] = ((((lado/2) * Mathf.Sin((1f / 3f) * Mathf.PI)) / (Mathf.Sin((1f / 6f) )* Mathf.PI))) * Mathf.Sin((1f / 2f) * Mathf.PI + (1f / 3f * Mathf.PI) * i);
            */

            //Cuenta que calcula los puntos relativos
            auxCalc[i, 0] = (apotema) * Mathf.Cos((1f / 2f) * Mathf.PI - (1f / 3f) * Mathf.PI * i) * magnitud;
            auxCalc[i, 1] = (apotema) * Mathf.Sin((1f / 2f) * Mathf.PI - (1f / 3f) * Mathf.PI * i) * magnitud;

            DebugPrint.Log(auxCalc[i, 0] + " " + auxCalc[i, 1]);

        }
    }

    static public Vector3 AbsSidePosHex(Vector3 posicionInicial, int lado, float z, float multiplicador = 1)
    {
        //accede a la variable global que contiene las coordenadas relativas y las guarda en un vector3, asi mismo permite multiplicarlas por un escalar
        return new Vector3(posicionInicial.x + auxCalc[lado, 0] * multiplicador, posicionInicial.y + auxCalc[lado, 1] * multiplicador, z);
    }

    public IEnumerator VincularHexagonos(System.Action<bool> end, System.Action<string> msg)
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

        int[][,] hex = hexagonos;

        //if (comprobacion)
            for (int h = 0; h < hex.GetLength(0); h++)
            {
                for (int l = 0; l < hex[h].GetLength(0); l++)
                {
                    for (int i = 0; i < hex[h].GetLength(1); i++)
                    {
                        hex[h][l, i] = 0;
                    }
                }
            }


        for (int i = 0; i < disponibles.Length; i++)
        {
            disponibles[i] = new List<int>();
            for (int h = 0; h < hex.GetLength(0); h++)
            {
                if (hex[h][0, 0] == 0)
                    disponibles[i].Add(h);
            }
        }

        for (int hexIndex = 0; hexIndex < hex.GetLength(0); hexIndex++)
        {
            if (hexIndex % 1000 == 0)
            {

                msg("Ejecutando algoritmo de vinculacion de hexagonos" +
                    "\n" +
                    "<size=20>" + hexIndex + " de " + hex.GetLength(0) + "</size>" +
                    "\n" +
                    "Presiona escape para cancelar");

                yield return null;
            }


            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopAllCoroutines();
                LoadSystem.instance.Load("Menu");
            }


            if (hex[hexIndex][0, 0] == 0) //if que se encarga de preguntar si alguno ya esta completo,
                                          //esto puede pasar en caso de que el array tenga algun hexagono seteado de antemano
            {
                hex[hexIndex][0, 0] = 1;//nivel del hexagono

                for (int lado = 1; lado < hex[hexIndex].GetLength(0); lado++)
                {
                    //le resto uno a la variable de los lados para que sea compatible con el sistema de la lista
                    lado--;

                    //calculo el lado opuesto (cuando quiera trabajarlo para mi array de hexagonos voy a tener que sumarle 1 para equivaler los indices)
                    int ladoOpuesto = LadoOpuesto(lado);

                    //Le vuelvo a incrementar 1 para dejarlo como estaba
                    lado++;

                    int auxL, auxI;
                    //Voy a guardar la cantidad maxima de indices para despues poder elegir uno al azar, dentro de la lista
                    //me encargo de no pisar un lado ya seteado
                    if (hex[hexIndex][lado, 1] == 0 && ((auxL = disponibles[ladoOpuesto].Count) > 0))
                    {

                        auxL = Random.Range(0, auxL);

                        //obtengo el indice de los hexagonos a partir de la lista
                        auxI = disponibles[ladoOpuesto][auxL];

                        //quito tanto el elegido de forma aleatoria como el que estoy parado ahora
                        disponibles[ladoOpuesto].RemoveAt(auxL);
                        disponibles[lado - 1].Remove(hexIndex);

                        //seteo mi hexagono
                        hex[hexIndex][lado, 0] = auxI;
                        hex[hexIndex][lado, 1] = ladoOpuesto + 1;//le sumo 1 por q el indice de los hexagonos arranca en 1 en comparacion al de las listas que arranca en 0

                        //seteo el otro hexagono
                        hex[auxI][hex[hexIndex][lado, 1], 0] = hexIndex;
                        hex[auxI][hex[hexIndex][lado, 1], 1] = lado;
                    }
                    else if (hex[hexIndex][lado, 1] == 0)
                    {
                        hex[hexIndex][lado, 0] = -1;
                    }
                    //print(hexagonos[h, l, 0]);
                }
            }
        }
      

        if (ChequeoHexagonos(hex))
        {
            DebugPrint.Warning("Generacion no grata, rearmando");
            Debug.LogWarning("warning, rearmando");

            msg("Generacion no grata, rearmando");
            yield return null;
            yield return new WaitForCorutinesForLoad(this,VincularHexagonos,(s)=>msg(s));
            msg("rearmado exitoso");
        }
        else
        {
            _hexagonos = hex;
        }

        msg("generacion correcta");

        //end(true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hexagonos"></param>
    /// <returns></returns>
    bool ChequeoHexagonos(int[][,] hexagonos)
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

            for (int l = 1; l < hexagonos[h].GetLength(0); l++)
                pantalla += "|\tLado \t ? \tid \tlado\t|";

            pantalla += "\n";

            for (int l = 1; l < hexagonos[h].GetLength(0); l++)
            {
                pantalla += "|\t" + l + "\t?\t" + hexagonos[h][l, 0] + "\t" + hexagonos[h][l, 1] + "\t|";
                //print("| Lado " + l + " teleport hexagono: " + hexagonos[h, l, 0] + " al lado: " + hexagonos[h, l, 1] + " |");
                if (h != hexagonos[hexagonos[h][l, 0]][hexagonos[h][l, 1], 0])
                {
                    DebugPrint.Error("Error en Lado " + l + " de la id: " + h + " resultado no esperado: l " + hexagonos[hexagonos[h][l, 0]][hexagonos[h][l, 1], 1] + " ,h " + hexagonos[hexagonos[h][l, 0]][hexagonos[h][l, 1], 0]);

                    DebugPrint.Error(h + " " + hexagonos[hexagonos[h][l, 0]][hexagonos[h][l, 1], 0]);

                    reload = true;
                }
            }
            DebugPrint.Log(pantalla);
        }

        if(reload)
            return reload;

        List<int> checkedList = new List<int>();//historial
        Queue<int> toCheck = new Queue<int>();

        toCheck.Enqueue(0);//agrego mi comienzo
        checkedList.Add(0); //lo agrego como ya verificado, solo por ser el comienzo

        hexagonos[0][0, 0] = 0;

        int maxLevel = 0;

        while (toCheck.TryDequeue(out int current))//si ya no tengo nada mas que chequear significa que ya recorri todos los posibles hexagonos desde el comienzo
        {
            for (int l = 1; l < hexagonos[current].GetLength(0); l++)
            {
                if(!checkedList.Contains(hexagonos[current][l,0]))
                {
                    toCheck.Enqueue(hexagonos[current][l, 0]);
                    checkedList.Add(hexagonos[current][l, 0]);
                    hexagonos[hexagonos[current][l, 0]][0, 0] = hexagonos[current][0, 0] + 1;
                    
                    if (maxLevel < hexagonos[hexagonos[current][l, 0]][0, 0])
                    {
                        maxLevel = hexagonos[hexagonos[current][l, 0]][0, 0];
                        _idMaxLevel = hexagonos[current][l, 0];
                    }
                }
            }
        }

        if(checkedList.Count != hexagonos.Length)//si mi cantidad de hexagonos no es la misma que la que puedo recorrer re armo el recorrido
        {
            reload = true;
            Debug.Log(checkedList.Count + " " + hexagonos.GetLength(0));
        }

        return reload;
    }

    static public int CalcEdge(Vector2 direction, float angulo)
    {
        float angle = 360 - Utilitys.DifAngulosVectores(instance.anguloDefecto, direction);

        int maxlado = (int)(360 / angulo);

        int lado = Mathf.FloorToInt(angle / angulo);

        if (lado >= maxlado)
            lado = maxlado-1;

        return lado;
    }

    static public int CalcEdge(Vector2 direction)
    {
        return CalcEdge(direction, 60);
    }

    protected override void Awake()
    {
        base.Awake();
        scala = hexagono.transform.localScale.x;

        lado = scala / 2; //+ correccionScala;

        apotema = Mathf.Sqrt(Mathf.Pow(lado, 2) - Mathf.Pow(lado / 2, 2));

        anguloDefecto = new Vector2(Mathf.Cos((2f / 3f) * Mathf.PI), Mathf.Sin((2f / 3f) * Mathf.PI));

        LocalSidePosHex();

        DebugPrint.Log("Informacion de seteo");

        DebugPrint.Log("lado" + lado);

        DebugPrint.Log("apotema " + apotema);

        LoadSystem.AddPostLoadCorutine(() => {

            arrHexCreados[0].gameObject.SetActive(true);

            activeHex.Add(0, arrHexCreados[0]);

            SetRenders(arrHexCreados[0]);
        });
    }
}
