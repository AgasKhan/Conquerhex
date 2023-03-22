using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuerteDistancia : MonoBehaviour
{
    // Start is called before the first frame update
    public float muerte;

    Rigidbody2D rb;
    // Update is called once per frame
    void Awake()
    {
        Destroy(this.gameObject, muerte);
        rb= GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        if(rb.velocity.sqrMagnitude<1)
        {
            Destroy(this.gameObject);
        }       
    }
}
