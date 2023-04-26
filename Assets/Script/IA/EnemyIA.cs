using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyIA : IAFather
{
    //MoveAbstract move;

    Seek seek;
    Persuit pursuit;

    [SerializeField]
    MoveAbstract me;
    [SerializeField]
    MoveAbstract target; 
    
    //GameObject target;

    [SerializeField] float _viewRadius;
    [SerializeField] float _detectionRadius;

    WeaponKataBase wb;
    Character character;
    //[SerializeField] Transform[] _totalWaypoints;

    //int _currentWaypoint;



    //Transform _target;

    //Action _OnCurrentPath;

    private void Start()
    {

    }

    #region patrol
    //void NormalPath()
    //{
    //    _currentWaypoint++;

    //    if (_currentWaypoint >= _totalWaypoints.Length)
    //    {
    //        _currentWaypoint--;

    //        _OnCurrentPath = BackwardPath;
    //    }
    //}

    //void BackwardPath()
    //{
    //    _currentWaypoint--;

    //    if (_currentWaypoint < 0)
    //    {
    //        _currentWaypoint++;

    //        _OnCurrentPath = NormalPath;
    //    }
    //}



    //Patrol

    //Transform nextWaypoint = _totalWaypoints[_currentWaypoint];
    //Vector2 dirToWaypoint = seek.Calculate(move.Director(nextWaypoint.position));

    //if (_viewRadius * _viewRadius >= dirToWaypoint.sqrMagnitude)
    //{
    //    _OnCurrentPath();

    //}
    //Return
    //BackwardPath();


    //Detection
    #endregion
    public override void OnEnterState(Character param)
    {
        /*Vector2 dirToTarget = seek.Calculate(me.Director(target.transform.position));

        Debug.Log("dittotarget " + dirToTarget);

        if (dirToTarget.sqrMagnitude <= _viewRadius * _viewRadius)
        {
            var aux = pursuit.Calculate(me.Director(target.transform.position));
            me.ControllerPressed(aux, 0);

            if (dirToTarget.sqrMagnitude <= _detectionRadius * _detectionRadius)
            {
                wb.IADetect(character, target.transform.position);

                Debug.Log("attack-dirto " + dirToTarget);
            }
        }
        */
    }

    public override void OnStayState(Character param)
    {
        //character = param;

        //var aux = seek.Calculate(target);
        //me.ControllerPressed(aux, 1);
    }

    public override void OnExitState(Character param)
    {
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(_viewRadius));
    }



}



