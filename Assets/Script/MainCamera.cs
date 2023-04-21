using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : SingletonMono<MainCamera>
{
    public Vector2Quad obj;

    private void Start()
    {
        obj.prev = obj.tr.position.Vect3To2();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        transform.position  = obj.tr.position;
    }

 
}

[System.Serializable]
public struct Vector2Quad
{
    public Transform tr;
    public Vector2 prev;
    public Vector2 velocity;

    public void LoadVelocity()
    {
        velocity = (tr.position.Vect3To2() * 1000 - prev * 1000) / (Time.deltaTime*1000);
        prev = tr.position.Vect3To2();
    }
}
