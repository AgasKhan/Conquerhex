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

    [SerializeReference]
    Hexagone[] _arrHexCreados;

    [SerializeField]
    Pictionarys<int, Hexagone> _activeHex = new Pictionarys<int, Hexagone>();

    [SerializeReference]
    float[,] _auxCalc = new float[6, 2];

    [SerializeReference]
    GameObject _hexagono;

    int[][,] _hexagonos;

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

        bool aislado = false;

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
                hex[hexIndex][0, 0] = 1;

                for (int lado = 1; lado < hex[hexIndex].GetLength(0); lado++)
                {
                    //le resto uno a la variable de los lados para que sea compatible con el sistema de la lista
                    lado--;

                    //calculo el lado opuesto (cuando quiera trabajarlo para mi array de hexagonos voy a tener que sumarle 1 para equivaler los indices)
                    int ladoOpuesto = ((lado - 3) >= 0) ? (lado - 3) : (lado + 3);

                    //Le vuelvo a incrementar 1 para dejarlo como estaba
                    lado++;

                    int auxL, auxI, auxRandom;
                    //Voy a guardar la cantidad maxima de indices para despues poder elegir uno al azar, dentro de la lista
                    //me encargo de no pisar un lado ya seteado
                    if (hex[hexIndex][lado, 1] == 0 && ((auxL = disponibles[ladoOpuesto].Count) > 0))
                    {
                        
                        do
                        {
                            //defino mi numero aleatorio en base a la disponibilidad (ya que los anteriores hexagonos ya los tengo seteados, no quiero pisarlos)
                            auxRandom = Random.Range(0, auxL);

                            //utilizo un while para no vincularme a mi mismo al menos que no quede otra opcion
                        } while(auxRandom == hexIndex && auxL>1);

                        auxL = auxRandom;

                        //obtengo el indice de los hexagonos a partir de la lista
                        auxI = disponibles[ladoOpuesto][auxL];

                        //print("lado " + ladoOpuesto + " ID " + disponibles[ladoOpuesto][auxL]);

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
        //correcion si por casualidad un hexagono se vinculo totalmente asi mismo
        for (int h = 0; h < hex.GetLength(0); h++)
        {
            bool auxAislado = true;

            for (int l = 1; l < hex[h].GetLength(0); l++)
            {
                if (hex[h][l, 0] != h)
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
            Debug.LogWarning("warning, rearmando");

            msg("Generacion no grata, rearmando");
            yield return null;
            yield return new WaitForCorutines(this,VincularHexagonos,(s)=>msg(s));
            msg("rearmando exitoso");
        }
        else
        {
            _hexagonos = hex;
        }

        msg("generacion correcta");

        end(true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hexagonos"></param>
    /// <returns></returns>
    bool ImprimirHexagonos(int[][,] hexagonos)
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
        return reload;
    }

    protected override void Awake()
    {
        base.Awake();
        scala = hexagono.transform.localScale.x;

        lado = scala / 2; //+ correccionScala;

        apotema = Mathf.Sqrt(Mathf.Pow(lado, 2) - Mathf.Pow(lado / 2, 2));

        LocalSidePosHex();

        DebugPrint.Log("Informacion de seteo");

        DebugPrint.Log("lado" + lado);

        DebugPrint.Log("apotema " + apotema);

        LoadSystem.AddPostLoadCorutine(() => {

            arrHexCreados[0].gameObject.SetActive(true);

            activeHex.Add(0, arrHexCreados[0]);

            arrHexCreados[0].SetRenders();
        });
    }
}
