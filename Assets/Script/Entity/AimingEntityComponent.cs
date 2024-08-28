using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;

public class AimingEntityComponent : ComponentOfContainer<Entity>
{
    public Vector3 Aiming
    {
        get => _aiming;
        set
        {
            _aiming = value;
            onAiming?.Invoke(value);
        }
    }

    public event System.Action<Vector3> onAiming;

    [SerializeField]
    Vector3 _aiming;

    public List<AbstractDetectParent> detects = new List<AbstractDetectParent>();

    public override void OnEnterState(Entity param)
    {
    }

    public override void OnExitState(Entity param)
    {   
    }

    public override void OnStayState(Entity param)
    {   
    }
}
