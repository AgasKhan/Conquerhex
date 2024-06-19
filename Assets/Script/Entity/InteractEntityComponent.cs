using ComponentsAndContainers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class InteractEntityComponent : ComponentOfContainer<Entity>, ISaveObject
{
    public bool interactuable = true;
    [HideInInspector]
    public Sprite Image;

    public LogicActive<(InteractEntityComponent, Character)> interactAction;

    public Pictionarys<Type, InteractAction> interact => _interact;

    Pictionarys <Type, InteractAction> _interact = new Pictionarys<Type, InteractAction>();

    [HideInInspector]
    public GenericSubMenu genericMenu;
    [HideInInspector]
    public Character lastCharInteract;

    public event Action OnInteract
    {
        add
        {
            _onInteract += value;
        }
        remove
        {
            _onInteract -= value;
        }
    }
    private event Action _onInteract;

    public virtual void Interact(Character character)
    {
        if (!interactuable)
            return;

        lastCharInteract = character;

        _onInteract?.Invoke();

        interactAction.Activate((this, character));
    }

    public virtual T Interact<T>() where T : InteractAction
    {
        if (!interactuable)
            return null;

        _onInteract?.Invoke();
        return (T)_interact[typeof(T)];
    }

    public override void OnEnterState(Entity param)
    {
        Image = param.flyweight.image;

        foreach (var item in GetComponents<InteractAction>())
        {
            _interact.Add(item.GetType(), item);
        }

        genericMenu = new GenericSubMenu(this);

        genericMenu.SetCreateAct((menu) =>{genericMenu.InteractAction();});
    }

    public override void OnExitState(Entity param)
    {

    }

    public override void OnStayState(Entity param)
    {

    }

    public void ChangeInteract(bool newValue)
    {
        interactuable = newValue;
    }

    public string Save()
    {
        return JsonUtility.ToJson(interactuable);
    }

    public void Load(string str)
    {
        interactuable = JsonUtility.FromJson<bool>(str);
    }
}
