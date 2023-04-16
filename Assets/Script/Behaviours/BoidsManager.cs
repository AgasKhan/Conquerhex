using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoidsManager : SingletonMono<BoidsManager>
{
    public float ViewRadius
    {
        get
        {
            return _viewRadius * _viewRadius;
        }
    }
    [SerializeField] float _viewRadius;

    public float SeparationRadius
    {
        get
        {
            return _separationRadius * _separationRadius;
        }
    }
    [SerializeField] float _separationRadius;

    [field: SerializeField, Range(0f, 2.5f)]
    public float SeparationWeight { get; private set; }

    [field: SerializeField, Range(0f, 2.5f)]
    public float AlignmentWeight { get; private set; }

    [field: SerializeField, Range(0f, 2.5f)]
    public float CohesionWeight { get; private set; }
}
