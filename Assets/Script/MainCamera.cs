using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : SingletonMono<MainCamera>
{
    public Vector2Quad obj;

    public Vector2Quad me;

    public float velocity;

    public float multiplyFront;

    private void Start()
    {
        me.tr = transform;

        obj.prev = obj.tr.position.Vect3To2();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (obj.tr == null)
            return;

        me.LoadVelocity();

        Vector2 direction = (obj.tr.position.Vect3To2() + obj.velocity* multiplyFront) - (me.tr.position.Vect3To2());

        //direction += me.velocity;

        if (direction.sqrMagnitude > 1)
            direction.Normalize();

        me.tr.position += (direction * velocity*Time.deltaTime).Vec2to3(me.tr.position.z);
    }

    private void FixedUpdate()
    {
        obj.LoadVelocity();
    }
}

public class IaMovement
{
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
