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
    [System.Serializable]
    public struct CameraSet : IDetect
    {
        [Header("Camera")]
        public Vector3 offsetObjPosition;

        public Vector3 rotationPerspective;

        public Vector3 vectorPerspective;

        [field: SerializeField, Header("Detection")]
        public float maxRadius { get ; set ; }
        [field: SerializeField]
        public float minRadius { get ; set ; }
        [field: SerializeField]
        public bool inverse { get ; set ; }
        [field: SerializeField]
        public float angle { get ; set ; }
        [field: SerializeField]
        public float dot { get ; set ; }
        [field: SerializeField]
        public int maxDetects { get; set; }
        [field: SerializeField]
        public int minDetects { get; set; }
    }

    [SerializeField]
    public CameraSet[] sets;

    public event System.Action<Mode> onMode;

    public event System.Action<Vector3> onAiming;

    public Vector3? ObjectivePosition;

    public Vector3 AimingToObjective
    {
        get => _aimingToObj;
        set
        {
            _aimingToObj = value;
            Debug.DrawRay(transform.position, _aimingToObj);
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
