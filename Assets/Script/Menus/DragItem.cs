using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector]
    public Transform parentAfterDrag;
    [HideInInspector]
    public CanvasGroup myCanvasGroup;
    [HideInInspector]
    public Transform lastParent;
    [SerializeField]
    public Transform canvas;

    private void Awake()
    {
        lastParent = transform.parent;

        //Debug.Log(lastParent.name);

        myCanvasGroup = GetComponent<CanvasGroup>();

        canvas = GetComponentInParent<Canvas>().transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        myCanvasGroup.blocksRaycasts = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(canvas);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        myCanvasGroup.blocksRaycasts = true;
        transform.SetAsLastSibling();
    }
}
