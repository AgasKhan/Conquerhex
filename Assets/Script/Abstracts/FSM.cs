using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FSMAutomaticEnd<TContext> : FSMParent<FSMAutomaticEnd<TContext>, TContext, IStateWithEnd<FSMAutomaticEnd<TContext>>>
{
    public bool end => CurrentState.end;

    public void EnterState(IStateWithEnd<FSMAutomaticEnd<TContext>> stateWithEnd)
    {
        Init(stateWithEnd);
    }

    public override void UpdateState()
    {
        if(!CurrentState.end)
            base.UpdateState();

        if (CurrentState.end)
        {
            ExitState();
        }                  
    }
}

public abstract class FSM<TChild, TContext> : FSMSerialize<TChild, TContext> where TChild : FSM<TChild, TContext>
{ 
    protected FSM(TContext reference)
    {
        Init(reference);
    }
}


[System.Serializable]
public abstract class FSMSerialize<TChild, TContext> : FSMParent<TChild, TContext, IState<TChild>> where TChild : FSMSerialize<TChild, TContext>
{
}

public abstract class FSMParent<TChild, TContext, TState> : ISwitchState<TChild, TState> where TChild : FSMParent<TChild, TContext, TState> where TState : IState<TChild>
{
    [HideInInspector]
    public TContext context;

    TState currentState;

    public event System.Action<TState, TState> onChange; //del que vengo al cual voy

    public event System.Action<TChild> onEnter;

    public event System.Action<TChild> onExit;

    public TState CurrentState
    {
        get => currentState;
        set => SwitchState(value);
    }

    protected TChild FSMConvertToChild()
    {
        return (TChild)this;
    }

    void SwitchState(TState state)
    {
        if (state.Equals(currentState) || state == null)
            return;

        ExitState();
        onChange?.Invoke(currentState, state);
        Init(state);
    }

    public virtual void UpdateState()
    {
        currentState.OnStayState(FSMConvertToChild());
    }

    /// <summary>
    /// Obligatorio para setear el estado inicial, tambien dispara el evento de entrada del estado
    /// </summary>
    /// <param name="first"></param>
    protected void Init(TState first)
    {
        currentState = first;
        onEnter?.Invoke(FSMConvertToChild());
        currentState.OnEnterState(FSMConvertToChild());
    }

    /// <summary>
    /// Se encarga de ejecutar la salida de un estado de forma manual, tambien dispara el evento de salida del estado
    /// </summary>
    protected void ExitState()
    {
        currentState.OnExitState(FSMConvertToChild());
        onExit?.Invoke(FSMConvertToChild());
    }

    /// <summary>
    /// Obligatorio en caso de que se utilice la version serializable para unity
    /// </summary>
    /// <param name="reference"></param>
    public virtual void Init(TContext reference)
    {
        this.context = reference;
    }
}