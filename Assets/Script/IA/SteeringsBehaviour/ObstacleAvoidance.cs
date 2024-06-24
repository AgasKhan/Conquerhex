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
        steering = steering.SwitchSteering<T>();

        return this;
    }

    protected override Vector3 InternalCalculate(MoveAbstract target)
    {
        if (steering == null)
            return Vector3.zero;

        _direction = steering.Calculate(target);

        if (Physics.Raycast(transform.position, _direction, _direction.magnitude, GameManager.instance.obstacleAvoidanceLayer))
        {
            _direction = Quaternion.Euler(0, angle * dirSigned, 0) * _direction;
        }

        return _direction;
    }
}