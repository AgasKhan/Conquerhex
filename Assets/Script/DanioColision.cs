using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanioColision : MonoBehaviour
{
    public float danio;

    public float enfriamiento;

    public string owner;

    public float multiplicadorPared;

    Rigidbody2D rgb2d;
    private void OnTriggerStay2D(Collider2D collision)
    {
        Bala(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Bala(collision.collider);
    }

    void Bala(Collider2D collision)
    {
        if (collision.name != owner && owner != null)
        {
            Vida health = collision.GetComponent<Vida>();
            if (health != null)
            {
                float aux = health.hp;
                health.RestarHp(danio, enfriamiento);
                if (health.hp == aux)
                {
                    DebugPrint.Log("daño evadido");
                }
            }
            
            if (collision.CompareTag("Player") || collision.CompareTag("Enemigo"))
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void Start()
    {

        if (GetComponent<Rigidbody2D>())
            rgb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(rgb2d!=null)
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Acos(rgb2d.velocity.normalized.x) * Mathf.Sign(rgb2d.velocity.y) * Mathf.Rad2Deg - 90);
        
    }
}