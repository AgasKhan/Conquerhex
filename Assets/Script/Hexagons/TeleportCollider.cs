using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCollider : MonoBehaviour
{

    [SerializeField]
    Teleport teleport;

    int id => teleport.id;

    //hacia donde se teletransporta, el primer indice es el lado del propio hexagono, y el segundo es el destino (0 para la id y 1 para el lado)
    int[,] ladosArray => teleport.ladosArray;

    //pareja de coordenadas
    float[,] ladosPuntos => teleport.ladosPuntos;

    float velocityTransfer => teleport.velocityTransfer;

    Vector2 anguloDefecto => teleport.anguloDefecto;

    Teleport[] arrHexCreados => HexagonsManager.arrHexCreados;

    Pictionarys<int, Teleport> activeHex => HexagonsManager.activeHex;

    void OnTriggerStay2D(Collider2D other)
    {
        MoveAbstract fisicaOther = other.GetComponent<MoveAbstract>();

        if (fisicaOther != null)
        {
            float angle;
            float[] difEspejada = new float[2]; //voy a guardar la diferencia para poder espejarlo de forma correcta
            int lado;
            //PrintF log = new PrintF();

            Vector2 vectorSalida = (other.gameObject.transform.position - this.gameObject.transform.position).normalized;

            fisicaOther.Acelerator(velocityTransfer * fisicaOther.direction);

            Vector2 vectorVelocidad = fisicaOther.vectorVelocity;

            angle = 360 - Utilitys.DifAngulosVectores(anguloDefecto, vectorSalida);

            lado = Mathf.FloorToInt(angle / 60);

            Teleport arrHexTeleport = HexagonsManager.arrHexCreados[ladosArray[lado, 0]].GetComponent<Teleport>();//accedo al script del array al que me quiero teletransportar

            float anguloVelocidad = Utilitys.DifAngulosVectores(new Vector2(Mathf.Cos((lado * -60) * Mathf.Deg2Rad), Mathf.Sin((lado * -60) * Mathf.Deg2Rad)), vectorVelocidad);

            //aplico una velocidad al objeto que esta cerca del portal

            //Para detectar a donde va el objeto
            Debug.DrawRay(this.gameObject.transform.position, (other.gameObject.transform.position - this.gameObject.transform.position).normalized, Color.green, 30);

            Debug.DrawRay(this.gameObject.transform.position, anguloDefecto, Color.red, 30);

            if
            (anguloVelocidad < 180 && anguloVelocidad > 0)
            {
                //tp?.Invoke();

                difEspejada[0] = ladosPuntos[lado, 0] - other.transform.position.x;
                difEspejada[1] = ladosPuntos[lado, 1] - other.transform.position.y;

                /*
                 DebugPrint.Log("Efectuado teletransporte");

                 DebugPrint.Log("Salio del hexagono " + other.name + " con el angulo: "+angle + " y el lado: " + lado);

                 DebugPrint.Log("transportando al ID: " + (ladosArray[lado,0]) + " Y al lado " + (ladosArray[lado, 1]));
                 */

                other.gameObject.transform.position =
                    new Vector3(
                        arrHexTeleport.ladosPuntos[ladosArray[lado, 1], 0] - difEspejada[0],
                        arrHexTeleport.ladosPuntos[ladosArray[lado, 1], 1] - difEspejada[1],
                        other.gameObject.transform.position.z);

                other.gameObject.transform.SetParent(arrHexTeleport.gameObject.transform);
                //le doy un empujon para que no se quede en el medio

                for (int i = 0; i < fisicaOther.carlitos.Length; i++)
                {
                    fisicaOther.carlitos[i].transform.position = HexagonsManager.AbsSidePosHex(arrHexCreados[arrHexTeleport.ladosArray[i, 0]].transform.position, ((i - 3) >= 0) ? (i - 3) : (i + 3), LoadMap.instance.carlitos[i].transform.position.z, 2) + (other.gameObject.transform.position - arrHexCreados[ladosArray[lado, 0]].transform.position);
                }

                if (other.CompareTag("Player"))
                {
                    MainCamera.instance.gameObject.transform.position = new Vector3(
                        arrHexTeleport.ladosPuntos[ladosArray[lado, 1], 0] - (ladosPuntos[lado, 0] - Camera.main.gameObject.transform.position.x),
                        arrHexTeleport.ladosPuntos[ladosArray[lado, 1], 1] - (ladosPuntos[lado, 1] - Camera.main.gameObject.transform.position.y),
                        Camera.main.gameObject.transform.position.z);

                    for (int i = 0; i < LoadMap.instance.renders.Length; i++)
                    {
                        arrHexCreados[arrHexTeleport.ladosArray[i, 0]].gameObject.SetActive(true);
                        activeHex.Add(arrHexTeleport.ladosArray[i, 0], arrHexCreados[arrHexTeleport.ladosArray[i, 0]]);

                        LoadMap.instance.renders[i].transform.position = HexagonsManager.AbsSidePosHex(arrHexCreados[ladosArray[lado, 0]].transform.position, i, LoadMap.instance.renders[i].transform.position.z, 2);

                        LoadMap.instance.cameras[i].gameObject.transform.position = new Vector2(
                            arrHexCreados[arrHexTeleport.ladosArray[i, 0]].transform.position.x,
                            arrHexCreados[arrHexTeleport.ladosArray[i, 0]].transform.position.y
                            );
                    }


                    for (int i = activeHex.Count - 1; i >= 0; i--)
                    {
                        bool off = true;

                        for (int l = 0; l < 6; l++)
                        {
                            if (arrHexTeleport.id == activeHex[i].id || arrHexTeleport.ladosArray[l, 0] == HexagonsManager.activeHex[i].id)
                            {
                                off = false;
                                break;
                            }
                        }

                        if (off)
                        {
                            arrHexCreados[activeHex[i].id].gameObject.SetActive(false);//desactivo todo el resto de hexagonos, para que no consuman cpu
                            activeHex.RemoveAt(i);
                        }
                    }
                }
            }
        }
    }
}
