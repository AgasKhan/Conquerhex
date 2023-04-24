using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : DinamicEntityWork//, ISwitchState<Character>
{
    // Start is called before the first frame update
    [SerializeField]
    public Ability prin;
    [SerializeField]
    public Ability sec;
    [SerializeField]
    public Ability ter;


    /*
    public IState<Character> CurrentState 
    { 
        get => _ia;
        set
        {
            _ia.OnExitState(this);
            _ia = value;
            _ia.OnEnterState(this);
        } 
    }

    [SerializeReference]
    IState<Character> _ia;
    */

    public IAIO CurrentState
    {
        get => _ia;
        set
        {
            _ia.OnExitState(this);
            _ia = value;
            _ia.OnEnterState(this);
        }
    }

    [SerializeReference]
    IAIO _ia;

    protected override void Config()
    {
        base.Config();
        MyAwakes += MyAwake;
    }

    void MyAwake()
    {
        prin.Init(this);
        sec.Init(this);
        ter.Init(this);

        _ia?.OnEnterState(this);
    }

}

    
