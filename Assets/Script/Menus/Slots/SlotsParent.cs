using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class SlotsParent : MonoBehaviour, IDropHandler
{
    protected DragItem draggableItem;

    public System.Action <GameObject> onAcceptDrop;

    [SerializeField]
    Transform Container;

    private void Start()
    {
        MenuManager.instance.eventListVoid.AddRange(new Pictionarys<string, System.Action<GameObject>>()
        {

            {"FarmSlot", FarmSlot},
            {"AttackSlot", AttackSlot},
            {"DefendSlot", DefendSlot}

        });

        Extensions.SlotEvent(this);
    }

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
        onAcceptDrop(this.gameObject);
    }

    public virtual void DeclinedDrop()
    {
        draggableItem.parentAfterDrag = draggableItem.originalParent;
    }

    public void FarmSlot(GameObject g)
    {
        Debug.Log("FarmingSlot");
    }

    public void AttackSlot(GameObject g)
    {
        Debug.Log("AttackSlot");
    }

    public void DefendSlot(GameObject g)
    {
        Debug.Log("DefendSlot");
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
