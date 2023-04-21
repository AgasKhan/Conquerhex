using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IAEnemy : Seek, IControllerDir
{
    [SerializeField] float _viewRadius;
    [SerializeField] Transform[] _totalWaypoints;

    int _currentWaypoint;

    Action _OnCurrentPath;

    #region Move
    public void ControllerDown(Vector2 dir, float tim)
    {
    }

    public void ControllerPressed(Vector2 dir, float tim)
    {
        Transform nextWaypoint = _totalWaypoints[_currentWaypoint];
        Vector2 dirToWaypoint = DirectionSeek(move.Director(nextWaypoint.position));

        //Si no está dentro del rango de visión, patrulla
        if (_viewRadius * _viewRadius >= dirToWaypoint.sqrMagnitude)
        {
            _OnCurrentPath();
        }

        //Sino, persigue
        else
        {
            DirectionPursuit(move.Director(dir));
        }
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
    #endregion

    #region Attack

    public virtual void ControllerDownATK(Vector2 dir, float tim)
    {
        //Ataque 1
    }

    public virtual void ControllerPressedATK(Vector2 dir, float tim)
    {
        //Ataque 2
    }

    public virtual void ControllerUpATK(Vector2 dir, float tim)
    {
        //Ataque 3
    }

    #endregion
}


