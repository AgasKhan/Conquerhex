using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState<T>
{
    void OnEnterState(T param);

    void OnStayState(T param);

    void OnExitState(T param);
}

public interface IState
{
    void OnEnterState();

    void OnStayState();

    void OnExitState();
}

public interface ISwitchState<Context, State> where State : IState<Context>
{
    State CurrentState { get; set; }
}

