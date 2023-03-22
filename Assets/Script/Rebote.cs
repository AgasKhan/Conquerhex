using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rebote : MonoBehaviour
{
    public float distancia;
    public bool parry;
    public Collider2D col;

    private void Start()
    {
        parry = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(parry)
        {
            Rigidbody2D rgb2 = collision.gameObject.GetComponent<Rigidbody2D>();

            Vector2 reflejo;

            //Busco a todos los enemigos posibles a los q rebotarle la bala
            GameObject[] enemigos = GameObject.FindGameObjectsWithTag("Enemigo");

            //el enemigo al que le voy a rebotar la bala, si posee la menor distancia
            GameObject enemigo=null;

            if(enemigos.Length>0)
            {
                float dist = distancia;
                for (int i = 0; i < enemigos.Length; i++)
                {
                    float angulo=0;
                    if (collision.gameObject.GetComponent<Rigidbody2D>() != null)
                    {
                        angulo = Euler.DifAngulosVectores(collision.gameObject.GetComponent<Rigidbody2D>().velocity, enemigos[i].transform.position - collision.transform.position);
                    }

                    if ((enemigos[i].transform.position-transform.position).sqrMagnitude < dist && (angulo>90 && angulo<270))
                    {
                        dist = (enemigos[i].transform.position - transform.position).sqrMagnitude;
                        enemigo = enemigos[i];
                        collision.gameObject.GetComponent<DanioColision>().owner = gameObject.name;
                        //DebugPrint.Log("Delta angulo por rebote: " + angulo);
                    }
                }
                //DebugPrint.Log(enemigo.name);
            }
            if (enemigo != null)
                reflejo = new Vector2(enemigo.transform.position.x - collision.gameObject.transform.position.x, (enemigo.transform.position.y + 0.3f) - collision.gameObject.transform.position.y).normalized;
            else
                reflejo = rgb2.velocity.normalized*-1;

            

            rgb2.velocity = reflejo * rgb2.velocity.magnitude;

            AudioManager.instance.Play("parryGood");
        }
       
    }
}
