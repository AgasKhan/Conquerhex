using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : SingletonMono<MainCamera>
{
    public Transform obj;

    public bool perspective;

    [SerializeField]
    Vector3 rotationPerspective;

    [SerializeField]
    Vector3 vectorPerspective;

    private void OnEnable()
    {
        Camera.main.orthographic = !perspective;

        if(!perspective)
        {
            transform.rotation = Quaternion.identity;
            transform.GetChild(0).localPosition = Vector3.zero;
        }
        else
        {
            transform.rotation = Quaternion.Euler(rotationPerspective);
            transform.GetChild(0).localPosition = vectorPerspective;
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (obj == null)
            return;
        transform.position  = obj.position.Vect3To2().Vec2to3(transform.position.z);
    }
}
