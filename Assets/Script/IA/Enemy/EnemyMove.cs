using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyMove : Seek, IControllerDir
{
    //[SerializeField] float _viewRadius;
    [SerializeField] Transform[] _totalWaypoints;

    int _currentWaypoint;

    [SerializeField] Transform _target;

    Action _OnCurrentPath;

    private void Start()
    {
        
    }

    public void ControllerDown(Vector2 dir, float tim)
    {
        Debug.Log("Down");
    }

    public void ControllerPressed(Vector2 dir, float tim)
    {
        //Transform nextWaypoint = _totalWaypoints[_currentWaypoint];
        //Vector2 dirToWaypoint = DirectionSeek(move.Director(nextWaypoint.position));

        ////Si no est� dentro del rango de visi�n, patrulla
        //if (_viewRadius * _viewRadius >= dirToWaypoint.sqrMagnitude)
        //{

        //    _OnCurrentPath();
        //    //ControllerPressed(dirToWaypoint, 0);

        //}

        ////Sino, persigue
        //else
        //{
        //    DirectionPursuit(move.Director(_target.position));
        //    Debug.Log("Pursuit");
        //}
        

    }

    public void ControllerUp(Vector2 dir, float tim)
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


    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.white;
    //    Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(_viewRadius));
    //}

    private void Update()
    {

        

    }
}




