using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAFather : MonoBehaviour, IState<Character>
{
    public abstract void OnEnterState(Character param);
    public abstract void OnExitState(Character param);
    public abstract void OnStayState(Character param);
}
