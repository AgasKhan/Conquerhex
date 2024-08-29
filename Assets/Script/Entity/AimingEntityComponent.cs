using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;

public class AimingEntityComponent : ComponentOfContainer<Entity>
{
    public enum Mode
    {
        topdown,
        perspective,
        focus
    }

    public event System.Action<Mode> onMode;

    public event System.Action<Vector3> onAiming;

    public Transform track;

    public Vector3 AimingToObjective
    {
        get => _aimingToObj;
        set
        {
            _aimingToObj = value;
            onAiming?.Invoke(_aimingToObj);
        }
    }

    public Mode mode
    {
        get => _mode;
        set
        {
            _mode = value;
            onMode.Invoke(value);
        }
    }

    [SerializeField]
    Vector3 _aimingToObj;

    [SerializeField]
    Mode _mode = Mode.topdown;

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
