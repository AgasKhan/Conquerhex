using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPopup : MonoBehaviour
{
    private float destroyTimer = 1f;
    
    void Update()
    {
        float moveSpeed = 2f;

        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        destroyTimer -= Time.deltaTime;
        if(destroyTimer<=0)
        {
                Destroy(gameObject);
        }
    }
}
