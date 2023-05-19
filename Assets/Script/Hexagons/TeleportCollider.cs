using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCollider : MonoBehaviour
{

    [SerializeField]
    Teleport teleport;

    //hacia donde se teletransporta, el primer indice es el lado del propio hexagono, y el segundo es el destino (0 para la id y 1 para el lado)
    Teleport[] ladosArray => teleport.ladosArray;

    //pareja de coordenadas
    float[,] ladosPuntos => teleport.ladosPuntos;

    float velocityTransfer => teleport.velocityTransfer;

    Vector2 anguloDefecto => teleport.anguloDefecto;

    Teleport[] arrHexCreados => HexagonsManager.arrHexCreados;

    Pictionarys<int, Teleport> activeHex => HexagonsManager.activeHex;

    private void Awake()
    {
        teleport = GetComponentInParent<Teleport>();
    }

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

            //le doy un empujon para que no se quede en el medio
            fisicaOther.Acelerator(velocityTransfer * fisicaOther.direction);

            Vector2 vectorVelocidad = fisicaOther.vectorVelocity;

            angle = 360 - Utilitys.DifAngulosVectores(anguloDefecto, vectorSalida);

            lado = Mathf.FloorToInt(angle / 60);

            if (lado > 5)
                lado = 5;

            Teleport arrHexTeleport = ladosArray[lado];//accedo al script del array al que me quiero teletransportar

            float anguloVelocidad = Utilitys.DifAngulosVectores(new Vector2(Mathf.Cos((lado * -60) * Mathf.Deg2Rad), Mathf.Sin((lado * -60) * Mathf.Deg2Rad)), vectorVelocidad);

            //aplico una velocidad al objeto que esta cerca del portal

            //Para detectar a donde va el objeto
            Debug.DrawRay(this.gameObject.transform.position, (other.gameObject.transform.position - this.gameObject.transform.position).normalized, Color.green, 30);

            Debug.DrawRay(this.gameObject.transform.position, anguloDefecto, Color.red, 30);

            if
            (anguloVelocidad < 180 && anguloVelocidad > 0)
            {
                fisicaOther.Teleport(arrHexTeleport);

                difEspejada[0] = ladosPuntos[lado, 0] - other.transform.position.x;
                difEspejada[1] = ladosPuntos[lado, 1] - other.transform.position.y;

                /*
                 DebugPrint.Log("Efectuado teletransporte");

                 DebugPrint.Log("Salio del hexagono " + other.name + " con el angulo: "+angle + " y el lado: " + lado);

                 DebugPrint.Log("transportando al ID: " + (ladosArray[lado,0]) + " Y al lado " + (ladosArray[lado, 1]));
                 */
                

                other.gameObject.transform.position =
                    new Vector3(
                        //arrHexTeleport.ladosPuntos[ladosArray[lado, 1], 0] - difEspejada[0],
                        //arrHexTeleport.ladosPuntos[ladosArray[lado, 1], 1] - difEspejada[1],
                        arrHexTeleport.ladosPuntos[HexagonsManager.LadoOpuesto(lado), 0] - difEspejada[0],
                        arrHexTeleport.ladosPuntos[HexagonsManager.LadoOpuesto(lado), 1] - difEspejada[1],
                        other.gameObject.transform.position.z);

                other.gameObject.transform.SetParent(arrHexTeleport.gameObject.transform);

                if(!arrHexTeleport.gameObject.activeSelf)
                {
                    arrHexTeleport.gameObject.SetActive(true);
                    arrHexTeleport.gameObject.SetActive(false);
                }
                

                if (fisicaOther.carlitos != null)
                    for (int i = 0; i < fisicaOther.carlitos.Length; i++)
                    {
                        fisicaOther.carlitos[i].transform.position = HexagonsManager.AbsSidePosHex(arrHexTeleport.ladosArray[i].transform.position, HexagonsManager.LadoOpuesto(i), fisicaOther.carlitos[i].transform.position.z, 2) + (other.gameObject.transform.position - arrHexTeleport.transform.position);
                    }


                if (other.CompareTag("Player"))
                {
                    MainCamera.instance.gameObject.transform.position = new Vector3(
                        arrHexTeleport.ladosPuntos[HexagonsManager.LadoOpuesto(lado), 0] - (ladosPuntos[lado, 0] - Camera.main.gameObject.transform.position.x),
                        arrHexTeleport.ladosPuntos[HexagonsManager.LadoOpuesto(lado), 1] - (ladosPuntos[lado, 1] - Camera.main.gameObject.transform.position.y),
                        Camera.main.gameObject.transform.position.z);

                    for (int i = 0; i < LoadMap.instance.renders.Length; i++)
                    {
                        arrHexTeleport.ladosArray[i].gameObject.SetActive(true);
                        activeHex.Add(arrHexTeleport.ladosArray[i].id, arrHexTeleport.ladosArray[i]);

                        LoadMap.instance.renders[i].transform.position = HexagonsManager.AbsSidePosHex(arrHexTeleport.transform.position, i, LoadMap.instance.renders[i].transform.position.z, 2);

                        LoadMap.instance.cameras[i].gameObject.transform.position = new Vector2(
                            arrHexTeleport.ladosArray[i].transform.position.x,
                            arrHexTeleport.ladosArray[i].transform.position.y
                            );
                    }


                    for (int i = activeHex.Count - 1; i >= 0; i--)
                    {
                        bool off = true;

                        for (int l = 0; l < 6; l++)
                        {
                            if (arrHexTeleport.id == activeHex[i].id || arrHexTeleport.ladosArray[l].id == HexagonsManager.activeHex[i].id)
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
            }
        }
    }
}
