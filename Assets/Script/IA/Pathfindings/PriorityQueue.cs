using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T>
{
    Dictionary<T, float> _allElements;

    public int Count { get { return _allElements.Count; } }

    public PriorityQueue()
    {
        _allElements = new Dictionary<T, float>();
    }

    public void Enqueue(T elem, float cost)
    {
        //Si no contiene la key en el diccionario, creamos el par
        if (!_allElements.ContainsKey(elem))
            _allElements.Add(elem, cost);
        else //Sino lo guardamos
            _allElements[elem] = cost;
    }

    public T Dequeue()
    {
        T elem = default;

        //Empiezo a chequear costos desde el valor maximo que se puede tener
        var currentValue = float.MaxValue;

        //Itero cada elemento (par) del diccionario
        foreach (var item in _allElements)
        {
            //Si el costo del nodo actual es menor
            if (currentValue > item.Value)
            {
                //Me guardo ese nodo para devolver
                elem = item.Key;

                //Guardo ese costo para chequear en la siguiente iteracion
                currentValue = item.Value;
            }
        }

        //Remuevo del diccionario
        _allElements.Remove(elem);

        //Lo devuelvo
        return elem;
    }
}
