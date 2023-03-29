using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldEnemySpawner : MonoBehaviour
{
    float[] randomList;

    int max;
    int min;

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

    void SpawnEnemy(Transform padre, Vector3 pos)
    {
        float aux = NivelGen();

        randomList = GenRandomList(max - min);

        GameObject enemigo = null;// = Instantiate(enemigos);

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
}
