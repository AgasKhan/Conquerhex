using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : DynamicEntity, ISwitchState<Character>
{
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

    public override float weightCapacity => ((BodyBase)flyweight).weightCapacity;


    protected override void Config()
    {
        base.Config();
        MyAwakes += MyAwake;
        MyStarts += MyStart;
    }

    void MyAwake()
    {
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

    void IAUpdate()
    {
        _ia.OnStayState(this);
    }
}


