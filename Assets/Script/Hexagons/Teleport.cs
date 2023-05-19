using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public int id;

    //hacia donde se teletransporta, el primer indice es el lado del propio hexagono, y el segundo es el destino (0 para la id y 1 para el lado)
    //public int[,] ladosArray;

    public Teleport[] ladosArray = new Teleport[6];//Lo uso para definir A DONDE me voy a teletransportar

    //pareja de coordenadas
    public float[,] ladosPuntos = new float[6, 2];//Lo uso para guardar la coordenadas de cada lado

    public float velocityTransfer;

    public Vector2 anguloDefecto;

   
    public void SetTeleportEdge(int[,] ladosArray)
    {
        LoadSystem.AddPostLoadCorutine(() => {

            for (int ii = 0; ii < ladosArray.GetLength(0) - 1; ii++)
            {
                this.ladosArray[ii] = HexagonsManager.arrHexCreados[ladosArray[ii + 1, 0]];
                //this.ladosArray[ii] = ladosArray[ii + 1, 1] - 1; //Le resto para tener el indice en 0

                //para X e Y
                this.ladosPuntos[ii, 0] = transform.position.x + HexagonsManager.auxCalc[ii, 0];
                this.ladosPuntos[ii, 1] = transform.position.y + HexagonsManager.auxCalc[ii, 1];
            }
        });
    }

    private void Awake()
    {
        //vector que da el angulo por defecto para calcular el hexagono (esta a 120 grados)
        anguloDefecto = new Vector2(Mathf.Cos((2f / 3f) * Mathf.PI), Mathf.Sin((2f / 3f) * Mathf.PI));
    }


}
