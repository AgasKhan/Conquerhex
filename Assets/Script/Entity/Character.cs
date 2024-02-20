using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Entity, ISwitchState<Character>
{
    public InventoryEntityComponent inventory;
    public AttackEntityComponent attack;
    public MoveEntityComponent move;



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

        move = GetInContainer<MoveEntityComponent>();
        attack = GetInContainer<AttackEntityComponent>();
        inventory = GetInContainer<InventoryEntityComponent>();
    }

    void IAUpdate()
    {
        _ia.OnStayState(this);
    }
}


