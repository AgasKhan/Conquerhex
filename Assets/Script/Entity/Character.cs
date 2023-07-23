using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : DynamicEntity, ISwitchState<Character>
{
    [SerializeField]
    Detect<RecolectableItem> areaFarming;

    [SerializeField]
    IState<Character> _ia;

    public IState<Character> CurrentState
    {
        get => _ia;
        set
        {
            if (_ia == null && value != null)
            {
                MyUpdates += IAUpdate;
            }
            else if (value == null)
            {
                MyUpdates -= IAUpdate;
            }

            _ia?.OnExitState(this);
            _ia = value;
            _ia?.OnEnterState(this);
        }
    }

    protected override void Config()
    {
        base.Config();
        MyAwakes += MyAwake;
        MyUpdates += MyUpdate;
        MyStarts += MyStart;
    }

    void MyAwake()
    {
        areaFarming.radius = ((BodyBase)flyweight).areaFarming;

        weightCapacity = ((BodyBase)flyweight).weightCapacity;

        _ia = GetComponent<IState<Character>>();
    }

    void MyStart()
    {
        if (_ia != null)
        {
            _ia.OnEnterState(this);
            MyUpdates += IAUpdate;
        }
    }

    void MyUpdate()
    {
        var recolectables = areaFarming.Area(transform.position, (algo) => { return true; });
        foreach (var recolectable in recolectables)
        {
            //if (currentWeight + recolectable.weight <= weightCapacity)
            {
                recolectable.Recolect(this);
            }
        }
    }

    void IAUpdate()
    {
        _ia.OnStayState(this);
    }
}


