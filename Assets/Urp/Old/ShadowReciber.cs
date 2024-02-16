using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ShadowReciber : MonoBehaviour
{
    new Renderer renderer;

    public int order;

    public string layer = "Shadow";

    private void Awake()
    {
        renderer = GetComponent<Renderer>();        
    }

    private void OnEnable()
    {
        renderer.sortingLayerName = layer;

        renderer.sortingOrder = order;
    }

}