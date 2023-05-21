using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : SingletonMono<MainCamera>
{
    public Transform obj;

    // Update is called once per frame
    private void LateUpdate()
    {
        if (obj == null)
            return;
        transform.position  = obj.position.Vect3To2().Vec2to3(transform.position.z);
    }
}
