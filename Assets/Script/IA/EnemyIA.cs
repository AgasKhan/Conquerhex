using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyIA : MonoBehaviour
{
    MoveAbstract move;

    Seek seek;
    Persuit pursuit;

    GameObject target;

    [SerializeField] float _viewRadius;
    [SerializeField] float _detectionRadius;
    [SerializeField] Transform[] _totalWaypoints;

    int _currentWaypoint;

    [SerializeField] Transform _target;

    Action _OnCurrentPath;

    private void Start()
    {
        
    }

    public void Patrol()
    {
        Transform nextWaypoint = _totalWaypoints[_currentWaypoint];
        Vector2 dirToWaypoint = seek.Calculate(move.Director(nextWaypoint.position));

        if (_viewRadius * _viewRadius >= dirToWaypoint.sqrMagnitude)
        {
            _OnCurrentPath();

        }

    }

    public void Detection()
    {
        Vector2 dirToTarget = seek.Calculate(move.Director(target.transform.position));

        if(dirToTarget.sqrMagnitude <= _viewRadius * _viewRadius)
        {
            pursuit.Calculate(move.Director(target.transform.position));

            if(dirToTarget.sqrMagnitude <= _detectionRadius * _detectionRadius)
            {
                //Attack
            }
        }

    }

    public void Return()
    {
        BackwardPath();
    }

    void NormalPath()
    {
        _currentWaypoint++;

        if (_currentWaypoint >= _totalWaypoints.Length)
        {
            _currentWaypoint--;

            _OnCurrentPath = BackwardPath;
        }
    }

    void BackwardPath()
    {
        _currentWaypoint--;

        if (_currentWaypoint < 0)
        {
            _currentWaypoint++;

            _OnCurrentPath = NormalPath;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(_viewRadius));
    }

    private void Update()
    {

        

    }

}




