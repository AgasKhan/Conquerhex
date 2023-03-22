using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuerteDestruccion : MonoBehaviour
{
    Vida health;
    Rigidbody2D rgb2d;
    //Timers loco = new Timers(3);

    void OnTriggerStay2D(Collider2D objeto)
    {

        health = objeto.GetComponent<Vida>();
        rgb2d = objeto.GetComponent<Rigidbody2D>();

        if (health!=null)
            health.RestarHp(5,3);

        if(rgb2d!=null)
            rgb2d.velocity += Time.deltaTime * 50 * rgb2d.velocity.normalized;

        //this.GetComponent<SpriteRenderer>().color = new Color(Random.Range(1, 11) / 10f, Random.Range(1, 11) / 10f, Random.Range(1, 11) / 10f, 1f);
    }

   
}