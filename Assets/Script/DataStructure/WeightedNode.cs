using System;
using UnityEngine;

[Serializable]
public class WeightedNode<T> : IComparable<WeightedNode<T>>
{
    [SerializeField] private T _element;
    [SerializeField] private float _weight;

    public T Element => _element;

    public float Weight { get => _weight; set => _weight = value; }

    public WeightedNode(T element, float weight)
    {
        _element = element;
        _weight = weight;
    }

    public int CompareTo(WeightedNode<T> other)
    {
        return _weight.CompareTo(other.Weight);
    }
}