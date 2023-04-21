using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Slot : MonoBehaviour, IDropHandler
{
    //------------------------------------------------------
    public BodyPart SlotOf;
    //------------------------------------------------------

    protected DragItem draggableItem;

    public Action <GameObject> onAcceptDrop;

    [SerializeField]
    Transform Container;


    private void Start()
    {
        Container = GetComponentInChildren<VerticalLayoutGroup>().transform;

        //LoadSystem.AddPostLoadCorutine(InitSlots);

        //Extensions.SlotEvent(this);

        //LoadSystem.AddPostLoadCorutine(InitSlots);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (onAcceptDrop == null)
            Extensions.SlotEvent(this);

        GameObject dropped = eventData.pointerDrag;

        //dropped.transform.SetAsLastSibling();

        draggableItem = dropped.GetComponent<DragItem>();

        //------------------------------------------------------
        var modObject = dropped.GetComponent<ModsManager>();

        if (modObject != null)
        {
            if (modObject.myPart == SlotOf)
                AcceptedDrop();
            else
                DeclinedDrop();
        }
        else if (draggableItem != null)
            AcceptedDrop();

        //------------------------------------------------------

    }

    public virtual void AcceptedDrop()
    {
        draggableItem.parentAfterDrag = Container;
        draggableItem.lastParent = Container;
        onAcceptDrop(this.gameObject);
    }

    public virtual void DeclinedDrop()
    {
        draggableItem.parentAfterDrag = draggableItem.lastParent;
    }

    /*
    public SlotsParent(DragItem draggableItem, Action onAcceptDrop, Transform container)
    {
        this.draggableItem = draggableItem;
        this.onAcceptDrop = onAcceptDrop;
        Container = container;
    }
    */
}
