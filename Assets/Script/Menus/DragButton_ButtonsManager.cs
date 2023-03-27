using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragButton_ButtonsManager : ButtonsManager, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector]
    public Transform parentAfterDrag;
    [HideInInspector]
    public CanvasGroup myCanvasGroup;
    [HideInInspector]
    public Transform originalParent;

    [SerializeField]
    Sprite _mySprite;
    [SerializeField]
    DoubleString _information;

    private void Awake()
    {
        originalParent = transform.parent;

        myCanvasGroup = GetComponent<CanvasGroup>();

        menu.eventListVoid.AddRange(new Pictionarys<string, System.Action<GameObject>>()
        {

            {"ShowWindow", ShowWindow}

        });
    }
    
    void ShowWindow(GameObject g)
    {
        DetailsWindow.instance.SetWindow(_mySprite, _information);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.parent.parent.parent.parent);
        transform.SetAsLastSibling();
        myCanvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        myCanvasGroup.blocksRaycasts = true;
    }

}
