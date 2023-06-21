using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Node : MonoBehaviour
{
    [SerializeField]
    List<Node> _neighbors = new List<Node>();

    public int cost = 1;

    public List<Node> getNeighbors => _neighbors;
}
