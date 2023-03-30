using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotsParent : MonoBehaviour, IDropHandler
{
    protected DragItem draggableItem;

    [SerializeField]
    Transform Container;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;

        //dropped.transform.SetAsLastSibling();

        draggableItem = dropped.GetComponent<DragItem>();

        if(draggableItem!=null)
            AcceptedDrop();


        /*
        if (Container == draggableItem.originalParent)
        {
            DeclinedDrop();
            Debug.Log("NO se acepto el drop");
        }
        else
        {
            AcceptedDrop();
            Debug.Log("Se acepto el drop");
        }
        */
    }

    public virtual void AcceptedDrop()
    {
        draggableItem.parentAfterDrag = Container;
    }

    public virtual void DeclinedDrop()
    {
        draggableItem.parentAfterDrag = draggableItem.originalParent;
    }

}
