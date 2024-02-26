using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveEntityComponent))]
[RequireComponent(typeof(AttackEntityComponent))]
public class Character : Entity, ISwitchState<Character>
{
    //public new BodyBase flyweight;

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


    public void Attack()
    {

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

        move = GetInContainer<MoveEntityComponent>();
        attack = GetInContainer<AttackEntityComponent>();
        inventory = GetInContainer<InventoryEntityComponent>();
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

