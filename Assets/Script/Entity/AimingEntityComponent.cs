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

    public event System.Action<Vector3> onAimingXZ;

    public Vector3 ObjectivePosition
    {
        get => _objectivePosition;
        set
        {
            _objectivePosition = value;
            _aimingToObj = _objectivePosition - (transform.position + offsetView);
            _aimingToObj.Normalize();
            onAimingXZ.Invoke(AimingToObjectiveXZ);
        }
    }

    /// <summary>
    /// Setea en base a un vector2 de forma indirecta el ObjectivePosition <br/>
    /// Toma el Y como Z
    /// </summary>
    public Vector2 AimingToObjective2D
    {
        set
        {
            ObjectivePosition = (transform.position + offsetView) + value.Vect2To3XZ(0);
        }
    }

    /// <summary>
    /// Vector de direccion desde el offset al objetivo
    /// </summary>
    public Vector3 AimingToObjective
    {
        get => _aimingToObj;
    }

    /// <summary>
    /// Vector que desprecia su componente en Y de direccion desde el offset al objetivo
    /// </summary>
    public Vector3 AimingToObjectiveXZ
    {
        get => _aimingToObj.Vect3Copy_Y(0);
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
    Vector3 _objectivePosition;

    [SerializeField]
    Vector3 _aimingToObj;

    [SerializeField]
    Vector3 offsetView;

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
