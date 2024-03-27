using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueueWithDick<T>
{
    PriorityQueue<WeightedNode<T>> priorityQueue = new PriorityQueue<WeightedNode<T>>();

    Dictionary<T, WeightedNode<T>> keyValues = new Dictionary<T, WeightedNode<T>>();

    public int Count { get { return priorityQueue.Count; } }

    public bool ContainsKey(T elem) => keyValues.ContainsKey(elem);

    public void Enqueue(T elem, float cost)
    {

        if(!keyValues.ContainsKey(elem))
        {
            var aux = new WeightedNode<T>(elem, cost);

            priorityQueue.Enqueue(aux);

            keyValues.Add(elem, aux);
        }
        else
        {
            keyValues[elem].Weight = cost;

            priorityQueue.UpdateElement(keyValues[elem]);
        }
    }

    public T Dequeue()
    {
        var aux = priorityQueue.Dequeue();
        keyValues.Remove(aux.Element);
        return aux.Element;
    }
}


