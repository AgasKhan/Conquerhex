using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyEnemy : MonoBehaviour, IPrototype<MyEnemy>
{
    Material mat;

    void Awake()
    {
        mat = GetComponent<Renderer>().material;
    }

    public MyEnemy SetColor(Color color)
    {
        mat.color =color;

        return this;
    }

    public MyEnemy SetPos(Vector3 vector)
    {
        transform.position = vector;

        return this;
    }

    public MyEnemy SetScale(Vector3 vector)
    {
        transform.localScale = vector;

        return this;
    }

    public MyEnemy Clone()
    {
        return Instantiate(this);
    }
}

public interface IPrototype<T>
{
    T Clone();
}
