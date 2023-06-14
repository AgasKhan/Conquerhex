using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentRectTransform : MonoBehaviour
{
    public RectTransform rectTransform;

    protected virtual void Awake()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
    }
}
