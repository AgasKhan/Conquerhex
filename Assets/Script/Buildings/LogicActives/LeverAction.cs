using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverAction : LogicActive<(InteractEntityComponent interact, Character character)>
{
    public UnityEngine.Events.UnityEvent action;
    public override void Activate((InteractEntityComponent interact, Character character) genericParams)
    {
        action.Invoke();
    }
}
