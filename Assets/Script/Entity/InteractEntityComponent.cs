using ComponentsAndContainers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InteractEntityComponent : ComponentOfContainer<Entity>
{
    public bool interactuable = true;
    public Sprite Image;

    public Pictionarys<Type, InteractAction> interact => _interact;

    Pictionarys <Type, InteractAction> _interact = new Pictionarys<Type, InteractAction>();

    public GenericSubMenu mySubMenu;

    public void ShowMenu()
    {
        //container.flyweight

        mySubMenu.Create();
    }

    public virtual T Interact<T>() where T : InteractAction
    {
        if (!interactuable)
            return null;

        return (T)_interact[typeof(T)];
    }

    public override void OnEnterState(Entity param)
    {
        Image = param.flyweight.image;

        mySubMenu.Init();

        foreach (var item in GetComponents<InteractAction>())
        {
            _interact.Add(item.GetType(), item);
        }

        mySubMenu = new GenericSubMenu(this);

        //Interact<CraftingAction>().Activate(param.);

        //interact = param.flyweight.GetFlyWeight<InteractBase>().interact;
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
