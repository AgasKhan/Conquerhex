using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateWithEnd<T> : IState<T>
{
    bool End { get; }
}

public interface IStateWithEndWithNext<T> : IState<T>
{
    bool End { get; }

    IStateWithEnd<T> Next { get; }
}
