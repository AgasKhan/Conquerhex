using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarlitoEntity : MonoBehaviour, IGetEntity
{
    [SerializeField]
    Entity entity;

    public bool visible { get => entity.visible; set => entity.visible = value; }

    public Entity GetEntity()
    {
        if (visible)
            return entity;
        else
            return null;
    }

    // Start is called before the first frame update
    void Awake()
    {
        entity = GetComponentInParent<Entity>();
        tag = transform.parent.tag;
    }
}
