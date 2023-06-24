using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCollider : MonoBehaviour
{

    [SerializeField]
    Hexagone teleport;

    //hacia donde se teletransporta, el primer indice es el lado del propio hexagono, y el segundo es el destino (0 para la id y 1 para el lado)
    Hexagone[] ladosArray => teleport.ladosArray;

    //pareja de coordenadas
    float[,] ladosPuntos => teleport.ladosPuntos;

    float velocityTransfer => teleport.velocityTransfer;

    Vector2 anguloDefecto => teleport.anguloDefecto;

    Pictionarys<int, Hexagone> activeHex => HexagonsManager.activeHex;

    private void Awake()
    {
        teleport = GetComponentInParent<Hexagone>();
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
            fisicaOther.Acelerator(velocityTransfer * fisicaOther.direction, velocityTransfer);

            Vector2 vectorVelocidad = fisicaOther.vectorVelocity;

            angle = 360 - Utilitys.DifAngulosVectores(anguloDefecto, vectorSalida);

            lado = Mathf.FloorToInt(angle / 60);

            if (lado > 5)
                lado = 5;

            Hexagone arrHexTeleport = ladosArray[lado];//accedo al script del array al que me quiero teletransportar

            float anguloVelocidad = Utilitys.DifAngulosVectores(new Vector2(Mathf.Cos((lado * -60) * Mathf.Deg2Rad), Mathf.Sin((lado * -60) * Mathf.Deg2Rad)), vectorVelocidad);

            //aplico una velocidad al objeto que esta cerca del portal

            //Para detectar a donde va el objeto
            Debug.DrawRay(this.gameObject.transform.position, (other.gameObject.transform.position - this.gameObject.transform.position).normalized, Color.green, 30);

            Debug.DrawRay(this.gameObject.transform.position, anguloDefecto, Color.red, 30);

            if
            (anguloVelocidad < 180 && anguloVelocidad > 0)
            {
                difEspejada[0] = ladosPuntos[lado, 0] - other.transform.position.x;
                difEspejada[1] = ladosPuntos[lado, 1] - other.transform.position.y;

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
                
                

                fisicaOther.Teleport(arrHexTeleport, lado);
            }
        }


    }

    
    
}
