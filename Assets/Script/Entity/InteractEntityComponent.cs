using ComponentsAndContainers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InteractEntityComponent : ComponentOfContainer<Entity>
{
    public bool interactuable = true;
    public Sprite Image;
    public Action<Character> Interact;
    
    public override void OnEnterState(Entity param)
    {
        Image = param.flyweight.image;
    }

    public override void OnExitState(Entity param)
    {
        throw new System.NotImplementedException();
    }

    public override void OnStayState(Entity param)
    {
        throw new System.NotImplementedException();
    }
}
