using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarlitoEntity : MonoBehaviour, IGetEntity
{
    [SerializeField]
    Entity entity;

    public Entity GetEntity()
    {
        return entity;
    }

    // Start is called before the first frame update
    void Awake()
    {
        entity = GetComponentInParent<Entity>();
        tag = transform.parent.tag;
    }
}
