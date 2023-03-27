using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotsParent : MonoBehaviour, IDropHandler
{
    protected DragButton_ButtonsManager draggableItem;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;

        dropped.transform.SetAsLastSibling();

        draggableItem = dropped.GetComponent<DragButton_ButtonsManager>();

        /*
        if (transform != draggableItem.originalParent)
            DeclinedDrop();
        else
            AcceptedDrop();
        */
    }

    public virtual void AcceptedDrop()
    {
        draggableItem.parentAfterDrag = transform;
    }

    public virtual void DeclinedDrop()
    {
        draggableItem.parentAfterDrag = draggableItem.originalParent;

    }

}
