using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveEntityComponent))]
[RequireComponent(typeof(CasterEntityComponent))]
public class Character : Entity, ISwitchState<Character, IState<Character>>
{
    public InventoryEntityComponent inventory;
    public CasterEntityComponent caster;
    public MoveEntityComponent move;

    IState<Character> _ia;

    FSMCharacter fsmCharacter;

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

    public EventControllerMediator attackEventMediator => caster.attack;

    public EventControllerMediator abilityEventMediator => caster.ability;

    public void Attack(int i)
    {
        //fsmCharacter.CurrentState = attack;
        caster.Attack(i);
    }

    public void Ability(int i)
    {
        //fsmCharacter.CurrentState = attack;
        caster.Ability(i);
    }    

    protected override void Config()
    {
        base.Config();
        MyAwakes += MyAwake;
        MyStarts += MyStart;
        //MyUpdates += fsmCharacter.UpdateState;
    }

    void MyAwake()
    {
        _ia = GetComponent<IState<Character>>();

        move = GetInContainer<MoveEntityComponent>();
        caster = GetInContainer<CasterEntityComponent>();
        inventory = GetInContainer<InventoryEntityComponent>();

        fsmCharacter = new FSMCharacter(this);
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

public class FSMCharacter : FSM<FSMCharacter, Entity>
{
    public FSMCharacter(Entity reference) : base(reference)
    {
        //Init(context.GetInContainer<MoveEntityComponent>());
    }
}