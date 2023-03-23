using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    
    public int id;

    //hacia donde se teletransporta, el primer indice es el lado del propio hexagono, y el segundo es el destino (0 para la id y 1 para el lado)
    public int[,] ladosArray;

    //pareja de coordenadas
    public float[,] ladosPuntos;

    public float velocityTransfer;

    void OnTriggerStay2D(Collider2D other)
    {
        Rigidbody2D fisicaOther = other.GetComponent<Rigidbody2D>();
        
        if (fisicaOther != null)
        {
            //StartCoroutine(FixColision(other));

            

            float angle;
            float[] difEspejada = new float[2]; //voy a guardar la diferencia para poder espejarlo de forma correcta
            int lado;
            //PrintF log = new PrintF();

            //vector que da el angulo por defecto para calcular el hexagono (esta a 120 grados)
            Vector2 anguloDefecto = new Vector2(Mathf.Cos((2f / 3f) * Mathf.PI), Mathf.Sin((2f / 3f) * Mathf.PI));

            Vector2 vectorSalida = (other.gameObject.transform.position - this.gameObject.transform.position).normalized;

            fisicaOther.velocity += Time.deltaTime * velocityTransfer * fisicaOther.velocity.normalized;
            
            Vector2 vectorVelocidad = fisicaOther.velocity;

            angle = 360 - Euler.DifAngulosVectores(anguloDefecto, vectorSalida);

            lado = Mathf.FloorToInt(angle / 60);

            Teleport arrHexTeleport = LoadMap.arrHexCreados[ladosArray[lado, 0]].GetComponent<Teleport>();//accedo al script del array al que me quiero teletransportar

            float anguloVelocidad = Euler.DifAngulosVectores(new Vector2(Mathf.Cos((lado * -60) * Mathf.Deg2Rad), Mathf.Sin((lado * -60) * Mathf.Deg2Rad)), vectorVelocidad);

            //aplico una velocidad al objeto que esta cerca del portal

            //Para detectar a donde va el objeto
            Debug.DrawRay(this.gameObject.transform.position, (other.gameObject.transform.position - this.gameObject.transform.position).normalized, Color.green, 30);

            Debug.DrawRay(this.gameObject.transform.position, anguloDefecto, Color.red, 30);

            if
            (
                anguloVelocidad < 180
                &&
                anguloVelocidad > 0
            )
            {

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


                if (other.CompareTag("Player"))
                {
                    //AudioManager.instance.Play("transporte");
                    for (int i = 0; i < LoadMap.arrHexCreados.Length; i++)
                    {
                        if (LoadMap.arrHexCreados[i].activeSelf && !(
                            arrHexTeleport.id == i ||
                            arrHexTeleport.ladosArray[0, 0] == i ||
                            arrHexTeleport.ladosArray[1, 0] == i ||
                            arrHexTeleport.ladosArray[2, 0] == i ||
                            arrHexTeleport.ladosArray[3, 0] == i ||
                            arrHexTeleport.ladosArray[4, 0] == i ||
                            arrHexTeleport.ladosArray[5, 0] == i))
                        {
                            LoadMap.arrHexCreados[i].SetActive(false);//desactivo todo el resto de hexagonos, para que no consuman cpu
                            DebugPrint.Log("Desactivando hexagono id: " + i);
                        }


                    }

                    //arrHexTeleport.gameObject.SetActive(true);

                    Camera.main.gameObject.transform.position = new Vector3(
                        arrHexTeleport.ladosPuntos[ladosArray[lado, 1], 0] - (ladosPuntos[lado, 0] - Camera.main.gameObject.transform.position.x),
                        arrHexTeleport.ladosPuntos[ladosArray[lado, 1], 1] - (ladosPuntos[lado, 1] - Camera.main.gameObject.transform.position.y),
                        Camera.main.gameObject.transform.position.z);

                    for (int i = 0; i < LoadMap.instance.renders.Length; i++)
                    {
                        LoadMap.arrHexCreados[arrHexTeleport.ladosArray[i, 0]].SetActive(true);

                        LoadMap.instance.renders[i].transform.position = LoadMap.AbsSidePosHex(LoadMap.arrHexCreados[ladosArray[lado, 0]].transform.position, i, LoadMap.instance.renders[i].transform.position.z, 2);

                        LoadMap.instance.cameras[i].gameObject.transform.position = new Vector2(
                            LoadMap.arrHexCreados[arrHexTeleport.ladosArray[i, 0]].transform.position.x,
                            LoadMap.arrHexCreados[arrHexTeleport.ladosArray[i, 0]].transform.position.y
                            );

                        LoadMap.instance.carlitos[i].transform.position = LoadMap.AbsSidePosHex(LoadMap.arrHexCreados[arrHexTeleport.ladosArray[i, 0]].transform.position, ((i - 3) >= 0) ? (i - 3) : (i + 3), LoadMap.instance.carlitos[i].transform.position.z, 2) + (other.gameObject.transform.position - LoadMap.arrHexCreados[ladosArray[lado, 0]].transform.position);
                    }
                }
                //printeo la info
                //log.Print();
            }
        }
    }

 /*
    IEnumerator FixColision(Collider2D fix)
    {
       
        Collider2D[] arr = fix.gameObject.GetComponents<Collider2D>();
        foreach (Collider2D item in arr)
            item.enabled = false;
        yield return null;
        yield return null;
        foreach (Collider2D item in arr)
            item.enabled = true;
    }*/
}
