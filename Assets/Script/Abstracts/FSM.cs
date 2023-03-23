using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSM<T, Context> : ISwitchState<T> where T : FSM<T,Context>
{

    public Context context;

    IState<T> currentState;

    public IState<T> CurrentState 
    {   
        get => currentState; 
        set => SwitchState(value); 
    }

    T FSMConvertToChild()
    {
        return (T)this;
    }

    void SwitchState(IState<T> state)
    {
        if (state == currentState || state == null)
            return;

        currentState.OnExitState(FSMConvertToChild());
        Init(state);
    }

    public void UpdateState()
    {
        currentState.OnStayState(FSMConvertToChild());
    }

    protected void Init(IState<T> first)
    {
        currentState = first;
        currentState.OnEnterState(FSMConvertToChild());
    }

    protected FSM(Context reference)
    {
        this.context = reference;
    }
}


