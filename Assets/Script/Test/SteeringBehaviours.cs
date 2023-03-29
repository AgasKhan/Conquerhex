using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Behaviour { Seek, Flee };
public class SteeringBehaviours : MonoBehaviour
{
    [SerializeField]
    public Vector2Quad _obj;

    [SerializeField]
    public List<Vector3> carlitos= new List<Vector3>(); 

    [SerializeField]
    float _maxSpeed = 7;

    [SerializeField]
    float _desaceleration = 1f;

    Vector2 _desiredVelocity;
    Vector2 _steering ;
    Vector2 _velocity = Vector2.zero;

    public Behaviour desiredBehav = Behaviour.Seek;

    Carlitos fsmCarlitos;

    public LayerMask layerMask;

    public void Arrive()
    {
        _desiredVelocity = Vector2.ClampMagnitude(Direction(), _maxSpeed);

        if (_desiredVelocity.sqrMagnitude < _velocity.sqrMagnitude / (_desaceleration* _desaceleration))
            _desiredVelocity = -_velocity*(_desaceleration-1);

        _steering = _desiredVelocity - _velocity;

        AddVelocity(_steering);
    }

    public void Seek(float mukltiply)
    {
        _desiredVelocity = Direction(mukltiply).normalized * _maxSpeed;

        _steering = _desiredVelocity - _velocity;

        AddVelocity(_steering);
    }

    Vector2 Direction(float mukltiply=1)
    {
        if (carlitos.Count<=0)
            return Vector2.zero;

        Vector2 _direction = (carlitos[carlitos.Count - 1] - transform.position).Vect3To2();

        _direction *= mukltiply;

        if (carlitos.Count > 1 && _direction.sqrMagnitude < 0.1f* 0.1f)
            carlitos.RemoveAt(carlitos.Count-1);

        return _direction;
    }

    void AddVelocity(Vector2 velocity)
    {
        _velocity += velocity * Time.deltaTime;//sumo la velocidad en metros por segundo
    }

    private void Locomotion()
    {
        transform.position += (_velocity * Time.deltaTime).Vec2to3(0);//me muevo en metros por segundo
    }


    /*
    void Manolo(float mukltiply)
    {
        Vector2 _direction = (_obj.tr.position - transform.position).Vect3To2() + _obj.velocity;

        _direction *= mukltiply;

        _steering = _direction - _velocity;

        if (_steering.sqrMagnitude> _aceleration * _aceleration)
            _steering = Vector2.ClampMagnitude(_steering, _aceleration);

        _velocity += _steering;
    }


    private void Move()
    {
        if (_velocity.sqrMagnitude > _maxSpeed * _maxSpeed)
            _velocity = Vector2.ClampMagnitude(_velocity, _maxSpeed);

        transform.position += (_velocity * Time.deltaTime).Vec2to3(0);
    }
    */

    private void Start()
    {
        carlitos.Add(transform.position);

        fsmCarlitos = new Carlitos(this);
    }

    private void Update()
    {
        Arrive();

        _obj.LoadVelocity();

        RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, _obj.tr.position - transform.position);

        if (raycastHit2D.transform == _obj.tr)
        {
            fsmCarlitos.CurrentState = fsmCarlitos.persecucion;

        }
        else
        {
            fsmCarlitos.CurrentState = fsmCarlitos.vuelta;
        }

        Debug.DrawRay(transform.position, _obj.tr.position-transform.position);

        

        Locomotion();

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


public class Carlitos : FSM<Carlitos, SteeringBehaviours>
{
    public IState<Carlitos> persecucion = new Persecucion();

    public IState<Carlitos> vuelta = new Vuelta();

    public Carlitos(SteeringBehaviours reference) : base(reference)
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
        param.context.carlitos.Add(param.context._obj.tr.position);
    }

    public void OnExitState(Carlitos param)
    {
    }

    public void OnStayState(Carlitos param)
    {
        param.context.carlitos[param.context.carlitos.Count - 1] = param.context._obj.prev;

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