using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Behaviour { Seek, Flee };
public class Hunter : MonoBehaviour
{
    [SerializeField]
    public MoveAbstract obj;

    public MoveAbstract me;

    [SerializeField]
    public List<Vector3> carlitos= new List<Vector3>(); 

    [SerializeField]
    float _maxSpeed = 7;

    [SerializeField]
    float _desaceleration = 1f;

    Vector2 _desiredVelocity;
    Vector2 _steering ;

    public Behaviour desiredBehav = Behaviour.Seek;

    Carlitos fsmCarlitos;

    public LayerMask layerMask;

    public void Arrive()
    {
        _desiredVelocity = Vector2.ClampMagnitude(Direction(), _maxSpeed);

        if (_desiredVelocity.sqrMagnitude < me.vectorVelocity.sqrMagnitude / (_desaceleration* _desaceleration))
            _desiredVelocity = -me.vectorVelocity * (_desaceleration-1);

        _steering = _desiredVelocity - me.vectorVelocity;

        me.Acelerator(_steering);
    }

    public void Seek(float mukltiply)
    {
        _desiredVelocity = Direction(mukltiply).normalized * _maxSpeed;

        _steering = _desiredVelocity - me.vectorVelocity;

        me.Acelerator(_steering);
    }

    Vector2 Direction(float multiply=1)
    {
        if (carlitos.Count<=0)
            return Vector2.zero;

        Vector2 _direction = (carlitos[carlitos.Count - 1] - transform.position).Vect3To2();

        _direction *= multiply;

        if (carlitos.Count > 1 && _direction.sqrMagnitude < 0.1f* 0.1f)
            carlitos.RemoveAt(carlitos.Count-1);

        return _direction;
    }

    private void Start()
    {
        carlitos.Add(transform.position);

        fsmCarlitos = new Carlitos(this);
    }

    private void Update()
    {
        Arrive();

        RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, obj.transform.position - transform.position);

        if (raycastHit2D.transform == obj.transform)
        {
            fsmCarlitos.CurrentState = fsmCarlitos.persecucion;
        }
        else
        {
            fsmCarlitos.CurrentState = fsmCarlitos.vuelta;
        }

        Debug.DrawRay(transform.position, obj.transform.position-transform.position);
    }

    private void FixedUpdate()
    {
        fsmCarlitos.UpdateState();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        foreach (var item in carlitos)
        {
            Gizmos.DrawSphere(item, 0.1f);
        }
    }
}


public class Carlitos : FSM<Carlitos, Hunter>
{
    public IState<Carlitos> persecucion = new Persecucion();

    public IState<Carlitos> vuelta = new Vuelta();

    public Carlitos(Hunter reference) : base(reference)
    {
        Init(vuelta);
    }
}

public class Persecucion : IState<Carlitos>
{
    public void OnEnterState(Carlitos param)
    {
        if (param.context.carlitos.Count >= 3)
            return;

        param.context.carlitos.Add(param.context.transform.position);
        param.context.carlitos.Add(param.context.obj.transform.position);
    }

    public void OnExitState(Carlitos param)
    {
    }

    public void OnStayState(Carlitos param)
    {
        param.context.carlitos[param.context.carlitos.Count - 1] = param.context.obj.transform.position;

        Vector3 dir = param.context.carlitos[param.context.carlitos.Count - 3] - param.context.transform.position;

        RaycastHit2D raycastHit2D = Physics2D.Raycast(param.context.transform.position, dir, dir.magnitude);

        Debug.DrawRay(param.context.transform.position, dir, Color.blue);

        if (raycastHit2D.transform == null)
            param.context.carlitos[param.context.carlitos.Count - 2] = param.context.transform.position;
        else
        {
            param.context.carlitos.Insert(param.context.carlitos.Count - 2, param.context.transform.position);
        }
    }
}

public class Vuelta : IState<Carlitos>
{
    public void OnEnterState(Carlitos param)
    {
    }

    public void OnExitState(Carlitos param)
    {
    }

    public void OnStayState(Carlitos param)
    {

    }
}