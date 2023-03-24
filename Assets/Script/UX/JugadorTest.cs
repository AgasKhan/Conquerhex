using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JugadorTest : MonoBehaviour
{
    [SerializeField] float speed = 5;

    private void Start()
    {
        VirtualControllers.movement.pressed += MyController_sticklocko;

        VirtualControllers.movement.up += MyController_up;
    }

    private void MyController_up(Vector2 arg1, float arg2)
    {
        if (arg2 < 0.5f)
            transform.position += arg1.Vec2to3(0);
    }

    private void MyController_sticklocko(Vector2 obj, float n)
    {
        transform.position += new Vector3(obj.x, obj.y, 0) * speed * Time.deltaTime;
    }

}
