using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateWithEnd<T> : IState<T>
{
    bool end { get; }
}
