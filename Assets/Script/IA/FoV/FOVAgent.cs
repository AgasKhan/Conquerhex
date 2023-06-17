using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVAgent : MonoBehaviour
{
    [SerializeField] List<Transform> enemies;

    [SerializeField] float _viewRadius;
    [SerializeField] float _viewAngle;
    [SerializeField] LayerMask obstacleLayer;

    void FixedUpdate()
    {
        foreach (var enemy in enemies)
        {
            if (InFieldOfView(enemy.position))
            {
                enemy.GetComponent<Renderer>().material.color = Color.red;
                Debug.DrawLine(transform.position, enemy.position, Color.red);
            }
            else
            {
                enemy.GetComponent<Renderer>().material.color = Color.white;
            }
        }
    }

    bool InFieldOfView(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;

        //Que este dentro de la distancia maxima de vision
        if (dir.sqrMagnitude > _viewRadius * _viewRadius) return false;

        //Que no haya obstaculos
        if (!InLineOfSight(dir)) return false;

        //Que este dentro del angulo
        return Vector3.Angle(transform.forward, dir) <= _viewAngle / 2;


    }

    bool InLineOfSight(Vector3 direction)
    {
        return !Physics.Raycast(transform.position, direction, _viewRadius, obstacleLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _viewRadius);

        var realAngle = _viewAngle / 2;

        Gizmos.color = Color.magenta;
        Vector3 lineLeft = GetDirFromAngle(-realAngle + transform.eulerAngles.y);
        Gizmos.DrawLine(transform.position, transform.position + lineLeft * _viewRadius);

        Vector3 lineRight = GetDirFromAngle(realAngle + transform.eulerAngles.y);
        Gizmos.DrawLine(transform.position, transform.position + lineRight * _viewRadius);
    }

    Vector3 GetDirFromAngle(float angle)
    {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
