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
    Vector3[] ladosPuntos => teleport.ladosPuntos;

    float velocityTransfer => teleport.velocityTransfer;

    Vector2 difEspejada;//voy a guardar la diferencia para poder espejarlo de forma correcta

    int lado;

    Vector3 vectorSalida;

    private void Awake()
    {
        teleport = GetComponentInParent<Hexagone>();
    }

    void OnTriggerStay(Collider other)
    {
        MoveAbstract fisicaOther = other.GetComponent<MoveAbstract>();

        if (fisicaOther == null)
            return;

        vectorSalida = (other.transform.position - transform.position).normalized;

        //le doy un empujon para que no se quede en el medio
        fisicaOther.Acelerator(fisicaOther.direction, velocityTransfer, velocityTransfer);

        lado = HexagonsManager.CalcEdge(vectorSalida.Vect3To2XZ());

        Hexagone arrHexTeleport = ladosArray[lado];//accedo al script del array al que me quiero teletransportar

        float anguloVelocidad = Utilitys.DifAngulosVectores(new Vector2(Mathf.Cos((lado * -60) * Mathf.Deg2Rad), Mathf.Sin((lado * -60) * Mathf.Deg2Rad)), fisicaOther.VectorVelocity.Vect3To2XZ());

        //aplico una velocidad al objeto que esta cerca del portal

        if
        (anguloVelocidad < 180 && anguloVelocidad > 0)
        {
            difEspejada[0] = ladosPuntos[lado].x - other.transform.position.x;
            difEspejada[1] = ladosPuntos[lado].z - other.transform.position.z;

            other.gameObject.transform.position =
                new Vector3(
                    arrHexTeleport.ladosPuntos[HexagonsManager.LadoOpuesto(lado)].x - difEspejada[0],
                    other.transform.position.y,
                    arrHexTeleport.ladosPuntos[HexagonsManager.LadoOpuesto(lado)].z - difEspejada[1]);

            if(other.CompareTag("Player"))                
                teleport.SetPortalColor(Color.cyan);

            fisicaOther.Teleport(arrHexTeleport, lado);

            /*
            if (!arrHexTeleport.gameObject.activeSelf)
            {
                arrHexTeleport.gameObject.SetActive(true);
                arrHexTeleport.gameObject.SetActive(false);
            }
            */
        }
    }
}
