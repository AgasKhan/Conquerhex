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

    Transform Container;


    private void Start()
    {
        Container = GetComponentInChildren<VerticalLayoutGroup>().transform.GetChild(0);

        //LoadSystem.AddPostLoadCorutine(InitSlots);

        Extensions.SlotEvent(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
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
    IEnumerator InitSlots(System.Action<bool> end, System.Action<string> msg)
    {
        Extensions.SlotEvent(this);
        end(true);
        yield return null;
    }
}
