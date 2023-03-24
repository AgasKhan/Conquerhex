using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JugadorTest : MonoBehaviour
{
    [SerializeField] Controlador myController = null;

    [SerializeField] float speed = 5;

    private void Update()
    {
        Vector3 myDir = myController.MoveDir();
        transform.position += new Vector3(myDir.x, myDir.y, 0) * speed * Time.deltaTime;
    }
}
