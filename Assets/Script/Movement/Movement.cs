using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MyScripts
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

    public void SetVectorDT(Vector2 vector)
    {
        //para setear los vectores de velocidad

        rb2d.velocity += vector;
    }

    protected override void Config()
    {
        MyAwakes += MyAwake;
        MyFixedUpdates += MyFixedUpdate;
    }

    // Start is called before the first frame update
    void MyAwake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rend = GetComponentInChildren<SpriteRenderer>();

        if (rend != null)
            MyUpdates += MyUpdateRender;
    }

    // Update is called once per frame
    void MyFixedUpdate()
    {
        rb2d.velocity -= rb2d.velocity*(restaLineal * Time.deltaTime);

        if(rb2d.velocity.sqrMagnitude<0.01f)
        {
            rb2d.velocity = Vector2.zero;
        }            

        //Debug.DrawRay(this.transform.position, rb2d.velocity, Color.blue);    
    }

    void MyUpdateRender()
    {
        rend.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);
    }


}
