using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ObstacleAvoidance : SteeringBehaviour
{
    public SteeringBehaviour steering;

    [SerializeField]
    float angle = 60;

    [SerializeReference]
    Timer dirChange;

    int dirSigned = 1;

    protected override void Awake()
    {
        dirChange = TimersManager.Create(2, () => dirSigned *= -1).SetLoop(true);
    }

    public override SteeringBehaviour SwitchSteering<T>()
    {
        steering.SwitchSteering<T>();

        return this;
    }

    protected override Vector2 InternalCalculate(MoveAbstract target)
    {
        if (steering == null)
            return Vector2.zero;

        _direction = steering.Calculate(target);

        var aux = Physics2D.RaycastAll(transform.position, _direction, _direction.magnitude, GameManager.instance.obstacleAvoidanceLayer);

        if (aux != null && aux.Length > 1)
        {
            _direction = Quaternion.Euler(0, 0, angle * dirSigned) * _direction;
        }

        return _direction;
    }
}