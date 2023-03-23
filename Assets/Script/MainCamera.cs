using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
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

        obj.LoadVelocity();
        me.LoadVelocity();

        Vector2 direction = (obj.tr.position.Vect3To2() + obj.velocity* multiplyFront) - (me.tr.position.Vect3To2());

        if (direction.sqrMagnitude > 1)
            direction.Normalize();


        me.tr.position += (direction * velocity*Time.deltaTime).Vec2to3(me.tr.position.z);



        me.UpdatePrev();
        obj.UpdatePrev();

    }


}

public class IaMovement
{

}

[System.Serializable]
public struct Vector2Quad
{
    public Transform tr;
    public Vector2 actual;
    public Vector2 prev;
    public Vector2 velocity;

    public void LoadVelocity()
    {
        velocity = (tr.position.Vect3To2() - prev) / Time.deltaTime;
    }

    public void UpdatePrev()
    {
        prev = tr.position.Vect3To2();
    }
}
