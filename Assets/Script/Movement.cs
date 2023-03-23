using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour
{
    public Rigidbody2D rb2d;
    SpriteRenderer rend;
    public float restaLineal;
    
    public void SetVectorDT(float magnitud, float angulo)
    {
        //para setear los vectores de velocidad
        
        Vector2 aux = new Vector2( Mathf.Cos(angulo) , Mathf.Sin(angulo));
        
        rb2d.velocity += aux * magnitud;
    }
    public void SetVectorDT(float magnitud, Vector2 vector)
    {
        //para setear los vectores de velocidad

        rb2d.velocity += vector.normalized * magnitud;
    }

    // Start is called before the first frame update
    void Awake()//esto es de garca
    {
        rb2d = GetComponent<Rigidbody2D>();
        rend = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        rb2d.velocity -= rb2d.velocity*(restaLineal * Time.deltaTime);

        if(rb2d.velocity.sqrMagnitude<0.01f)
        {
            rb2d.velocity = Vector2.zero;
        }

        if(rend!=null)
            rend.sortingOrder = Mathf.RoundToInt(transform.position.y*-100);

        //Debug.DrawRay(this.transform.position, rb2d.velocity, Color.blue);    
    }
}
