using System.Collections.Generic;
using System;
using System.Collections;

public class PriorityQueue<T> : IEnumerable<T> where T : IComparable<T>
{
    private List<T> _heap = new List<T>();

    public bool IsEmpty => _heap.Count == 0;

    public int Count => _heap.Count; 

    public void UpdateElement(T element)
    {
        int index = _heap.IndexOf(element);

        if (index == -1)
        {
            throw new ArgumentException("El elemento no está en la cola de prioridad.");
        }

        // Actualizar el elemento
        _heap[index] = element;

        // Realizar HeapifyUp y HeapifyDown para mantener las propiedades de la cola de prioridad
        HeapifyUp(index);
        HeapifyDown(index);
    }

    public void Enqueue(T element)
    {
        _heap.Add(element);
        HeapifyUp(_heap.Count - 1);
    }

    public T Dequeue()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException("PriorityQueue is empty");
        }

        var min = _heap[0];
        _heap[0] = _heap[_heap.Count - 1];
        _heap.RemoveAt(_heap.Count - 1);
        HeapifyDown(0);

        return min;
    }

    public void Clear()
    {
        _heap.Clear();
    }


    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;

            if (_heap[index].CompareTo(_heap[parent]) >= 0)
            {
                break;
            }

            Swap(index, parent);
            index = parent;
        }
    }

    private void HeapifyDown(int index)
    {
        int leftChild, rightChild, smallestChild;

        while (true)
        {
            leftChild = 2 * index + 1;
            rightChild = 2 * index + 2;
            smallestChild = index;

            if (leftChild < _heap.Count && _heap[leftChild].CompareTo(_heap[smallestChild]) < 0)
            {
                smallestChild = leftChild;
            }

            if (rightChild < _heap.Count && _heap[rightChild].CompareTo(_heap[smallestChild]) < 0)
            {
                smallestChild = rightChild;
            }

            if (smallestChild == index)
            {
                break;
            }

            Swap(index, smallestChild);
            index = smallestChild;
        }
    }

    private void Swap(int i, int j)
    {
        (_heap[i], _heap[j]) = (_heap[j], _heap[i]);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)_heap).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_heap).GetEnumerator();
    }
}

